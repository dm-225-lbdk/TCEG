using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class sudoku : Control 
{
	private VBoxContainer MainSceene;
	private HBoxContainer GameOverSceene;

	private Button backButton;
	private List<Button> ChooseNumberButton = new();
	private List<List<Button>> BoxButton = new();
	private Label NumberState;
	private Label LivesState;
	private Control Griding3x3;

	private Label GameOverLabel;
	private Label Info;


	Random random = new();
    private int currentNumber = 1;
	private int currentLives = 5;
	private int startNumbersCount = 0;
	private List<List<int>> matrix = new();
	private Dictionary<int, int> maxNumbers = new();

	public override void _Ready()
	{
		for (int i = 0; i < 9; i++) matrix.Add(Enumerable.Repeat(0, 9).ToList());
		for (int i = 1; i < 10; i++) maxNumbers.Add(i, 0);

		NumberState = GetNode<Label>("VBoxContainer/HBoxContainer2/ChoosedNumber");
		LivesState = GetNode<Label>("VBoxContainer/HBoxContainer2/Lives");

		backButton = GetNode<Button>("Button");
		backButton.Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");

		foreach (Button child in GetNode<GridContainer>("VBoxContainer/HBoxContainer/NumberChoose/GridContainer").GetChildren().Cast<Button>()) ChooseNumberButton.Add(child);

		for (int i = 0; i < 9; i++)
		{
			BoxButton.Add(new List<Button>());
			for (int j = 0; j < 9; j++)
			{
				var button = GetNode<GridContainer>("VBoxContainer/HBoxContainer/SudokuPlate/GridContainer").GetChild<Button>(i * 9 + j);
				BoxButton[i].Add(button);
				int localI = i;
				int localJ = j;
				BoxButton[i][j].Pressed += () => {OnBoxButtonPressed(localI, localJ);};
			}
		}

		SudokuGenerator();

		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (random.Next(0, 2) == 1)
				{
					maxNumbers[matrix[i][j]]--;
					ButtonSetNumber(i, j, 0);
				}
			}
		}

		startNumbersCount = maxNumbers.Values.Sum();

		foreach (Button button in ChooseNumberButton){
			button.Pressed += () => {
				OnChooseNumberButtonPressed(int.Parse(button.Text));
			};
		}
	}

	public void OnWrongAnswer()
	{
		currentLives--;
		if (currentLives == 0)
		{
			EndCase("Ви програли!");
			return;
		}
		LivesState.Text = $"Кількість спроб: {currentLives}";
	}

	private void OnChooseNumberButtonPressed(int number)
	{
		currentNumber = number;
		if (maxNumbers[number] == 9) ChooseNumberButton[number - 1].Disabled = true;
		NumberState.Text = $"Обрана цифра: {currentNumber}";
	}

	private void OnBoxButtonPressed(int i, int j)
	{

		if (matrix[i][j] != 0) return;

		if (CheckBox(i, j, currentNumber) == false)
		{
			OnWrongAnswer();
			return;
		}

		if (maxNumbers.Values.Sum() == 81) EndCase("Ви виграли!");

		ButtonSetNumber(i, j, currentNumber);
	}

    private void EndCase(string endState)
    {
        MainSceene = GetNode<VBoxContainer>("VBoxContainer");
		Griding3x3 = GetNode<Control>("Griding3x3");
		GameOverSceene = GetNode<HBoxContainer>("HBoxContainer");

		MainSceene.Visible = false;
		Griding3x3.Visible = false;
		GameOverSceene.Visible = true;

		GameOverLabel = GetNode<Label>("HBoxContainer/Lose");
		Info = GetNode<Label>("HBoxContainer/Info");

		GameOverLabel.Text = endState + "\nРахунок: " + ((maxNumbers.Values.Sum() - startNumbersCount) * 100 / 81) + " (" + maxNumbers.Values.Sum() + "/81)";
		Info.Text = 
		"1: " + maxNumbers[1] + 
		"\n2: " + maxNumbers[2] +
		"\n3: " + maxNumbers[3] +
		"\n4: " + maxNumbers[4] +
		"\n5: " + maxNumbers[5] +
		"\n6: " + maxNumbers[6] +
		"\n7: " + maxNumbers[7] +
		"\n8: " + maxNumbers[8] +
		"\n9: " + maxNumbers[9];
    }

    private void ButtonSetNumber(int i, int j, int number)
	{
		matrix[i][j] = number;
		if (number == 0)
		{
			BoxButton[i][j].Text = "";
			return;
		}
		maxNumbers[number]++;
		if (maxNumbers[number] == 9) ChooseNumberButton[number - 1].Disabled = true;
		BoxButton[i][j].Text = matrix[i][j].ToString();
	}

	private bool CheckBox(int i, int j, int number)
	{
		for (int line = 0; line < 9; line++)
		{
			if (matrix[i][line] == number 
			|| matrix[line][j] == number) return false;

			int startRow = 3 * (i / 3) + line / 3;
            int startCol = 3 * (j / 3) + line % 3;
			if (matrix[startRow][startCol] == number) return false;
		}
		return true;
	}

	private bool SudokuGenerator()
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (matrix[i][j] == 0)
				{
					List<int> numbers = Enumerable.Range(1, 9).OrderBy(x => Guid.NewGuid()).ToList();

					foreach (int number in numbers)
					{
						if (CheckBox(i, j, number))
						{
							matrix[i][j] = number;

							maxNumbers[number]++;	

							if (SudokuGenerator()) 
							{
								MatrixFielding();
								return true;
							}

							maxNumbers[number]--;
							matrix[i][j] = 0;
						}
					}
					return false;
				}
			}
		}
		return true;
	}

	private void MatrixFielding()
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				BoxButton[i][j].Text = matrix[i][j].ToString();
			}
		}
	}
}