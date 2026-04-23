using System;
using System.Collections.Generic;
using System.Drawing;

namespace OneMoreJump.Model
{
    public class LevelGenerator
    {
        private const int MinCarsPerRow = 2;
        private const int MaxCarsPerRow = 6;
        private const int MinLogsPerRow = 1;
        private const int MaxLogsPerRow = 4;
        private const int CarWidthMultiplier = 2;
        private const int LogWidthMultiplier = 3;

        private readonly Random _random;

        public LevelGenerator() => _random = new Random();

        public LevelGenerator(int seed) => _random = new Random(seed);

        public List<Obstacle> GenerateObstacles(int level, int gridSize)
        {
            var obstacles = new List<Obstacle>();
            var gameWidth = 15 * gridSize;

            GenerateRoadObstacles(obstacles, level, gridSize, gameWidth);
            GenerateRiverObstacles(obstacles, level, gridSize, gameWidth);

            if (level >= 5)
            {
                GenerateSpecialObstacles(obstacles, level, gridSize, gameWidth);
            }

            return obstacles;
        }

        private void GenerateRoadObstacles(List<Obstacle> obstacles, int level, int gridSize, int gameWidth)
        {
            const int roadRows = 5;
            var baseCarSpeed = 2 + (level / 2);

            for (var row = 0; row < roadRows; row++)
            {
                var y = (3 + row) * gridSize;
                var direction = (row % 2 == 0) ? 1 : -1;
                var speed = baseCarSpeed * direction;
                var carCount = Math.Min(MinCarsPerRow + level, MaxCarsPerRow);

                for (var i = 0; i < carCount; i++)
                {
                    var x = _random.Next(0, gameWidth);
                    var carWidth = gridSize * CarWidthMultiplier;
                    var bounds = new Rectangle(x, y, carWidth, gridSize);
                    var carType = GetRandomCarType();
                    obstacles.Add(new Obstacle(bounds, carType, speed));
                }
            }
        }

        private void GenerateRiverObstacles(List<Obstacle> obstacles, int level, int gridSize, int gameWidth)
        {
            const int riverRows = 5;
            var baseLogSpeed = 1 + (level / 3);

            for (var row = 0; row < riverRows; row++)
            {
                var y = (8 + row) * gridSize;
                var direction = (row % 3 == 0) ? 1 : -1;
                var speed = baseLogSpeed * direction;
                var logCount = Math.Min(MinLogsPerRow + (level / 2), MaxLogsPerRow);

                for (var i = 0; i < logCount; i++)
                {
                    var logWidth = gridSize * LogWidthMultiplier;
                    var x = _random.Next(0, gameWidth - logWidth);
                    var bounds = new Rectangle(x, y, logWidth, gridSize);
                    var riverType = GetRandomRiverType(level);
                    obstacles.Add(new Obstacle(bounds, riverType, speed));
                }
            }
        }

        private void GenerateSpecialObstacles(List<Obstacle> obstacles, int level, int gridSize, int gameWidth)
        {
            if (level >= 5)
            {
                var crocodileCount = Math.Min(level / 5, 3);

                for (var i = 0; i < crocodileCount; i++)
                {
                    var y = _random.Next(8 * gridSize, 13 * gridSize);
                    var x = _random.Next(0, gameWidth - gridSize * 2);
                    var speed = _random.Next(1, 3) * (_random.Next(0, 2) * 2 - 1);
                    var bounds = new Rectangle(x, y, gridSize * 2, gridSize);
                    obstacles.Add(new Obstacle(bounds, ObstacleType.Crocodile, speed));
                }
            }

            if (level >= 8)
            {
                var fastCarRow = _random.Next(3, 8);
                var y = fastCarRow * gridSize;
                var x = _random.Next(0, gameWidth);
                var speed = (level / 2) * (_random.Next(0, 2) * 2 - 1);
                var bounds = new Rectangle(x, y, gridSize * 2, gridSize);
                obstacles.Add(new Obstacle(bounds, ObstacleType.Car, speed));
            }
        }

        private ObstacleType GetRandomCarType() => ObstacleType.Car;

        private ObstacleType GetRandomRiverType(int level)
        {
            var rand = _random.Next(100);

            return level switch
            {
                >= 7 when rand < 20 => ObstacleType.Crocodile,
                >= 4 when rand < 40 => ObstacleType.Turtle,
                _ => ObstacleType.Log
            };
        }

        public double CalculateLevelDifficulty(int level, int gridSize)
        {
            var obstacles = GenerateObstacles(level, gridSize);
            var difficulty = 0.0;

            foreach (var obstacle in obstacles)
            {
                difficulty += Math.Abs(obstacle.Speed) * 0.5;
                difficulty += 1.0;

                switch (obstacle.Type)
                {
                    case ObstacleType.Crocodile:
                        difficulty += 2.0;
                        break;
                    case ObstacleType.Turtle:
                        difficulty += 0.5;
                        break;
                }
            }

            return difficulty;
        }
    }
}