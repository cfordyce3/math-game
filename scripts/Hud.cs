using Godot;
using System;

public partial class Hud : CanvasLayer
{
	// Player node
	private Player _player;
	
	// Child nodes
	private Label _statsCounter;
	
	// Private variables
	[Export] private int _lives;
	[Export] private int _ammo;
	[Export] private int _level;
	[Export] private int _armor;
	
	// Public variables
	
	public override void _Ready()
	{
		_player = GetNode<Player>("../Player");
		_statsCounter = GetNode<Label>("StatsCounter");
	}

	private void UpdateStats()
	{
		_lives = _player.Lives;
		_ammo = _player.Ammo;
		_level = _player.Level;
		_armor = _player.Armor;

		_statsCounter.Text = "Level:" + _level + "\nArmor Level:" + _armor;
		_statsCounter.Text += "\nPress R to reset";
		//_statsCounter.Text += String.Format("\nReady to shoot: {0}", _readyToShoot);
	}

	public override void _Process(double delta)
	{
		UpdateStats();
	}
}
