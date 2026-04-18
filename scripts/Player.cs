using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class Player : CharacterBody2D
{
	// Stateful player states
	private enum State 
	{
		Idle,
		Attacking,
	}

	// Track which weapon is equipped
	private enum EquippedWeapon
	{
		Sword,
		Bow
	}
	
	// Get Player Child Nodes
	private PlayerAnimations _playerSprite;
	private AnimatedSprite2D _bowSprite;
	private CollisionShape2D _playerCollisionBox;
	private AudioStreamPlayer _playerSoundPlayer;
	private Node2D _arrowSpawnLocation;
	
	// Camera ScreenSize
	public Vector2 ScreenSize;

	[ExportGroup("Debugging")] // variables for testing
	[Export] private bool _moveOnAttack = true;
	[Export] private int _footstepVolume = -14;
	[Export] private int _swordSwingVolume = 0;
	[Export] private int _bowStringVolume = 0;
	
	// Private attributes
	[ExportGroup("Attributes")]
	[Export] private State _state = State.Idle; // stateful player
	[Export] private int _speed = 100;
	[Export] private EquippedWeapon _weapon = EquippedWeapon.Sword; // defaults to sword
	private bool _moving = false;
	
	// Attack delay timer
	private int _stateCounter = 0;
	
	// Public attributes
	[Export] public int Lives = 3;
	[Export] public int Ammo = 100;
	[Export] public int Level = 0;
	[Export] public int Armor = 0;

	// Velocity and direction
	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _inputDirection = Vector2.Zero;
	private bool _flipped = false;

	// Audio variables
	private AudioStreamWav _swordSwingSound; // sword swinging sound
	private AudioStreamWav _bowStringSound; // bow string sound
	
	// Arrow variables
	private PackedScene _arrowPreload;
	private Node _arrowInstance;
	public int ShootDirection;

	// Custom Events (Signals)
	[Signal] public delegate void MovingEventHandler(int volume); // ALL custom signals must include EventHandler suffix
	[Signal] public delegate void StoppingEventHandler();

	// On Ready (one shot)
	public override void _Ready()
	{
		// Set variables
		ScreenSize = GetViewportRect().Size;
		_playerSprite = GetNode<PlayerAnimations>("AnimatedSprite2D"); // gets AnimatedSprite child node
		_bowSprite = GetNode<AnimatedSprite2D>("bowAnimations"); // gets AnimatedSprite child node for bow
		
		// Audio config
		// Get audio-related nodes
		_playerSoundPlayer = GetNode<AudioStreamPlayer>("PlayerAudioPlayer"); // gets AudioStreamPlayer child node
		// Preload audio files
		LoadAudioFiles();
		
		// Load arrow
		_arrowPreload = GD.Load<PackedScene>("res://scenes/arrow.tscn"); // load arrow scene to be spawned (instantiated)
		_arrowSpawnLocation = GetNode<Node2D>("ArrowSpawnLocation"); // loads arrow spawn point
		
		// Signals
		_playerSprite.AnimationFinishedWithName += OnAnimationFinishedWithNameSignal;
	}

	private void LoadAudioFiles()
	{
		// Sword swing sounds
		_swordSwingSound = GD.Load<AudioStreamWav>("res://assets/sounds/sword_swing.wav");
		_bowStringSound = GD.Load<AudioStreamWav>("res://assets/sounds/bow_string.wav");
	}
	

	// Get input direction / Velocity
	private void GetMovement() // true for moving, false for not moving
	{
		_inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (!_moveOnAttack) // if false
		{
			if (_state == State.Attacking)
			{
				Velocity = Vector2.Zero;
			}
			else Velocity = _inputDirection * _speed;
		}
		else Velocity = _inputDirection * _speed;

		if (!_moving && Velocity.Length() > 0)
		{
			EmitSignal(SignalName.Moving, _footstepVolume);
		} 
		else if (_moving && Velocity == Vector2.Zero)
		{
			EmitSignalStopping();
		}
		
		if (Velocity != Vector2.Zero) _moving = true;
		else _moving = false;
	}

	// Movement animation
	private void AnimateMovement()
	{
		_bowSprite.Visible = true;
		if (_inputDirection.X > 0) // go right
		{
			if (_inputDirection.Y < 0) _playerSprite.Play("run_up"); // up right
			else if (_inputDirection.Y > 0) _playerSprite.Play("run_down"); // down right
			else _playerSprite.Play("run_side"); // just right
		}

		if (_inputDirection.X < 0) // go left
		{
			if (_inputDirection.Y < 0) _playerSprite.Play("run_up"); // up left
			else if (_inputDirection.Y > 0) _playerSprite.Play("run_down"); // down left
			else _playerSprite.Play("run_side"); // just left
		}

		if (_inputDirection.Y < 0) { _playerSprite.Play("run_up"); } // just up

		if (_inputDirection.Y > 0) { _playerSprite.Play("run_down"); } // just down

		if (_inputDirection.Length() == 0 && _state == State.Idle) { _playerSprite.Play("idle"); } // no movement

	}

	private void AnimateBow()
	{
		if (_weapon == EquippedWeapon.Bow)
		{
			if (Velocity.Y < 0) _bowSprite.ZIndex = -1;
			else _bowSprite.ZIndex = 1;
		}
		if (_weapon != EquippedWeapon.Bow)
		{
			if (Velocity.Y < 0) _bowSprite.ZIndex = 1;
			else _bowSprite.ZIndex = -1;
		}
	}

	// Attack animation
	private void AnimateAttack()
	{
		if (_weapon == EquippedWeapon.Sword && !_playerSprite.IsPlaying())
		{
			PlaySwordSwingAudio();
			_playerSprite.Play("sword_attack");
		}
		// bow animation here
		if (_weapon == EquippedWeapon.Bow && !_playerSprite.IsPlaying()) //_state = State.Idle; // temporary until bow animation is finished
		{
			_playerSprite.Play("bow_attack");
			_bowSprite.Visible = false;
		} 
	}

	private void PlaySwordSwingAudio()
	{
		_playerSoundPlayer.Stream = _swordSwingSound;
		_playerSoundPlayer.VolumeDb = _swordSwingVolume;
		_playerSoundPlayer.Play();
	}

	private async void PlayBowStringAudio()
	{
		_playerSoundPlayer.Stream = _bowStringSound;
		_playerSoundPlayer.VolumeDb = _bowStringVolume;
		// TO HAVE A TEMPORARY ONE OFF TIMER
		// await ToSignal(GetTree().CreateTimer(0.25), SceneTreeTimer.SignalName.Timeout);
		// Better way: wait for the actual frame of the animation
		await ToSignal(_playerSprite, PlayerAnimations.SignalName.BowReleased);
		_playerSoundPlayer.Play();
	}

	public bool _readyToShoot = true;
	private void OnAnimationFinishedWithNameSignal(string animationName)
	{
		if (_state == State.Attacking)
		{
			_state = State.Idle;
		}

		if (animationName == "bow_attack")
		{
			_readyToShoot = true;
		}
	}

	// Flips sprite regardless of state
	private void FlipSprite()
	{
		if (_state != State.Attacking)
		{
			if (_inputDirection.X > 0)
			{
				_flipped = false;
				_playerSprite.FlipH = false;
				_bowSprite.FlipH = false;
			}
			if (_inputDirection.X < 0)
			{
				_flipped = true;
				_playerSprite.FlipH = true;
				_bowSprite.FlipH = true;
			}
			
		}
	}

	// Flips the arrow spawn
	private Vector2 _arrowSpawnPosition;
	private void FlipArrowSpawn()
	{
		if (!_flipped) // facing right
		{
			_arrowSpawnPosition = new Vector2(5, 1);
		}
		else // facing left
		{
			_arrowSpawnPosition = new Vector2(-5, 1);
		}

		_arrowSpawnLocation.Position = _arrowSpawnPosition;
	}

	// Gets current state every frame
	private void GetState()
	{
		if (Input.IsActionJustPressed("attack") && _state == State.Idle)
		{
			_state = State.Attacking;
			_playerSprite.Stop();
			Attack(); // having Attack() here means that there is a delay on any attack, bow or sword
					  // if this is undesired we need to have another system
		}
	}

	public void EquipWeapon()
	{
		if (Input.IsActionJustPressed("equip_sword") && _weapon != EquippedWeapon.Sword) 
		{
			_weapon = EquippedWeapon.Sword;
			_bowSprite.Play("unequipped");
		}
		else if (Input.IsActionJustPressed("equip_bow") && _weapon != EquippedWeapon.Bow) 
		{
			_weapon = EquippedWeapon.Bow;
			_bowSprite.Play("equipped");
		}
	}

	public async void Attack()
	{
		ShootDirection = (_flipped) ? -1 : 1; // -1 for left, 1 for right
		// switch-case statements are perfect for enums!	
		switch (_weapon) // basically says "inspect this variable", in this case, "_weapon" defined at the top
		{                // and match it one of the listed options
			case EquippedWeapon.Sword: // equivalent to if (_weapon == EquippedWeapon.Sword)
				// sword behavior
				break; // have to "break out" of the switch statement after matching otherwise the game will 
					   // go down to the next case
			case EquippedWeapon.Bow:
				if (Ammo > 0 && _readyToShoot) // only shoot if there's ammo
				{
					PlayBowStringAudio();
					await ToSignal(_playerSprite, PlayerAnimations.SignalName.BowReleased); // creates arrow at right animation frame
					_arrowInstance = _arrowPreload.Instantiate(); // creates a new arrow in-game
					AddSibling(_arrowInstance, true);
					Ammo--;
					_readyToShoot = false;
				}
				break;
			// more weapons? magic staff?
		}
	}
	
	public override void _Process(double delta)
	{
		// Get movement
		GetMovement();
		
		// Get state
		GetState(); // also attacks
		
		// Set equipped weapon
		EquipWeapon();
		
		// Animation logic
		FlipSprite();
		FlipArrowSpawn();
		AnimateBow();

		if (_state == State.Attacking) AnimateAttack();
		else AnimateMovement();
		
		// Audio logic
		
		
		// Move player
		MoveAndSlide();
		// MoveAndCollide(Velocity * (float)delta); // causes the player to freeze when going into a wall
	}
	
	// EVERYTHING AFTER THIS LINE IS TEMPORARY WHILE WAITING FOR FUTURE WORK
	public int GetPlayerWeapon()
	{
		return (int)_weapon; // returns the private _weapon variable as an int in a public manner
		// the reason why is because the enum itself is private and we don't need to pass it to anyone else
		
		// all enums are basically ints so the enumeration above is just shorthand for this:
		// enum EquippedWeapon
		//{
		//     Sword = 0,
		//     Bow = 1,
		//}
		// BUT you can also set whatever value you want like if you are explicit:
		// enum EquippedWeapon
		//{
		//     Sword = 9,
		//     Bow = 123,
		//     MagicStaff = 0,
		//}
	} 
}
