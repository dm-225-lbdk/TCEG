using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quiz
{
    public partial class quiz : Control
    {
    	private QuizData _quiz = new(DataChoice.quizData);
    	private Button backButton;
		private Button helpButton;
        private Label helpCount;
        private Button halfButton;
        private Label halfCount;
        private Label question;
    	private Label scores;
    	private Label debug;

		private GpuParticles2D coins;
		private AudioStreamPlayer audioButton;
		private AudioStreamPlayer victorySound;
    	private List<Button> answers = new();
		private List<Label> helpLabels = new();

    	private int quizScore = 0;
    	private int currentQuestion = 0;
    	private Random random = new();
    	private Dictionary<Button, Action> buttonActions = new();
		private Dictionary<Button, int> buttonValue = new();

		private Dictionary<Button, Label> buttonLabels = new();

    	public override void _Ready()
		{
			
			backButton = GetNode<Button>("backButton");

			halfButton = GetNode<Button>("MarginContainer2/HBoxContainer/VBoxContainer2/halfButton");
			halfCount = GetNode<Label>("MarginContainer2/HBoxContainer/VBoxContainer2/Label");
			helpButton = GetNode<Button>("MarginContainer2/HBoxContainer/VBoxContainer/helpButton");
			helpCount = GetNode<Label>("MarginContainer2/HBoxContainer/VBoxContainer/Label");

			question = GetNode<Label>("MarginContainer/VBoxContainer/quest");

			answers.Add(GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer1/Answer"));
			answers.Add(GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/Answer"));
			answers.Add(GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer3/Answer"));
			answers.Add(GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer4/Answer"));

			helpLabels.Add(GetNode<Label>("MarginContainer/VBoxContainer/HBoxContainer1/Label"));
			helpLabels.Add(GetNode<Label>("MarginContainer/VBoxContainer/HBoxContainer2/Label"));
			helpLabels.Add(GetNode<Label>("MarginContainer/VBoxContainer/HBoxContainer3/Label"));
			helpLabels.Add(GetNode<Label>("MarginContainer/VBoxContainer/HBoxContainer4/Label"));

			for (int i = 0; i < answers.Count; i++) buttonLabels.Add(answers[i], helpLabels[i]);

			scores = GetNode<Label>("MarginContainer/VBoxContainer/scores");
			
			backButton.Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");		
			halfButton.Pressed += HalfHandler;
			helpButton.Pressed += HelpHandler;

			audioButton = GetNode<AudioStreamPlayer>("buttonclick");
			audioButton.Play();

			victorySound = GetNode<AudioStreamPlayer>("victory");
			coins = GetNode<GpuParticles2D>("coins");

			_quiz.Questions = _quiz.Questions.OrderBy(x => random.Next()).ToList();

			QuizAlgorithm();
		}

        private void QuizAlgorithm()
    	{
			buttonValue.Clear();

    		foreach (var button in answers)
    		{
    			if (buttonActions.ContainsKey(button))
    			{
    				button.Pressed -= buttonActions[button];
    			}
    		}

    		Question item = _quiz.Questions[currentQuestion];
    		question.Text = item.Quest;
    		scores.Text = "Ваш виграш: " + quizScore*10000;
    		List<int> keys = item.Answers.Keys.OrderBy(x => random.Next()).ToList();

    		for (int i = 0; i < answers.Count; i++)
    		{
    			if (i < keys.Count)
    			{
					int keyScore = keys[i];
    				answers[i].Text = item.Answers[keyScore];
					buttonValue.Add(answers[i], keyScore);

    				Action handler = () => AnswerHandler(keyScore);
    				buttonActions[answers[i]] = handler;
    				answers[i].Pressed += handler;
    			}
    		}
    	}

        private void HelpHandler()
        {
			audioButton.Play();
			int keyValuesSum = buttonValue.Values.Where(x => x > 0).Sum();

			foreach (Button button in answers) {
				Label helpLabel = buttonLabels[button];
				helpLabel.Visible = true;
				helpLabel.Text = ((float)buttonValue[button] / keyValuesSum * 100).ToString("0.##") + "%";
			}

            helpCount.Text = "0/1";
			helpButton.Disabled = true;
        }

        private void HalfHandler()
        {
			audioButton.Play();
            foreach (Button button in answers) if (buttonValue[button] <= 0) button.Disabled = true;

			halfCount.Text = "0/1";
			halfButton.Disabled = true;
        }


        private void AnswerHandler(int keyScore)
        {

    		quizScore += keyScore;
			coins.Amount += keyScore;

    		if(currentQuestion > 3){
				victorySound.Play();
    			question.Text = "Ви пройшли!";
    			foreach (Button button in answers){
					button.Visible = false;
					Label helpLabel = buttonLabels[button];
					helpLabel.Visible = false;
				}
				
				helpButton.Visible = false;
				halfButton.Visible = false;

				helpCount.Visible = false;
				halfCount.Visible = false;

    			return;
    		}
			else {
				audioButton.Play();
				foreach (Button button in answers) {
					button.Disabled = false;
					Label helpLabel = buttonLabels[button];
					helpLabel.Visible = false;
				}
			}
        
    		currentQuestion++;
    		QuizAlgorithm();
        }
    }
}