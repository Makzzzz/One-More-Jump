using System;
using System.Collections.Generic;
using System.Drawing;

namespace OneMoreJump.Model;

public class GameModel
{
    private const int GridSize = 40;
    private const int InitialLives = 3;
    private const int PointsPerSafeCrossing = 100;

    public Frog PlayerFrog { get; private set; }
    public List<Obstacle> Obstacles { get; }
    public int Score { get; private set; }
    public int Level { get; private set; }
    public int Lives { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    private readonly LevelGenerator _levelGenerator;
    private readonly Stack<GameState> _gameStateHistory;

    public GameModel()
    {
        _levelGenerator = new LevelGenerator();
        _gameStateHistory = new Stack<GameState>();
        Obstacles = new List<Obstacle>();

        InitializeNewGame();
    }

    private void InitializeNewGame()
    {
        var startPosition = new Point(7 * GridSize, 14 * GridSize);
        PlayerFrog = new Frog(startPosition, GridSize);

        Score = 0;
        Level = 1;
        Lives = InitialLives;
        IsGameOver = false;
        IsPaused = false;

        GenerateLevelObstacles();
        SaveGameState();
    }

    private void GenerateLevelObstacles()
    {
        Obstacles.Clear();
        var newObstacles = _levelGenerator.GenerateObstacles(Level, GridSize);
        Obstacles.AddRange(newObstacles);
    }

    public void MoveFrog(Direction direction)
    {
        if (IsGameOver || IsPaused) return;

        SaveGameState();

        var newPosition = CalculateNewPosition(PlayerFrog.Position, direction);

        if (IsPositionWithinBounds(newPosition))
        {
            PlayerFrog.Position = newPosition;
            CheckCollisions();
            CheckSafeZoneReached();
        }
    }

    private Point CalculateNewPosition(Point currentPosition, Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Point(currentPosition.X, currentPosition.Y - GridSize),
            Direction.Down => new Point(currentPosition.X, currentPosition.Y + GridSize),
            Direction.Left => new Point(currentPosition.X - GridSize, currentPosition.Y),
            Direction.Right => new Point(currentPosition.X + GridSize, currentPosition.Y),
            _ => currentPosition
        };
    }

    private bool IsPositionWithinBounds(Point position)
    {
        var maxX = 15 * GridSize;
        var maxY = 20 * GridSize;

        return position.X >= 0 && position.X < maxX &&
               position.Y >= 0 && position.Y < maxY;
    }

    private void CheckCollisions()
    {
        var frogBounds = PlayerFrog.GetBounds();

        foreach (var obstacle in Obstacles)
        {
            if (frogBounds.IntersectsWith(obstacle.Bounds))
            {
                HandleCollision(obstacle);
                break;
            }
        }
    }

    private void HandleCollision(Obstacle obstacle)
    {
        if (obstacle.Type == ObstacleType.Car)
        {
            Lives--;

            if (Lives <= 0)
            {
                IsGameOver = true;
            }
            else
            {
                PlayerFrog.Position = new Point(7 * GridSize, 14 * GridSize);
            }
        }
        else if (obstacle.Type == ObstacleType.Log || obstacle.Type == ObstacleType.Turtle)
        {
        }
        else if (obstacle.Type == ObstacleType.Crocodile)
        {
            Lives = 0;
            IsGameOver = true;
        }
    }

    private void CheckSafeZoneReached()
    {
        if (PlayerFrog.Position.Y < 2 * GridSize)
        {
            Score += PointsPerSafeCrossing;
            Level++;
            GenerateLevelObstacles();
            PlayerFrog.Position = new Point(7 * GridSize, 14 * GridSize);
        }
    }

    private void SaveGameState()
    {
        if (_gameStateHistory.Count >= 10)
        {
            while (_gameStateHistory.Count > 9)
            {
                _gameStateHistory.Pop();
            }
        }

        var state = new GameState
        {
            FrogPosition = PlayerFrog.Position,
            Score = Score,
            Level = Level,
            Lives = Lives
        };

        _gameStateHistory.Push(state);
    }
    
    public void ResetGame()
    {
        InitializeNewGame();
    }
}