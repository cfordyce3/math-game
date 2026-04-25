using Godot;
using System;

// Generic enemy class of type CharacterBody2D
[GlobalClass]
public partial class Enemy : CharacterBody2D
{
    [Export] private int _health;
    [Export] private int _speed;
    [Export] private int _damage;

    [Export] private AudioStreamWav _hitSound;
    [Export] private AudioStreamWav _deathSound;

    [Export] private AudioStreamPlayer _audioPlayer;
    
    // When enemy takes a hit
    public async void OnHit()
    {
        // on hit
        _audioPlayer.Stream = _hitSound;
        _audioPlayer.Play();
        _health--;
        if (_health == 0) OnKilled();
        else await ToSignal(_audioPlayer, "finished");
    }
    
    // When enemy is killed
    public async void OnKilled()
    {
        _audioPlayer.Stream = _deathSound;
        _audioPlayer.Play();
        await ToSignal(_audioPlayer, "finished");
        QueueFree();
    }
    
    public override void _Ready()
    {
    }
}