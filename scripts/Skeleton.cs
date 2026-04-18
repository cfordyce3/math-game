using Godot;
using System;
using System.Collections.Generic;

public partial class Skeleton : Enemy
{
    [Export] private int _deathSoundVolume = -4;
    
    private AudioStreamWav _skeletonDyingSound;
    private AudioStreamPlayer _skeletonSoundPlayer;
    private AnimatedSprite2D _skeletonSprite;

    public override void _Ready()
    {
        _skeletonDyingSound = GD.Load<AudioStreamWav>("res://assets/sounds/skeleton_dying.wav");
        _skeletonSoundPlayer = GetNode<AudioStreamPlayer>("SkeletonAudioStreamPlayer");
        _skeletonSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        
        GetTree().NodeAdded += OnAddArrowSignal;
        // GetTree().NodeRemoved += OnRemoveArrowSignal;
    }

    private void OnAddArrowSignal(Node node)
    {
        if (node.Name.ToString().Contains("Arrow"))
        {
            Area2D arrow = node as Area2D;
            arrow.TreeExiting += OnRemoveArrowSignal;
            arrow.BodyEntered += HitByArrow;
        }
    }

    private async void HitByArrow(Node node)
    {
        if (node.Name == Name)
        {
            _skeletonSoundPlayer.Stream = _skeletonDyingSound;
            _skeletonSoundPlayer.VolumeDb = _deathSoundVolume;
            _skeletonSoundPlayer.Play();
            _skeletonSprite.Play("die");
            // Hide();
            GD.Print("Skeleton slain");
            await ToSignal(_skeletonSoundPlayer, "finished");
            QueueFree();
        }
    }

    private void OnRemoveArrowSignal()
    {
    }
    public override void _Process(double delta)
    {
    }
}
