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
	[Export] private int _ammo  = 10;
	
	private int _stateCounter = 0;
	
	// Public attributes
	[Export] public int Lives = 3;
	[Export] public int Armor = 0;

	// Velocity and direction
	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _inputDirection = Vector2.Zero;

	// Methods
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		_playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // gets AnimatedSprite child node
	}

	private void GetMovement()
	{
		_inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = _inputDirection * _speed;
	}

	private void AnimateMovement()
	{
		if (_inputDirection.X > 0) // go right
		{
			_playerSprite.FlipH = false;
			if (_inputDirection.Y < 0) _playerSprite.Play("run_up"); // up right
			else if (_inputDirection.Y > 0) _playerSprite.Play("run_down"); // down right
			else _playerSprite.Play("run_side");
		}

		if (_inputDirection.X < 0) // go left
		{
			_playerSprite.FlipH = true;
			if (_inputDirection.Y < 0) _playerSprite.Play("run_up"); // up left
			else if (_inputDirection.Y > 0) _playerSprite.Play("run_down"); // down left
			else _playerSprite.Play("run_side");
		}

		if (_inputDirection.Y < 0) { _playerSprite.Play("run_up"); } // go up

		if (_inputDirection.Y > 0) { _playerSprite.Play("run_down"); } // go down

		if (_inputDirection.Length() == 0 && _state == State.Idle) { _playerSprite.Play("idle"); } // no movement
	}

	private void AnimateAttack()
	{
		
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("jump") && _stateCounter == 0)
		{
			_playerSprite.Stop();
			_state = State.Attacking;
			_stateCounter = 50;
		}
		else if (_state > 0)
		{
			_stateCounter--;
		}
		if (_state == State.Attacking && _stateCounter == 0)
		{
			_state = State.Idle;
		}
		
		// Get movement
		GetMovement();
		
		// Animation logic
		if (_state != State.Attacking) AnimateMovement();
		else AnimateAttack();
		
		// Move player
		MoveAndSlide();
	}
}
