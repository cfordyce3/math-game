using Godot;
using System;
using System.Collections.Generic;

public partial class FootstepAudioPlayer : AudioStreamPlayer
{
	// Player parent node
	private Player _player;
	
	// Timer child node
	private Timer _footstepTimer;
	
	// Footstep sound
	private List<AudioStreamWav> _footstepSoundListPreload = new List<AudioStreamWav>();
	private AudioStreamWav _footstepSound;
	private AudioStreamWav _previousFootstepSound;
	private int _randomSoundChoice;

	private bool _firstStep = true;
	
	public override void _Ready()
	{
		// Parent (player) node
		_player = GetParent<Player>();
		_player.Moving += OnPlayerMovingSignal;
		_player.Stopping += OnPlayerStoppingSignal;
		
		// Timer node and signal
		_footstepTimer = GetNode<Timer>("FootstepTimer");
		_footstepTimer.Timeout += OnFootstepTimerTimeoutSignal;

		// Load footstep sounds
		foreach (string file in DirAccess.Open("res://assets/sounds/footsteps").GetFiles())
		{
			if (!file.Contains("import"))
			{
				_footstepSoundListPreload.Add(GD.Load<AudioStreamWav>("res://assets/sounds/footsteps/" + file));
			}
		}
		_footstepSound = _footstepSoundListPreload[GD.RandRange(0, _footstepSoundListPreload.Count-1)]; // Get first random sound
		Stream = _footstepSound;
	}

	private void OnPlayerMovingSignal(int volume)
	{
		VolumeDb = volume;
		_footstepTimer.Start();
		Play();
	}

	private void OnPlayerStoppingSignal()
	{
		Stop();
		_footstepTimer.Stop();
	}
	private void OnFootstepTimerTimeoutSignal()
	{
		
	}

	public override void _Process(double delta)
	{
	}
}
