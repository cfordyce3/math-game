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
	[Export] private int _speed = 200;
	[Export] private int _lives = 3;
	[Export] private int _armor = 0;
	[Export] private int _level = 0;
	[Export] private int _ammo  = 10;

	private Vector2 _velocity;

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

	public override void _Process(double delta)
	{
		// Movement logic
		_velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_left"))
		{
			_velocity.X -= 1;
			_playerSprite.FlipH = true;
		}
		if (Input.IsActionPressed("move_right"))
		{
			_velocity.X += 1;
			_playerSprite.FlipH = false;
		}
		if (Input.IsActionPressed("move_up"))
		{
			_velocity.Y -= 1;
		}
		if (Input.IsActionPressed("move_down"))
		{
			_velocity.Y += 1;
		}

		// Animate player
		if ((_velocity.X == 1) && (_velocity.Y == 0))
		{
			_playerSprite.Play("run_side");
		}
		if ((_velocity.X == -1) && (_velocity.Y == 0))
		{
			_playerSprite.Play("run_side");
		}
		if (_velocity.Y == 1)
		{
			_playerSprite.Play("run_down");
		}
		if (_velocity.Y == -1)
		{
			_playerSprite.Play("run_up");
		}

		if (_velocity.Length() > 0) _velocity = _velocity.Normalized() * _speed;
		else _playerSprite.Play("idle");

		Position += _velocity * (float)delta;

		// Clamp player to play area
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
	}
}
