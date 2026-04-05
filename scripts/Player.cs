using Godot;
using System;

public partial class Player : CharacterBody2D
{
<<<<<<< HEAD
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    public override void _Ready()
    {
        // Vector2 ScreenSize = GetViewportRect().Size;
        // Hide();
    }

    public override void _Process(double delta)
    {
        Vector2 velocity = Velocity;

        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Vector2 veloity = Velocity;

        // Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        // if (direction != Vector2.Zero)
        // {
        //     velocity.X = direction.X * Speed;
        // }
        // else
        // {
        //     velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        // }

        // Velocity = velocity;
        // MoveAndSlide();
    }

=======
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	
	public override void _Ready(){
		
		Hide()
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
>>>>>>> master
}
