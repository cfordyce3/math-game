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
	private Node2D _arrowSpawnLocation;
	
	// Camera ScreenSize
	public Vector2 ScreenSize;

	[ExportGroup("Debugging")] // variables for testing
	[Export] private bool _moveOnAttack = true;
	
	// Private attributes
	[ExportGroup("Attributes")]
	[Export] private State _state = State.Idle; // stateful player
	[Export] private int _speed = 100;
	[Export] private EquippedWeapon _weapon = EquippedWeapon.Sword; // defaults to sword
	
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

	// Load arrow scene for shooting
	private PackedScene _arrowPreload;
	public int ShootDirection;
	private Node _arrowInstance;

	// Methods
	
	// On Ready (one shot)
	public override void _Ready()
	{
		// Set variables
		ScreenSize = GetViewportRect().Size;
		_playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // gets AnimatedSprite child node
		_arrowPreload = GD.Load<PackedScene>("res://scenes/arrow.tscn"); // load arrow scene to be spawned (instantiated)
		_arrowSpawnLocation = GetNode<Node2D>("ArrowSpawnLocation"); // loads arrow spawn point
		
		// Signals
		_playerSprite.AnimationFinished += AnimationFinished;
	} 
	

	// Get input direction / Velocity
	private void GetMovement()
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
		if (_weapon == EquippedWeapon.Bow) _state = State.Idle; // temporary until bow animation is finished
	}
	
	private void AnimationFinished()
	{
		if (_state == State.Attacking) _state = State.Idle;
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
			}
			if (_inputDirection.X < 0)
			{
				_flipped = true;
				_playerSprite.FlipH = true;
			}
			
		}
	}

	// Flips the arrow spawn
	private void FlipArrowSpawn()
	{
		Vector2 _pos = _arrowSpawnLocation.Position;
		if (!_flipped) // facing right
		{
			_pos = new Vector2(6, 0);
		}
		else // facing left
		{
			_pos = new Vector2(-6, 0);
		}
		_arrowSpawnLocation.Position = _pos;
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
		if (Input.IsActionJustPressed("equip_sword") && _weapon != EquippedWeapon.Sword) _weapon = EquippedWeapon.Sword;
		else if (Input.IsActionJustPressed("equip_bow") && _weapon != EquippedWeapon.Bow) _weapon = EquippedWeapon.Bow;
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
				if (Ammo > 0) // only shoot if there's ammo
				{
					_arrowInstance = _arrowPreload.Instantiate(); // creates a new arrow in-game
					AddSibling(_arrowInstance);
					Ammo--;
				}
				else
				{
					_weapon = EquippedWeapon.Sword; // swap to sword when out of ammo
				}
				break;
			// more weapons? magic staff?
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
		FlipArrowSpawn();
		
		if (_state == State.Attacking) AnimateAttack();
		else AnimateMovement();
		
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
