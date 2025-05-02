using Godot;
using System;

public partial class Fire : CharacterBody2D
{
	private Sprite2D sprite;

	[Export] public int Speed = 200;
    AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        animationPlayer = GetNode<AnimationPlayer>("Animation");
        AddToGroup("Player");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("right")) 
		{
			direction.X += 1;
			sprite.FlipH = false;
		}
        if (Input.IsActionPressed("left"))
		{
			direction.X -= 1;
			sprite.FlipH = true;
		}
        if (Input.IsActionPressed("down")) direction.Y += 1;
        if (Input.IsActionPressed("up")) direction.Y -= 1;

        Velocity = direction.Normalized() * Speed;
        MoveAndSlide();

        if (direction != Vector2.Zero)
        {
            if (!animationPlayer.IsPlaying())
                animationPlayer.Play("player_walk");
        }
        else
        {
            if (animationPlayer.IsPlaying())
            {
                animationPlayer.Stop();
                animationPlayer.Seek(0, true);
            }
        }
    }
}
