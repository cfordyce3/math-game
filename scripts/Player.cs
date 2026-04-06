using Godot;
using System;

public partial class Player : CharacterBody2D

{
	// Get Player Child Nodes
	private AnimatedSprite2D _playerSprite;
	private CollisionShape2D _playerCollisionBox;
	
	// HeartPickup Node
	private Area2D _heartPickup;
	
	// Camera ScreenSize
	public Vector2 ScreenSize { get; set; }
	
	// Player Attributes
	[ExportGroup("Attributes")]
	[Export] private int _speed = 100;
	[Export] private int _lives = 3;
	[Export] private int _armor = 0;
	[Export] private int _level = 0;
	[Export] private int _ammo  = 10;

	private Vector2 _velocity = Vector2.Zero;
	private Vector2 _inputDirection = Vector2.Zero;

	// Methods
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		_playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // gets AnimatedSprite child node
		
		// Heart pickup
		_heartPickup = GetNode<Area2D>("HeartPickup");
		_heartPickup.BodyEntered += OnHeartPickupBodyEnteredSignal;
	}

	private void OnHeartPickupBodyEnteredSignal(Node2D body)
	{
		_lives++;
		GD.Print("got pickup, lives: " + _lives);
	}

	private Vector2 GetMovement()
	{
		_inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = _inputDirection * _speed;
		return _inputDirection;
	}

	private void AnimateMovement(Vector2 direction)
	{
		if (direction.X > 0) // go right
		{
			_playerSprite.FlipH = false;
			if (direction.Y < 0) _playerSprite.Play("run_up"); // up right
			else if (direction.Y > 0) _playerSprite.Play("run_down"); // down right
			else _playerSprite.Play("run_side");
		}

		if (direction.X < 0) // go left
		{
			_playerSprite.FlipH = true;
			if (direction.Y < 0) _playerSprite.Play("run_up"); // up left
			else if (direction.Y > 0) _playerSprite.Play("run_down"); // down left
			else _playerSprite.Play("run_side");
		}

		if (direction.Y < 0) { _playerSprite.Play("run_up"); } // go up

		if (direction.Y > 0) { _playerSprite.Play("run_down"); } // go down

		if (direction.Length() == 0) { _playerSprite.Play("idle"); } // no movement
	}

	public override void _Process(double delta)
	{
		// Movement logic
		//_velocity = Vector2.Zero;
		AnimateMovement(GetMovement());
		MoveAndSlide();
	}
}
