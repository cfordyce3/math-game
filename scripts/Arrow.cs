using Godot;
using System;

public partial class Arrow : Area2D
{
	[Export] private float _speed = 200;
	public int flip;
	
	private VisibleOnScreenNotifier2D _visibleOnScreenNotifier2D;
	private Sprite2D _arrowSprite;

	private AnimatedSprite2D _arrowSpawnTimeFromAnimation;
	private Node2D _arrowSpawnLocation;

	public override void _EnterTree()
	{
		Hide(); // wait for correct animation frame
		_arrowSpawnTimeFromAnimation = GetNode<AnimatedSprite2D>("../Player/AnimatedSprite2D");
		_arrowSpawnLocation = GetNode<Node2D>("../Player/ArrowSpawnLocation");
	}
	
	public override void _Ready()
	{
		Player _player = GetNode<Player>("../Player");
		
		// Flip arrow if player is flipped
		flip = _player.ShootDirection;
		
		// If arrow collides with something
		BodyEntered += OnBodyEnteredSignal;
		
		// If arrow leaves screen
		_visibleOnScreenNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		_visibleOnScreenNotifier2D.ScreenExited += OnScreenExitedSignal;
		
		// Get resources and play shoot sound
		_arrowSprite = GetNode<Sprite2D>("Sprite2D");
		GetNode<AudioStreamPlayer>("ShootSound").Play();
	}

	// When arrow exits the screen
	private void OnScreenExitedSignal()
	{
		QueueFree();
	}

	private void OnBodyEnteredSignal(Node body)
	{
		// if hitting enemy logic here
	}

	// bow_attack frame=3
	public override void _Process(double delta)
	{
		if (_arrowSpawnTimeFromAnimation.Animation == "bow_attack" && _arrowSpawnTimeFromAnimation.Frame == 3)
		{
			Show();
			GlobalPosition = _arrowSpawnLocation.GlobalPosition;
		}
		var pos = GlobalPosition;
		if (flip == -1)
		{
			pos.X -= _speed * (float)delta;
			_arrowSprite.FlipH = true;
		}
		else pos.X += _speed * (float)delta;
		GlobalPosition = pos;
	}
}
