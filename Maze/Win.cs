using Godot;
using System;

public partial class Win : Area2D
{
	[Signal]
    public delegate void PlayerWonEventHandler();
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("Player"))
        {
            EmitSignal("PlayerWon");
        }
    }
}

