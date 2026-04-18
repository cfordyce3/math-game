using Godot;
using System;

public partial class HudGraphics : Control
{
	// Player node
	private Player _player;

	// Child nodes
	private TextureRect[] _hearts = new TextureRect[5];
	// private TextureRect _heart1;
	// private TextureRect _heart2;
	// private TextureRect _heart3;
	// private TextureRect _heart4;
	// private TextureRect _heart5;

	// Private variables
	[Export] private int _lives = 0;

	public override void _Ready()
	{
		_player = GetNode<Player>("/root/Game/Player");
		_lives = _player.Lives;
		
		// Get all the hearts
		for (int i = 0; i < _hearts.Length; i++)
		{
			_hearts[i] = GetNode<TextureRect>("FullHearts/Heart" + (i+1)); // add each texture to array
			
			// set starting number of hearts to whatever player's hearts is
			if (i < _player.Lives) _hearts[i].Visible = true; 
			else _hearts[i].Visible = false;
		}

	}

	private void UpdateGraphics()
	{
		for (int i = 0; i < _hearts.Length; i++) 
		{
			if (i < _player.Lives) _hearts[i].Visible = true;
			else _hearts[i].Visible = false;
		}
		_lives = _player.Lives; // now set the current known lives to current player lives
	}

	public override void _Process(double delta)
	{
		if (_lives != _player.Lives) UpdateGraphics(); // if locally-known lives != player's lives then update
	}
}
