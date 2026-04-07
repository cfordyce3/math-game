using Godot;
using System;

public partial class Arrow : Area2D
{
	[Export] private float _speed = 200;
	public int flip;
	
	private VisibleOnScreenNotifier2D _visibleOnScreenNotifier2D;
	private Sprite2D _arrowSprite;
	
	public override void _Ready()
	{
		Player player = GetParent() as Player;
		flip = player.ShootDirection;
		BodyEntered += OnBodyEnteredSignal;
		
		_visibleOnScreenNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		_visibleOnScreenNotifier2D.ScreenExited += OnScreenExitedSignal;
		
		_arrowSprite = GetNode<Sprite2D>("Sprite2D");
		GetNode<AudioStreamPlayer>("ShootSound").Play(); // you're welcome lol
	}

	// When arrow exits the screen
	private void OnScreenExitedSignal()
	{
		QueueFree();
	}

	private void OnBodyEnteredSignal(Node body)
	{
		// TODO: if hitting enemy logic here
	}

	public override void _Process(double delta)
	{
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
