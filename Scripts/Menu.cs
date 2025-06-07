using Godot;
using System;

public partial class Menu : Control
{
	private Button _playButton;
	public override void _Ready()
	{
		_playButton = GetNode<Button>("CenterContainer/VBoxContainer/PlayButton");
		_playButton.Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/Tetris.tscn");
	}
}
