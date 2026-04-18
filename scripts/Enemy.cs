using Godot;
using System;

// Generic enemy class of type CharacterBody2D
[GlobalClass]
public partial class Enemy : CharacterBody2D
{
    [Export] private int _health;
    [Export] private int _speed;
    [Export] private int _damage;

    public override void _Ready()
    {
    }
}