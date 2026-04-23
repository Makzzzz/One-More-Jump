using System.Drawing;

namespace OneMoreJump.Model
{
    public class Obstacle
    {
        public Rectangle Bounds { get; private set; }

        public ObstacleType Type { get; }
        public int Speed { get; }

        public bool IsActive { get; }

        public int Id { get; }

        private static int _nextId = 1;

        public Obstacle(Rectangle bounds, ObstacleType type, int speed)
        {
            Bounds = bounds;
            Type = type;
            Speed = speed;
            IsActive = true;
            Id = _nextId++;
        }

        public Obstacle(Rectangle bounds, string typeName, int speed)
        {
            Bounds = bounds;
            Speed = speed;
            IsActive = true;
            Id = _nextId++;

            if (Enum.TryParse(typeName, out ObstacleType parsedType))
            {
                Type = parsedType;
            }
            else
            {
                Type = ObstacleType.Car;
            }
        }

        public void Update(int gameWidth)
        {
            if (!IsActive) return;

            var newBounds = Bounds;
            newBounds.X += Speed;

            if (Speed > 0 && newBounds.X > gameWidth)
            {
                newBounds.X = -newBounds.Width;
            }
            else if (Speed < 0 && newBounds.X < -newBounds.Width)
            {
                newBounds.X = gameWidth;
            }

            Bounds = newBounds;
        }

        public bool CheckCollision(Rectangle otherBounds) =>
            IsActive && Bounds.IntersectsWith(otherBounds);

        public override string ToString()
        {
            return $"{Type} #{Id} at ({Bounds.X}, {Bounds.Y}), Speed: {Speed}, Active: {IsActive}";
        }
    }
}