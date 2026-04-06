using Godot;
using System;

public partial class HeartPickup : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEnteredSignal;
	}

	private void OnBodyEnteredSignal(Node2D body)
	{
		GD.Print("body entered");
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
