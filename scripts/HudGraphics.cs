using Godot;
using System;
using System.ComponentModel;

public partial class HudGraphics : Control
{
	// Player node
	private Player _player;

	// Child nodes
	private AnimatedSprite2D[] _hearts = new AnimatedSprite2D[5];
	private AnimatedSprite2D[] _arrows = new AnimatedSprite2D[8];

	// Private variables
	[Export] private int _lives = 0;
	[Export] private int _ammo = 0;

	public override void _Ready()
	{
		_player = GetNode<Player>("/root/Game/Player");
		_lives = _player.Lives;
		_ammo = _player.Ammo;
		
		// Get all the hearts
		for (int i = 0; i < _hearts.Length; i++)
		{
			_hearts[i] = GetNode<AnimatedSprite2D>("Hearts/Heart" + (i+1)); // add each texture to array
			
			// set starting number of hearts to whatever player's hearts is
			if (i < _player.Lives) _hearts[i].Play("idle");
			else _hearts[i].Play("empty");
		}

		// Get all arrows
		for (int i = 0; i < _arrows.Length; i++)
		{
			_arrows[i] = GetNode<AnimatedSprite2D>("Arrows/Arrow" + (i+1));

			if (i < _player.Ammo) _arrows[i].Visible = true;
			else _arrows[i].Visible = false;
		}
	}

	private void UpdateGraphics()
	{
		// Update hearts
		for (int i = 0; i < _hearts.Length; i++) 
		{
			if (i < _player.Lives) _hearts[i].Play("idle");
			else _hearts[i].Play("empty");
		}
		_lives = _player.Lives; // now set the current known lives to current player lives
	}

	private void UpdateAmmo()
	{
		// Update arrows/ammo
		for (int i = 0; i < _arrows.Length; i++)
		{
			if (i < _player.Ammo) _arrows[i].Visible = true;
			else _arrows[i].Visible = false;
		}
		_ammo = _player.Ammo;
	}

	public override void _Process(double delta)
	{
		if (_lives != _player.Lives) UpdateGraphics(); // if locally-known lives != player's lives then update
		if (_ammo != _player.Ammo) UpdateAmmo();
	}
}
