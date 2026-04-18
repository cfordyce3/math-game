using Godot;
using System;

public partial class HudGraphics : Control
{
	// Player node
	private Player _player;

	// Child nodes
	private TextureRect _heart1;
	private TextureRect _heart2;
	private TextureRect _heart3;
	private TextureRect _heart4;
	private TextureRect _heart5;

	// Private variables
	[Export] private int _lives = 0;

	public override void _Ready()
	{
		_player = GetNode<Player>("../../Player");
		
		// Get all the hearts
		_heart1 = GetNode<TextureRect>("FullHearts/Heart1");
		_heart2 = GetNode<TextureRect>("FullHearts/Heart2");
		_heart3 = GetNode<TextureRect>("FullHearts/Heart3");
		_heart4 = GetNode<TextureRect>("FullHearts/Heart4");
		_heart5 = GetNode<TextureRect>("FullHearts/Heart5");
	}

	private void UpdateGraphics()
	{
		_lives = _player.Lives;

		switch (_lives)
		{
			case 5:
				_heart1.Visible = true;
				_heart2.Visible = true;
				_heart3.Visible = true;
				_heart4.Visible = true;
				_heart5.Visible = true;
				break;
			case 4:
				_heart1.Visible = true;
				_heart2.Visible = true;
				_heart3.Visible = true;
				_heart4.Visible = true;
				_heart5.Visible = false;
				break;
			case 3:
				_heart1.Visible = true;
				_heart2.Visible = true;
				_heart3.Visible = true;
				_heart4.Visible = false;
				_heart5.Visible = false;
				break;
			case 2:
				_heart1.Visible = true;
				_heart2.Visible = true;
				_heart3.Visible = false;
				_heart4.Visible = false;
				_heart5.Visible = false;
				break;
			case 1:
				_heart1.Visible = true;
				_heart2.Visible = false;
				_heart3.Visible = false;
				_heart4.Visible = false;
				_heart5.Visible = false;
				break;
			case 0:
				_heart1.Visible = false;
				_heart2.Visible = false;
				_heart3.Visible = false;
				_heart4.Visible = false;
				_heart5.Visible = false;
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateGraphics();
	}
}
