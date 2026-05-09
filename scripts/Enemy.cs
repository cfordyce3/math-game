using Godot;
using System;
using System.Data;

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
    [Export] private CollisionShape2D _primaryCollisionShape;
    [Export] private CollisionShape2D _detectionArea;
    [Export] public FollowMovement _followMovement;

    [Export] private ProgressBar _healthBar;

    private Player _player;

    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Killed
    }

    public EnemyState _enemyState;

    // When enemy takes a hit
    public async void OnHit(int damage)
    {
        // on hit
        _health -= damage;
        _healthBar.Value = _health;
        _healthBar.GetNode<Label>("../Label").Text = _health.ToString();

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
        _followMovement.Call(Node.MethodName.QueueFree);

        _player.Call(Player.MethodName.KilledEnemy);
        _healthBar.GetNode<Label>("../Label").Text = ""; // update label
        _primaryCollisionShape.SetDeferred("disabled", true); // disable hitbox
        _detectionArea.SetDeferred("disabled", true); // disable detection area

        _audioPlayer.Stream = _deathSound;
        _audioPlayer.Play();

        _animationSprite.Play("die");
        await ToSignal(_animationSprite, "animation_finished");
        QueueFree();
    }

    public override void _EnterTree()
    {
        // Set healthbar to max
        _healthBar.MaxValue = _health;
        _healthBar.Value = _health;
        _healthBar.GetNode<Label>("../Label").Text = _health.ToString();

        _player = GetNode<Player>("/root/Game/Player");

        _enemyState = EnemyState.Idle;
    }

    public override void _Ready()
    {
        var c = EnemyAttack.Count;
    }

    public override void _Process(double delta)
    {
        switch (_enemyState)
        {
            case EnemyState.Killed:
                EnemyKilled.Init();
                break;
            default:
                break;
        }
        EnemyAttack.Count++;
        GD.Print(EnemyAttack.Count);
    }
}
