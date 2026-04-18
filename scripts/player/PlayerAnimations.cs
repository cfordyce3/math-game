using Godot;
using System;

public partial class PlayerAnimations : AnimatedSprite2D
{
	[Signal] public delegate void GetAnimationDetailsEventHandler(string animationName, int frameIndex);
	[Signal] public delegate void AnimationFinishedWithNameEventHandler(string animationName);
	[Signal] public delegate void BowReleasedEventHandler();
	public override void _Ready()
	{
		AnimationFinishedWithName += OnAnimationFinishedWithNameSignal;
		AnimationFinished += OnAnimationFinishedSignal;
	}

	private void OnAnimationFinishedWithNameSignal(string animationName)
	{
		
	}
	public void OnAnimationFinishedSignal()
	{
		EmitSignal(SignalName.AnimationFinishedWithName, Animation);
	}

	public override void _Process(double delta)
	{
		// Send out animation details every frame
		EmitSignal(SignalName.GetAnimationDetails, Animation, Frame);
		
		// When bow is ready to release
		if (Animation == "bow_attack" && Frame == 3) EmitSignal(SignalName.BowReleased);

	}
}
