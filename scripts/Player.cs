using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Get Player Child Nodes
	private AnimatedSprite2D _playerSprite;
	private CollisionShape2D _playerCollisionBox;
	
	// Heart Node
	private Area2D _heartPickup;
	
	// Camera ScreenSize
	public Vector2 ScreenSize { get; set; }
	
	// Player Attributes
	[ExportGroup("Attributes")]
	[Export] public int Speed { get; set; } = 200;
	[Export] public int Lives { get; set; } = 3;
	[Export] public int Armor { get; set; } = 0;
	[Export] public int Level { get; set; } = 0;
	[Export] public int Ammo { get; set; } = 10;
	

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
		Lives++;
		GD.Print("got pickup, lives: " + Lives);
	}

	public override void _Process(double delta)
	{
		
		// Movement logic
		Vector2 velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
			_playerSprite.FlipH = true;
			_playerSprite.Play("idle");
		}
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
			_playerSprite.FlipH = false;
			_playerSprite.Play("idle");
		}
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
			_playerSprite.Play("going_up");
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
			_playerSprite.Play("idle");
		}

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
		}
		else 
		{
			_playerSprite.Play("idle");
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);
	}
}
