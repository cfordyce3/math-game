using Godot;
using System;

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
	private AnimatedSprite2D _playerSprite;
	private CollisionShape2D _playerCollisionBox;
	
	// Camera ScreenSize
	public Vector2 ScreenSize { get; set; }
	
	// Private attributes
	[ExportGroup("Attributes")]
	[Export] private State _state = State.Idle; // stateful player
	[Export] private int _speed = 100;
	[Export] private int _level = 0;
	[Export] private EquippedWeapon _weapon = EquippedWeapon.Sword; // defaults to sword
	[Export] private int _ammo  = 100;
	
	// Attack delay timer
	private int _stateCounter = 0;
	
	// Public attributes
	[Export] public int Lives = 3;
	[Export] public int Armor = 0;

	// Velocity and direction
	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _inputDirection = Vector2.Zero;
	private bool _flipped = false;

	// Load arrow scene for shooting
	private PackedScene _arrowPreload;
	public int ShootDirection;

	// Methods
	
	// On Ready (one shot)
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		_playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // gets AnimatedSprite child node
		_arrowPreload = GD.Load<PackedScene>("res://scenes/arrow.tscn"); // load arrow scene to be spawned (instantiated)
		_playerSprite.AnimationFinished += AttackFinished;
	} 
	

	// Get input direction / Velocity
	private void GetMovement()
	{
		_inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = _inputDirection * _speed;
	}

	// Movement animation
	private void AnimateMovement()
	{
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

	// Attack animation
	private void AnimateAttack()
	{
		if (_weapon == EquippedWeapon.Sword && !_playerSprite.IsPlaying()) _playerSprite.Play("sword_attack");
		// bow animation here
	}
	
	private void AttackFinished()
	{
		if (_state == State.Attacking) _state = State.Idle;
	}

	// Flips sprite regardless of state
	private void FlipSprite()
	{
		if (_inputDirection.X > 0)
		{
			_flipped = false;
			_playerSprite.FlipH = false;
		}
		if (_inputDirection.X < 0)
		{
			_flipped = true;
			_playerSprite.FlipH = true;
		}
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
			GD.Print("sword equipped");
		}
		else if (Input.IsActionJustPressed("equip_bow") && _weapon != EquippedWeapon.Bow)
		{
			_weapon = EquippedWeapon.Bow;
			GD.Print("bow equipped");
		}
	}

	public void Attack()
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
				if (_ammo > 0) // only shoot if there's ammo
				{
					Node _arrowInstance = _arrowPreload.Instantiate(); // creates a new arrow in-game
					AddChild(_arrowInstance);
					_ammo--;
					GD.Print("ammo remaining: " + _ammo);
				}
				else
				{
					_weapon = EquippedWeapon.Sword; // swap to sword when out of ammo
					GD.Print("out of ammo"); // eventually UI alert to player that they're out of ammo
				}
				break;
		}
	}
	
	public override void _Process(double delta)
	{
		// Get state
		GetState(); // also attacks
		
		// Set equipped weapon
		EquipWeapon();
		
		// Get movement
		GetMovement();
		
		// Animation logic
		FlipSprite();
		
		if (_state == State.Attacking)
		{
			AnimateAttack();
		}
		else AnimateMovement();
		
		// Move player
		MoveAndSlide();
	}
}
