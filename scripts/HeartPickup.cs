using Godot;
using System;

public partial class HeartPickup : Area2D
{
	// Child nodes
	private AudioStreamPlayer _pickupSound;
	private CollisionShape2D _heartHitbox;
	
	[Signal] public delegate void PickedUpEventHandler();
	
	public override void _Ready()
	{
		_pickupSound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		_heartHitbox = GetNode<CollisionShape2D>("CollisionShape2D");

		BodyEntered += OnBodyEnteredSignal;
	}

	private async void OnBodyEnteredSignal(Node2D body)
	{
		if (body is Player player)
		{
			_heartHitbox.SetDeferred("disabled", true);
			EmitSignal(SignalName.PickedUp);
			_pickupSound.Play();
			player.Lives++;
			Hide();
			await ToSignal(_pickupSound, "finished");
			QueueFree();
		}
	}

	public override void _Process(double delta)
	{
	}

}
