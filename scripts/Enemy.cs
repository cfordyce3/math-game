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
    
    [Export] private AnimatedSprite2D _animationSprite;
    
    [Export] private ProgressBar _healthBar;

    
    // When enemy takes a hit
    public async void OnHit(int damage)
    {
        // on hit
        _health -= damage;
        _healthBar.Value = _health;
        
        if (_health < 1) OnKilled();  // hit and killed
        else // hit but not killed
        {
            _audioPlayer.Stream = _hitSound; // load hit sound
            _audioPlayer.Play(); // play hit sound
            
            _animationSprite.Play("damaged");
            await ToSignal(_animationSprite, "animation_finished");
            _animationSprite.Play("idle");
        }
    }
    
    // When enemy is killed
    public async void OnKilled()
    {
        _audioPlayer.Stream = _deathSound;
        _audioPlayer.Play();
        _animationSprite.Play("die");
        await ToSignal(_animationSprite, "animation_finished");
        QueueFree();
    }

    public override void _EnterTree()
    {
        //
    }
    
    public override void _Ready()
    {
        // Set healthbar to max
        _healthBar.MaxValue = _health; 
        _healthBar.Value = _health;
    }
}