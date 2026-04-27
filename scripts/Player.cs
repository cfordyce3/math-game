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
	
	// Player direction
	public enum PlayerDirection
	{
		Up,
		Down,
		Left,
		Right
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
	[Export] private bool _moveOnAttack = false;
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
	[Export] public int Ammo = 10;
	[Export] public int Level = 0;
	[Export] public int Armor = 0;
	[Export] public bool Flipped = false;
	public PlayerDirection Direction = PlayerDirection.Right;

	// Velocity and direction
	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _inputDirection = Vector2.Zero;

	// Audio variables
	private AudioStreamWav _swordSwingSound; // sword swinging sound
	private AudioStreamWav _bowStringSound; // bow string sound
	
	// Arrow variables
	private PackedScene _arrowPreload;
	private Node _arrowInstance;
	public int ShootDirectionLR;
	public int ShootDirectionUD;

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
		
		if (Velocity.X > 0) Direction =  PlayerDirection.Right;
		if (Velocity.X < 0) Direction =  PlayerDirection.Left;
		if (Velocity.Y < 0) Direction =  PlayerDirection.Up;
		if (Velocity.Y > 0) Direction =  PlayerDirection.Down;
	}

	// Movement animation
	private void AnimateMovement()
	{
		_bowSprite.Visible = true;
		if (_inputDirection.X > 0) // go right
		{
			if (_weapon == EquippedWeapon.Bow) _bowSprite.Animation = "equipped";
			if (_inputDirection.Y < 0) _playerSprite.Play("run_up"); // up right
			else if (_inputDirection.Y > 0) _playerSprite.Play("run_down"); // down right
			else _playerSprite.Play("run_side"); // just right
		}

		if (_inputDirection.X < 0) // go left
		{
			if (_weapon == EquippedWeapon.Bow) _bowSprite.Animation = "equipped";
			else _bowSprite.Animation = "unequipped";
			if (_inputDirection.Y < 0) _playerSprite.Play("run_up"); // up left
			else if (_inputDirection.Y > 0) _playerSprite.Play("run_down"); // down left
			else _playerSprite.Play("run_side"); // just left
		}

		if (_inputDirection.Y < 0)
		{
			if (_weapon == EquippedWeapon.Bow) _bowSprite.Animation = "equipped";
			else _bowSprite.Animation = "unequipped";
			_playerSprite.Play("run_up");
		} // just up

		if (_inputDirection.Y > 0)
		{
			if (_weapon == EquippedWeapon.Bow) _bowSprite.Animation = "equipped_down";
			else _bowSprite.Animation = "unequipped";
			_playerSprite.Play("run_down");
		} // just down

		if (_inputDirection.Length() == 0 && _state == State.Idle)
		{
			if (Direction != PlayerDirection.Down)
			{
				if (_weapon == EquippedWeapon.Bow) _bowSprite.Animation = "equipped";
				else  _bowSprite.Animation = "unequipped";
			}
			// if (Direction != PlayerDirection.Up) _playerSprite.Play("idle");
			// else _playerSprite.Play("idle_up");

// changed this to a switch case since there are now 3 idle animations/states
			switch (Direction)
			{
				case PlayerDirection.Up:
					_playerSprite.Play("idle_up");
				break;

				case PlayerDirection.Down:
					_playerSprite.Play("idle_down");
				break;

				default:
					_playerSprite.Play("idle");
				break;
			}
		} // no movement

	}

	private void AnimateBow()
	{
		if (_weapon == EquippedWeapon.Bow)
		{
			if (Direction == PlayerDirection.Up) _bowSprite.ZIndex = -1;
			else _bowSprite.ZIndex = 1;
			
		}
		if (_weapon != EquippedWeapon.Bow)
		{
			if (Direction == PlayerDirection.Up) _bowSprite.ZIndex = 1;
			else _bowSprite.ZIndex = -1;
		}
	}

	// Attack animation
	private async void AnimateAttack()
	{
		// attack animations here

// refactored a bit since the sword has multiple animations now, feel free to change as you see fit
		switch (Direction)
		{
			// up animations
			case PlayerDirection.Up:
				// sword animation
				if (_weapon == EquippedWeapon.Sword && !_playerSprite.IsPlaying())
				{
					PlaySwordSwingAudio();
					_playerSprite.Play("sword_attack_up");
					await ToSignal(_playerSprite, PlayerAnimations.SignalName.AnimationFinished);
				}
				// bow animation
				if (_weapon == EquippedWeapon.Bow && !_playerSprite.IsPlaying()) 
				{
					_playerSprite.Play("bow_attack_up");
					_bowSprite.Visible = false;
					await ToSignal(_playerSprite, PlayerAnimations.SignalName.AnimationFinished);
					_bowSprite.Visible = true;
					//_bowSprite.Play("shoot_up");
				}
			break;
			// down animations
			case PlayerDirection.Down:
				// sword animation
				if (_weapon == EquippedWeapon.Sword && !_playerSprite.IsPlaying())
				{
					PlaySwordSwingAudio();
					_playerSprite.Play("sword_attack_down");
					await ToSignal(_playerSprite, PlayerAnimations.SignalName.AnimationFinished);
				}
				// bow animation
				if (_weapon == EquippedWeapon.Bow && !_playerSprite.IsPlaying())
				{
					_playerSprite.Play("bow_attack_down");
					_bowSprite.Play("shoot_down");
					await ToSignal(_bowSprite, PlayerAnimations.SignalName.AnimationFinished);
					_bowSprite.Animation = "equipped_down";
				}
			break;
			// side animations
			default:
				// sword animation
				if (_weapon == EquippedWeapon.Sword && !_playerSprite.IsPlaying())
				{
					PlaySwordSwingAudio();
					_playerSprite.Play("sword_attack_side");
				}
				// bow animation
				if (_weapon == EquippedWeapon.Bow && !_playerSprite.IsPlaying())
				{
					_playerSprite.Play("bow_attack_side");
					_bowSprite.Play("shoot_side");
				}
			break;
		}
					
			//_playerSprite.Play("bow_attack");
			//_bowSprite.Play("shoot_side");
			// _bowSprite.Visible = false; 
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

		if (animationName.Contains("bow_attack"))
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
				Flipped = false;
				_playerSprite.FlipH = false;
				_bowSprite.FlipH = false;
			}
			if (_inputDirection.X < 0)
			{
				Flipped = true;
				_playerSprite.FlipH = true;
				_bowSprite.FlipH = true;
			}
			
		}
	}

	// Determines the arrow spawn
	private Vector2 _arrowSpawnPosition;
	private void SetArrowSpawn()
	{
		switch (Direction) // spawn based on direction
		{
			case PlayerDirection.Up:
				_arrowSpawnPosition = (Flipped) ? new Vector2(-2, -7) : new Vector2(-1, -7);
				break;
			case PlayerDirection.Down:
				_arrowSpawnPosition = (Flipped) ? new Vector2(1, 4) : new Vector2(2, 4);
				break;
			case PlayerDirection.Left:
				_arrowSpawnPosition = new Vector2(-5, 1);
				break;
			case PlayerDirection.Right:
				_arrowSpawnPosition = new Vector2(5, 1);
				break;
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
		if (!_playerSprite.Animation.ToString().Contains("sword_attack") && !_playerSprite.Animation.ToString().Contains("bow_attack"))
		{
			if (Input.IsActionJustPressed("equip_sword") && _weapon != EquippedWeapon.Sword) 
			{
				_weapon = EquippedWeapon.Sword;
				_bowSprite.Play("unequipped");
			}
			else if (Input.IsActionJustPressed("equip_bow") && _weapon != EquippedWeapon.Bow) 
			{
				_weapon = EquippedWeapon.Bow;
				if (Direction == PlayerDirection.Down) _bowSprite.Play("equipped_down");
				else _bowSprite.Play("equipped");
			}
		}
	}

	public async void Attack()
	{
		ShootDirectionLR = (Flipped) ? -1 : 1; // -1 for left, 1 for right
		if (Direction == PlayerDirection.Up) ShootDirectionUD = -1; // -1 for up
		else if (Direction == PlayerDirection.Down) ShootDirectionUD = 1; // 1 for down
		else ShootDirectionUD = 0; // 0 for left or right
		switch (_weapon)
		{
			case EquippedWeapon.Sword:
				// sword behavior handled in PlayerAnimations.cs
				break; 
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

	private void KilledEnemy()
	{
		if (Ammo < 10) Ammo++;
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
		SetArrowSpawn();
		AnimateBow();

		if (_state == State.Attacking) AnimateAttack();
		else AnimateMovement();
		
		// Audio logic
		
		
		// Move player
		MoveAndSlide();
		// MoveAndCollide(Velocity * (float)delta); // causes the player to freeze when going into a wall
	}
}
