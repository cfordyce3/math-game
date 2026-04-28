using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class FollowMovement : Node
{
   [Export] public int _speed = 50;
   [Export] public int _followTime = 2;
   
   public Player _target;
   private CharacterBody2D _parent;
   private Vector2 _startPosition;
   private Area2D _detectionArea;
   private bool _targetInArea;

    public override void _Ready()
    {
        _targetInArea = false;
        _parent = GetNode<CharacterBody2D>("/root/Game/Orc");
        _detectionArea = GetNode<Area2D>("/root/Game/Orc/DetectionArea");
        _startPosition = _parent.Position;

        _detectionArea.BodyEntered += OnBodyEntered;
        _detectionArea.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        _targetInArea = true;
        _target = GetNode<Player>("/root/Game/Player");
    }

    private async void OnBodyExited(Node2D body)
    {
        _targetInArea = false;
        await ToSignal(GetTree().CreateTimer(_followTime), SceneTreeTimer.SignalName.Timeout);
        if (!_targetInArea) _target = null;
    }

    // public void Detection()
    // {
    //     if (_detectionArea.())
    // }

    public async void Movement()
    {
        // calculate direction to target
        Vector2 direction = (_target.GlobalPosition - _parent.GlobalPosition).Normalized();
        
        // move towards target
        _parent.Velocity = direction * _speed;
    }

    public override void _Process(double delta)
    {
        if (_target == null) return;
        
        Movement();
        _parent.MoveAndSlide();
    }

}
