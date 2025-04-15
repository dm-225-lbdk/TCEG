using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quiz
{
    public partial class rules : Control
    {

    	private QuizData _rules = new(DataChoice.rules);
    	private Button backButton;
        private RichTextLabel rulesText;
		private AudioStreamPlayer audioButton;

    	public override void _Ready()
    	{
			audioButton = GetNode<AudioStreamPlayer>("buttonclick");
			audioButton.Play();
			
    		backButton = GetNode<Button>("backButton");
    		backButton.Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");	

            rulesText = GetNode<RichTextLabel>("rules");	
            rulesText.Text = _rules.Rules;
    	}
	
    }
}