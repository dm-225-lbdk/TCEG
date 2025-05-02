using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Maze : Control
{

	private Node2D playerInstance = new();

	private List<CanvasItem> subscenes = new();

	// індекс 0 - сцена вибору розміру лабіринту
	// індекс 1 - сцена лабіринту
	// індекс 2 - сцена перемоги

	int mazeSize = 5;

	Random random = new Random();
	record struct Cord 
	{
		public int X; 
		public int Y;

		public static Cord operator +(Cord a, Cord b){ return new() { X = a.X + b.X, Y = a.Y + b.Y }; }
		public static Cord operator *(Cord a, int b){ return new() { X = a.X * b, Y = a.Y * b }; }
		
	}

	List<List<int>> mazeMatrix = new();

	public override void _Ready()
	{
		foreach (var subscene in GetChildren())
		{
			subscenes.Add((CanvasItem)subscene);
		}
		SetSizeSubScene();
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("esc"))
		{
			if(subscenes[1].Visible) GetTree().ReloadCurrentScene();
			else GetTree().ChangeSceneToFile("res://Scenes/Menu.tscn");
		}
	}

	private void SetSizeSubScene()
	{
		ShowOnlySubscene(0);

		var valueLable = subscenes[0]
			.GetNode<VBoxContainer>("VBoxContainer")
			.GetNode<Label>("Size");

		var slider = subscenes[0]
			.GetNode<VBoxContainer>("VBoxContainer")
			.GetNode<Slider>("HSlider");

		var playButton = subscenes[0]
			.GetNode<VBoxContainer>("VBoxContainer")
			.GetNode<Button>("Button");

		slider.ValueChanged += (value) =>
		{
			valueLable.Text = value.ToString();
		};

		playButton.Pressed += () =>
		{
            mazeSize = (int)slider.Value;
            MazeSubScene();
		};
	}

	private void MazeSubScene()
	{
		TileMap mazeScene = subscenes[1].GetNode<TileMap>("MazeLayout");
		Vector2I WALL = new(0, 0);
		Vector2I FLOOR = new(1, 0);
		Vector2I FLAG = new(1, 1);

		var flag = subscenes[1].GetNode<Win>("Win");
		playerInstance = (Node2D)ResourceLoader.Load<PackedScene>("res://Assets/Entity/Fire.tscn").Instantiate();
		
		mazeMatrix = MazeGenerator();

		for (int i = 0; i < mazeSize; i++)
		{
			for (int j = 0; j < mazeSize; j++)
			{
				switch (mazeMatrix[i][j])
				{
					case 0:
						mazeScene.SetCell(0, new Vector2I(i, j), 1, FLOOR);
						break;
					case 1:
						mazeScene.SetCell(2, new Vector2I(i, j), 1, WALL);
						break;
				}
			}
		}

		int mazeWinPlace = mazeSize - ((mazeSize % 2 == 0) ? 2 : 1);

		mazeScene.SetCell(1, new Vector2I(mazeWinPlace, mazeWinPlace), 1, FLAG);
		flag.Position = new Vector2(mazeWinPlace, mazeWinPlace) * 48;
		flag.PlayerWon += WinSubScene;
		AddChild(playerInstance);
		// playerInstance.Position = new Vector2(12, 7) * 48;
		playerInstance.Position = new Vector2(0, 0);

		for (int i = -7; i < mazeSize + 7; i++)
		{
			for (int j = -5; j < mazeSize + 5; j++)
			{
				if (mazeScene.GetCellSourceId(0, new Vector2I(i, j)) == -1)
				{
					mazeScene.SetCell(2, new Vector2I(i, j), 1, WALL);
				}
			}
		}

		ShowOnlySubscene(1);
	}

	private List<List<int>> MazeGenerator()
	{
		List<List<int>> matrix = new();

		for (int i = 0; i < mazeSize; i++)
		{
			List<int> row = new();
			for (int j = 0; j < mazeSize; j++)
			{
				row.Add((i % 2 == 0) && (j % 2 == 0) ? 0 : 1);
			}
			matrix.Add(row);
		}

		return WayMaker(matrix);
	}

	private void WinSubScene()
	{
		playerInstance.QueueFree();
		ShowOnlySubscene(2);
	}

	private void ShowOnlySubscene(int index)
	{
		for (int i = 0; i < subscenes.Count; i++) subscenes[i].Visible = i == index;
	}

	private List<List<int>> WayMaker(List<List<int>> matrix)
	{
		HashSet<Cord> ways = new();
		Cord start = new() { X = 0, Y = 0 };
		Cord currentWay = start;
		ways.Add(start);
		
		List<Cord> directions = new()
		{
			new() { X = 0, Y = -1 },
			new() { X = 0, Y = 1 },
			new() { X = -1, Y = 0 },
			new() { X = 1, Y = 0 }
		};

		int waysLeft = matrix.Sum(row => row.Count(x => x == 0)) - 1;

		while (waysLeft > 0)
		{

			directions = directions.OrderBy(x => random.Next()).ToList();

			List<Cord> neibours = new();

			foreach (var direction in directions)
			{
				Cord neibourg = currentWay + direction;

				if (neibourg.X < 0 || neibourg.X >= mazeSize ||
					neibourg.Y < 0 || neibourg.Y >= mazeSize ||
					ways.Contains(neibourg) ||
					(matrix[neibourg.X][neibourg.Y] == 1)) continue;

				waysLeft--;
				neibours.Add(neibourg);
				ways.Add(neibourg);
			}

			if (neibours.Count == 0)
			{
				foreach (var direction in directions)
				{
					Cord neibourgWay = currentWay + direction * 2;
					Cord neibourgWall = currentWay + direction;

					if (neibourgWay.X < 0 || neibourgWay.X >= mazeSize ||
						neibourgWay.Y < 0 || neibourgWay.Y >= mazeSize ||
						ways.Contains(neibourgWay) ||
						(matrix[neibourgWay.X][neibourgWay.Y] == 1)) continue;

					matrix[neibourgWall.X][neibourgWall.Y] = 0;
					ways.Add(neibourgWall);
					ways.Add(neibourgWay);
					waysLeft--;
					neibours.Add(neibourgWay);
					break;
				}
				currentWay = ways.ElementAt(random.Next(ways.Count));
				continue; 
			}
			currentWay = neibours[random.Next(0, neibours.Count)];
		}
		return matrix;
	}
}