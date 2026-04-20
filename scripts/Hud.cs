using Godot;
using System;

public partial class Hud : CanvasLayer
{
	// Player node
	private Player _player;
	
	// Child nodes
	private Label _statsCounter;
	
	// Private variables
	[Export] private int _lives = 0;
	[Export] private int _ammo = 0;
	[Export] private int _level = 0;
	[Export] private int _armor = 0;
	private string _weapon = "sword";
	private bool _readyToShoot = true;
	
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
		_weapon = (_player.GetPlayerWeapon() == 0) ? "sword" : "bow";
		_readyToShoot = _player._readyToShoot;

		_statsCounter.Text = String.Format("Level: {0}\nArmor Level:{1}\n\nWeapon: {2}", _level,
			_armor, _weapon);
		_statsCounter.Text += String.Format("\nReady to shoot: {0}", _readyToShoot);
	}

	public override void _Process(double delta)
	{
		UpdateStats();
	}
}
