using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Get Sprite
	private AnimatedSprite2D _playerSprite;
	
	// Camera screensize
	public Vector2 ScreenSize { get; set; }
	
	// Speed attributes
	[ExportGroup("Attributes")]
	[Export] public int Speed { get; set; } = 200;
	// Player gameplay attributes
	[Export] public int Lives { get; set; } = 3;
	[Export] public int Armor { get; set; } = 0;
	[Export] public int Level { get; set; } = 0;
	[Export] public int Ammo { get; set; } = 10;

	[ExportGroup("References")]
	[Export] private CollisionShape2D _collisionBox;

	// Methods
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		_playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _Process(double delta)
	{
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
