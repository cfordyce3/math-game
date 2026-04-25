using Godot;
using System;

public partial class GameManager : Node
{
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("reset_game")) GetTree().ReloadCurrentScene(); // reset game if R is pressed
    }
}