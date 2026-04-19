using Godot;
using System;
using System.Collections;

public partial class Arrow : Area2D
{
	[Export] private float _speed = 200;
	public int flip;
	private bool _moving = false;
	private bool _broken = false;
	
	private VisibleOnScreenNotifier2D _visibleOnScreenNotifier2D;
	private AnimatedSprite2D _arrowSprite;

	private AnimatedSprite2D _arrowSpawnTimeFromAnimation;
	private Node2D _arrowSpawnLocation;

	private Timer _shootSoundTimer;
	
	// Custom signals
	[Signal] public delegate void ObjectHitEventHandler(Node body);

	public override void _EnterTree()
	{
		// Hide(); // wait for correct animation frame
		_arrowSpawnTimeFromAnimation = GetNode<AnimatedSprite2D>("/root/Game/Player/AnimatedSprite2D");
		_arrowSpawnLocation = GetNode<Node2D>("/root/Game/Player/ArrowSpawnLocation");
		_shootSoundTimer = GetNode<Timer>("ShootSoundTimer");
	}
	
	public override void _Ready()
	{
		Player _player = GetNode<Player>("../Player");
		
		GlobalPosition = _arrowSpawnLocation.GlobalPosition;
		
		// Flip arrow if player is flipped
		flip = _player.ShootDirection;
		
		// If arrow collides with a body (Node)
		BodyEntered += OnBodyEnteredSignal;
		
		// If arrow leaves screen
		_visibleOnScreenNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		_visibleOnScreenNotifier2D.ScreenExited += OnScreenExitedSignal;
		
		// Get resources and play shoot sound
		_arrowSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_shootSoundTimer.Timeout += PlayShootSound;
		_shootSoundTimer.Start();
	}

	private void PlayShootSound()
	{
		GetNode<AudioStreamPlayer>("ShootSound").Play();
	}

	// When arrow exits the screen
	private void OnScreenExitedSignal()
	{
		QueueFree();
	}

	// Play arrow break animation
	private async void ArrowBreak()
	{
		_broken = true;
		_arrowSprite.Play("arrowBroken");
		GD.Print("Arrow Broke");
		await ToSignal(GetTree().CreateTimer(0.25), SceneTreeTimer.SignalName.Timeout);
		QueueFree();
	}

	private void OnBodyEnteredSignal(Node body)
	{
		// if hitting enemy logic here
		_moving = false;
		GD.Print(body.GetClass());
		EmitSignal(SignalName.ObjectHit, body);
		switch (body)
		{
			case TileMapLayer:
				// other logic for hitting tilemap
				ArrowBreak();
				break;
			case Enemy:
				// other logic for hitting enemies
				QueueFree();
				break;
			// other cases for other things it can hit
		}
	}

	private void MoveArrow(float delta)
	{
		Vector2 pos = GlobalPosition;
		if (flip == -1)
		{
			pos.X -= _speed * delta;
			_arrowSprite.FlipH = true;
		}
		else pos.X += _speed * delta;
		GlobalPosition = pos;
	}

	// bow_attack frame=3
	public override void _Process(double delta)
	{
		if (_moving) MoveArrow((float)delta);
		else if (_arrowSpawnTimeFromAnimation.Frame == 3)
		{
			Show();
			// Checks if arrow is colliding before frame 3
			if (!_broken) _moving = true;
		}
	}
}
