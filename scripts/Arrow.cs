using Godot;
using System;
using System.Collections.Generic;

public partial class Arrow : Area2D
{
	[Export] private float _speed = 200;
	public int flip;
	public int vflip;
	private bool _moving = false;
	private bool _broken = false;
	
	private List<Object> _enemiesList = [];
	
	private VisibleOnScreenNotifier2D _visibleOnScreenNotifier2D;
	private AnimatedSprite2D _arrowSprite;

	private AnimatedSprite2D _arrowSpawnTimeFromAnimation;
	private Node2D _arrowSpawnLocation;

	private Timer _shootSoundTimer;
	
	// Custom signals
	[Signal] public delegate void ObjectHitEventHandler(Node body);

	public override void _EnterTree()
	{
		// get a list of enemies to match when hitting entity
		foreach (var node in GetTree().Root.GetNode("Game").GetChildren())
		{
			if (node.Name.ToString().Contains("Skeleton") || node.Name.ToString().Contains("Orc"))
			{
				_enemiesList.Add(node);
			}
		}
		
		_arrowSpawnTimeFromAnimation = GetNode<AnimatedSprite2D>("../Player/AnimatedSprite2D");
		_arrowSpawnLocation = GetNode<Node2D>("../Player/ArrowSpawnLocation");
		_shootSoundTimer = GetNode<Timer>("ShootSoundTimer");
	}
	
	public override void _Ready()
	{
		Player _player = GetNode<Player>("../Player");
		
		GlobalPosition = _arrowSpawnLocation.GlobalPosition;
		
		// Flip arrow if player is flipped
		flip = _player.ShootDirectionLR;
		vflip = _player.ShootDirectionUD;
		
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
		QueueFree(); // remove arrow
	}

	// Play arrow break animation
	private async void ArrowBreak()
	{
		_broken = true;
		_arrowSprite.Play("arrowBroken");
		await ToSignal(GetTree().CreateTimer(0.25), SceneTreeTimer.SignalName.Timeout);
		QueueFree();
	}

	private void OnBodyEnteredSignal(Node body)
	{
		_moving = false; // stop moving arrow after hitting something
		switch (body)
		{
			case TileMapLayer: // if hit an environment object
				ArrowBreak();
				break;
			case Enemy: // if hit an enemy
				body.Call(Enemy.MethodName.OnHit, 2);
				QueueFree();
				break;
		}
	}

	private void MoveArrowHorizontal(float delta)
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

	private void MoveArrowVertical(float delta)
	{
		Vector2 pos = GlobalPosition;
		_arrowSprite.FlipH = (flip == -1);
		if (vflip  == -1)
		{
			pos.Y -= _speed * delta;
			RotationDegrees = -90;
		}
		else
		{
			RotationDegrees = 90;
			pos.Y += _speed * delta;
		}
		GlobalPosition = pos;
	}

	// bow_attack frame=3
	public override void _Process(double delta)
	{
		if (_moving && vflip == 0) MoveArrowHorizontal((float)delta);
		else if (_moving && vflip != 0) MoveArrowVertical((float)delta);
		else if (_arrowSpawnTimeFromAnimation.Frame == 3)
		{
			Show();
			// Checks if arrow is colliding before frame 3
			if (!_broken) _moving = true;
		}
	}
}
