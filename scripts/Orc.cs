using Godot;
using System;

public partial class Orc : Enemy
{
    public override void _Ready()
    {
        GetTree().NodeAdded += OnAddArrowSignal;
    }
    
    private void OnAddArrowSignal(Node node)
    {
        if (node.Name.ToString().Contains("Arrow"))
        {
            Area2D arrow = node as Area2D;
            arrow.BodyEntered += HitByArrow;
        }
    }

    private void HitByArrow(Node node)
    {
        if (node.Name == Name)
        {
            GD.Print("Orc slain");
            QueueFree();
        }
    }
}
