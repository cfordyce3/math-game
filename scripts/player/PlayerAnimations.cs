using Godot;
using System;

public partial class PlayerAnimations : AnimatedSprite2D
{
	private Player _player;
	private ShapeCast2D _swordSwingHitbox;
	
	[Signal] public delegate void GetAnimationDetailsEventHandler(string animationName, int frameIndex);
	[Signal] public delegate void AnimationFinishedWithNameEventHandler(string animationName);
	[Signal] public delegate void BowReleasedEventHandler();
	public override void _Ready()
	{
		_player = GetParent<Player>();
		_swordSwingHitbox = GetNode<ShapeCast2D>("SwordSwingHitbox");
		
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
		
		if (Animation == "sword_attack")
		{
			bool flipped = _player.GetPlayerFlipped();
			_swordSwingHitbox.Position = (flipped) ? new Vector2(-3, 1) : new Vector2(3, 1); // sets base position
			switch (Frame)
			{
				case 0:
				case 1:
					break;
				case 2: // frame 2
					_swordSwingHitbox.Enabled = true; // enables hitbox on first frame
					_swordSwingHitbox.RotationDegrees = (flipped) ? 55 : -60;
					break;
				case 3: // frame 3
					_swordSwingHitbox.RotationDegrees = (flipped) ? 130 : -130;
					break;
				case 4: // framr 4
					_swordSwingHitbox.RotationDegrees = (flipped) ? 155 : -160;
					break;
			}
		}
		else _swordSwingHitbox.Enabled = false; // if not attacking, no hitbox

	}
}
