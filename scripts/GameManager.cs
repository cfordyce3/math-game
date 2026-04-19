using Godot;
using System;

public partial class GameManager : Node
{
    // Player
    private Player _player;
    
    // HUD
    private CanvasLayer _hud;

    public override void _EnterTree()
    {
        // Set player node
        _player = GetNode<Player>("/root/Game/Player");
        
        // Set HUD node
        _hud = GetNode<CanvasLayer>("/root/Game/HUD");
    }
}
