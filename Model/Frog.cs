using System.Drawing;

namespace OneMoreJump.Model;

public class Frog
{
    private Point _position;

    public Point Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateBounds();
        }
    }

    public int Size { get; }
    public Rectangle Bounds { get; private set; }
    public Direction FacingDirection { get; private set; }
    public int JumpCount { get; private set; }

    public Frog(Point startPosition, int size)
    {
        _position = startPosition;
        Size = size;
        FacingDirection = Direction.Up;
        JumpCount = 0;

        UpdateBounds();
    }

    private void UpdateBounds() =>
        Bounds = new Rectangle(_position.X, _position.Y, Size, Size);

    public Rectangle GetBounds() => Bounds;

    public void Move(int deltaX, int deltaY)
    {
        Position = new Point(_position.X + deltaX, _position.Y + deltaY);
        JumpCount++;
        UpdateFacingDirection(deltaX, deltaY);
    }

    private void UpdateFacingDirection(int deltaX, int deltaY)
    {
        FacingDirection = deltaY switch
        {
            < 0 => Direction.Up,
            > 0 => Direction.Down,
            _ => FacingDirection
        };

        FacingDirection = deltaX switch
        {
            < 0 => Direction.Left,
            > 0 => Direction.Right,
            _ => FacingDirection
        };
    }

    public bool IsInSafeZone(Rectangle safeZoneBounds) =>
        Bounds.IntersectsWith(safeZoneBounds);

    public bool IsOnLog(Rectangle logBounds) =>
        Bounds.IntersectsWith(logBounds);

    public void ResetJumpCount() => JumpCount = 0;

    public override string ToString() =>
        $"Frog at ({Position.X}, {Position.Y}), Size: {Size}, Facing: {FacingDirection}, Jumps: {JumpCount}";
}