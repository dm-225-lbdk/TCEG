using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Game
{
	public partial class Tetris : Control
	{
		public TetrisDB figuresDB = new();
		public List<List<int>> board = new();
		public List<List<int>> UIBoard = new();
		public List<List<int>> figure = new();
		public List<List<int>> nextfigure = new();
		public Coord figurePosition;
		public Random rand = new();
		public int time = 800;
		public bool isGameCountinues = true;
		public string command = "next";
		public record Coord(int X, int Y);
		public int score = 0;
		public bool isCommadProcessing = false;

		Control TetrisGame;
		TileMap TetrisMap;
		MarginContainer Result;

		public override async void _Ready()
		{
			TetrisGame = GetNode<Control>("MarginContainer/CenterContainer");
			TetrisMap = TetrisGame.GetNode<TileMap>("Tetris/TileMap");

			for (int i = 0; i < 22; i++)
			{
				board.Add(new List<int>(new int[10]));
			}

			for (int i = 0; i < 20; i++)
			{
				UIBoard.Add(new List<int>(new int[10]));
			}

			figure = figuresDB.Figures[rand.Next(0, figuresDB.Figures.Count)];

			while (isGameCountinues)
			{
				await FigureSession();
				if (time > 250) time -= 25;
			}
			Result = TetrisGame.GetNode<MarginContainer>("Result");
			Result.Visible = true;
			TetrisMap.Visible = false;
			Result.GetNode<Label>("VBoxContainer/Label").Text = $"Game Over\nYour score: {score}";
			Result.GetNode<Button>("VBoxContainer/Button").Pressed += () => GetTree().ChangeSceneToFile("res://Scenes/Menu.tscn");
		}
		public override void _Process(double delta)
		{
			if (!isCommadProcessing)
			{
				if (Input.IsActionJustPressed("left"))
				{
					isCommadProcessing = true;
					MoveFigure("left");
					isCommadProcessing = false;
				}

				if (Input.IsActionJustPressed("right"))
				{
					isCommadProcessing = true;
					MoveFigure("right");
					isCommadProcessing = false;
				}

				if (Input.IsActionJustPressed("rotate"))
				{
					isCommadProcessing = true;
					RotateFigure();
					isCommadProcessing = false;
				}

				if (Input.IsActionJustPressed("place"))
				{
					isCommadProcessing = true;
					DownFigure();
					isCommadProcessing = false;
				}
			}
		}
		public async Task FigureSession()
		{
			bool figureTurn = true;
			figurePosition = new(0, 4);
			board[figurePosition.X][figurePosition.Y] = -1;
			nextfigure = figuresDB.Figures[rand.Next(0, figuresDB.Figures.Count)];

			List<List<int>> _figure = figure.Select(row => new List<int>(row)).ToList();
			List<List<int>> _nextfigure = nextfigure.Select(row => new List<int>(row)).ToList();

			while (_figure.Count < 4)
				_figure.Add(new List<int>());
			while (_nextfigure.Count < 4)
				_nextfigure.Add(new List<int>());
			
			for (int i = 0; i < _figure.Count; i++)
    		{
				while (_figure[i].Count < 4)
					_figure[i].Add(0);
			}

			for (int i = 0; i < _nextfigure.Count; i++)
    		{
				while (_nextfigure[i].Count < 4)
					_nextfigure[i].Add(0);
			}

			TileMapTileUpdate(new Coord(1, 13), _figure);
			TileMapTileUpdate(new Coord(8, 13), _nextfigure);

			while (figureTurn) figureTurn = await TurnSession();

			figure = nextfigure;
			CopyBoard();
		}
		public void CopyBoard()
		{
			int writeRow = 21;

			for (int i = 19; i >= 0; i--)
			{
				bool isFullRow = true;
				for (int j = 0; j < 10; j++)
				{
					if (UIBoard[i][j] == 0)
					{
						isFullRow = false;
						break;
					}
				}

				if (!isFullRow)
				{

					for (int j = 0; j < 10; j++)
					{
						board[writeRow][j] = UIBoard[i][j];
					}
					writeRow--;
				}

				else
				{
					score += UIBoard[i].Sum();
				}
			}

			for (int i = writeRow; i >= 0; i--)
			{
				for (int j = 0; j < 10; j++)
				{
					board[i][j] = 0;
				}
			}

		}
		public List<List<int>> RotateRight(List<List<int>> matrix)
		{
			int rowCount = matrix.Count;
			int colCount = matrix[0].Count;
			List<List<int>> rotated = new();

			for (int col = 0; col < colCount; col++)
			{
				List<int> newRow = new();
				for (int row = rowCount - 1; row >= 0; row--)
				{
					newRow.Add(matrix[row][col]);
				}
				rotated.Add(newRow);
			}

			return rotated;
		}
		public async Task<Boolean> TurnSession()
		{
			await Task.Delay(time);
			return MoveFigure("next");
		}
		public bool MoveFigure(string _command)
		{
			command = _command;
			
			switch (command)
			{
				case "next":
					return MovePosition(new Coord(1, 0));
				case "left":
					return MovePosition(new Coord(0, -1));
				case "right":
					return MovePosition(new Coord(0, 1));
			}
			return true;
		}
		public bool MovePosition(Coord delta)
		{
			board[figurePosition.X][figurePosition.Y] = 0;

			Coord testPosition = new(figurePosition.X + delta.X, figurePosition.Y + delta.Y);

			if (testPosition.X < 0 || testPosition.X >= board.Count ||
				testPosition.Y < 0 || testPosition.Y >= board[0].Count)
			{
				return false;
			}

			figurePosition = testPosition;
			return TileMapUpdate();
		}
		public bool RotateFigure()
		{
			figure = RotateRight(figure);
			return false;
		}
		public bool DownFigure()
		{
			while (MoveFigure("next")) { }
			return false;
		}
		public bool TileMapUpdate()
		{
			for (int y = 0; y < UIBoard.Count; y++)
			{
				for (int x = 0; x < UIBoard[y].Count; x++)
				{
					UIBoard[y][x] = board[y + 2][x];
				}
			}

			bool returningVal = MatrixUpdate(UIBoard, figure, figurePosition);

			TileMapTileUpdate(new Coord(0, 0), UIBoard);

			return returningVal;
		}
		public bool MatrixUpdate(List<List<int>> board, List<List<int>> figure, Coord position)
		{
			for (int y = 0; y < figure.Count; y++)
			{
				for (int x = 0; x < figure[y].Count; x++)
				{
					int value = figure[y][x];
					if (value != 0)
					{
						int boardX = position.X + y;
						int boardY = position.Y + x;

						if (boardX < 0)
						{
							isGameCountinues = false;
							return false;
						}
						else if (boardX >= board.Count ||
							boardY < 0 || boardY >= board[0].Count ||
							board[boardX][boardY] != 0)
						{
							if (command == "next")
								return !MatrixUpdate(board, figure, new Coord(position.X - 1, position.Y));
							else if (command == "left")
								return MatrixUpdate(board, figure, new Coord(position.X, position.Y + 1));
							else if (command == "right")
								return MatrixUpdate(board, figure, new Coord(position.X, position.Y - 1));
						}

					}
				}
			}

			for (int y = 0; y < figure.Count; y++)
			{
				for (int x = 0; x < figure[y].Count; x++)
				{
					int value = figure[y][x];
					if (value != 0)
					{
						int boardX = position.X + y;
						int boardY = position.Y + x;
						board[boardX][boardY] = value;
					}
				}
			}

			return true;
		}
		public void TileMapTileUpdate(Coord startPos, List<List<int>> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list[i].Count; j++)
				{
					int value = list[i][j];

					int mapX = startPos.X + i;
					int mapY = startPos.Y + j;

					if (value != 0)
						TetrisMap.SetCell(2, new Vector2I(mapY, mapX), 2, new Vector2I(0, 0), value);
					else
						TetrisMap.SetCell(2, new Vector2I(mapY, mapX), 0, new Vector2I(0, 0), 0);
				}
			}
		}

	}
}