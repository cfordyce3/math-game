using Godot;
using System;

public partial class HeartPickup : Area2D
{
	// Child nodes
	private AudioStreamPlayer _pickupSound;
	
	public override void _Ready()
	{
		_pickupSound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		
		BodyEntered += OnBodyEnteredSignal;
	}

	private async void OnBodyEnteredSignal(Node2D body)
	{
		if (body is Player player)
		{
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
