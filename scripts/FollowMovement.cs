using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class FollowMovement : Node
{
    [Export] public int _speed = 50;
    [Export] public float _followTime = 2.0f;
    [Export] public float _waitTime = 2.0f;

    public Player _target;
    private CharacterBody2D _parent;
    private Vector2 _startPosition;
    private Area2D _detectionArea;
    private bool _targetInArea;
    private int _overshootLimit;
    private Vector2 _directionToStart;

    public enum State
    {
        Idle,
        Follow,
        Search,
        Return
    }

    public State _currentState = State.Idle;

    public override void _Ready()
    {
        _targetInArea = false;
        _parent = GetParent<CharacterBody2D>();
        _detectionArea = GetNode<Area2D>("../DetectionArea");
        _startPosition = _parent.GlobalPosition;
        _overshootLimit = 2;
        _directionToStart = _startPosition - _parent.GlobalPosition;

        _detectionArea.BodyEntered += OnBodyEntered;
        _detectionArea.BodyExited += OnBodyExited;
    }
    
    private void OnBodyEntered(Node2D body)
    {
        _targetInArea = true;
        _target = GetNode<Player>("/root/Game/Player");
        _currentState = State.Follow;
    }

    private async void OnBodyExited(Node2D body)
    {
        _targetInArea = false;
        await ToSignal(GetTree().CreateTimer(_followTime), SceneTreeTimer.SignalName.Timeout);
        if (!_targetInArea) _target = null;
    }

    public async void Movement()
    {
        switch (_currentState)
        {
            case State.Idle:
                    _parent.Velocity = Vector2.Zero;
                break;
            case State.Follow:
                
                // if there is no target then return to start position
                if (_target == null)
                {
                    _currentState = State.Search;
                    return;
                }

                // calculate direction to target
                Vector2 direction = (_target.GlobalPosition - _parent.GlobalPosition).Normalized();
                
                // move towards target
                _parent.Velocity = direction * _speed;

                break;
            case State.Search:
                _parent.Velocity = Vector2.Zero;
                await ToSignal(GetTree().CreateTimer(_waitTime), SceneTreeTimer.SignalName.Timeout);
                _currentState = State.Return;
                break;
            case State.Return:
                
                // calculate direction to start
                _directionToStart = _startPosition - _parent.GlobalPosition;
                
                // stops the enemy once it reaches the start position
                if (_directionToStart.Length() < _overshootLimit)
                {
                    _parent.GlobalPosition = _startPosition;
                    _currentState = State.Idle;
                    return;
                }

                // move toward start
                _parent.Velocity = _directionToStart.Normalized() * _speed;
                break;
        }
    }

    public override void _Process(double delta)
    {
        Movement();
        _parent.MoveAndSlide();
    }

}
