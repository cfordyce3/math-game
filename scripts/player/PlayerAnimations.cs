using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerAnimations : AnimatedSprite2D
{
	private Player _player;
	private ShapeCast2D _swordSwingHitbox;
	
	private Node _sceneTree;
	
	private List<Object> _enemiesList = [];
	
	[Signal] public delegate void GetAnimationDetailsEventHandler(string animationName, int frameIndex);
	[Signal] public delegate void AnimationFinishedWithNameEventHandler(string animationName);
	[Signal] public delegate void BowReleasedEventHandler();

	public override void _EnterTree()
	{
		_sceneTree = GetTree().Root.GetNode("Game");
		var nodeCount = _sceneTree.GetChildren();
		// find every orc or skeleton and add it to the enemies list
		foreach (var node in  nodeCount)
		{
			if (node.Name.ToString().Contains("Skeleton") || node.Name.ToString().Contains("Orc"))
			{
				_enemiesList.Add(node);
			}
		}
	}

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

	private bool _hitAgain = true;
	public void HandleSwordSwing()
	{
		// Track whether player is flipped or not
		bool flipped = _player.Flipped;
			
		// Sets root (rotation point) position
		_swordSwingHitbox.Position = (flipped) ? new Vector2(-3, 1) : new Vector2(3, 1);
			
		// Set rotation based on animation frame
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
			case 4: // frame 4
				_swordSwingHitbox.RotationDegrees = (flipped) ? 155 : -160;
				break;
		}

		if (_swordSwingHitbox.IsColliding())
		{
			var collidedWith = _swordSwingHitbox.GetCollider(0);
			
			if (collidedWith != null && _enemiesList.Contains(collidedWith) && _hitAgain)
			{
				//_enemiesList.Remove(collidedWith); // remove from the list of enemies generated at _EnterTree
				_hitAgain = false;
				collidedWith.Call(Enemy.MethodName.OnHit); // calls the QueueFree function on found enemy
			}
		}
	}

	public override void _Process(double delta)
	{
		// Send out animation details every frame
		EmitSignal(SignalName.GetAnimationDetails, Animation, Frame);
		
		// When bow is ready to release
		if (Animation == "bow_attack" && Frame == 3) EmitSignal(SignalName.BowReleased);
		
		// When swinging sword
		if (Animation == "sword_attack") HandleSwordSwing();
		else
		{
			_hitAgain = true;
			_swordSwingHitbox.Enabled = false; // if not attacking, no sword hitbox
		}

	}
}
