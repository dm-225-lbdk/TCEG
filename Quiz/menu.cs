using Godot;
using System;

namespace Quiz
{
    public partial class menu : Control
    {
    	private Button startButton;
		private Button rulesButton;
		private AudioStreamPlayer audioButton;
    	public override void _Ready()
    	{

			audioButton = GetNode<AudioStreamPlayer>("buttonclick");

    		startButton = GetNode<Button>("MarginContainer/VBoxContainer/startButton");
    		startButton.Pressed += () => {
				GetTree().ChangeSceneToFile("res://Scenes/quiz.tscn");
			};

			rulesButton = GetNode<Button>("MarginContainer/VBoxContainer/rulesButton");
    		rulesButton.Pressed += () => {
				GetTree().ChangeSceneToFile("res://Scenes/rules.tscn");
			};
    	}
    }
}

