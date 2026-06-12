using Microsoft.Xna.Framework;

namespace Grinderino.Models;

public class Player
{
    public const int Width  = 24;
    public const int Height = 32;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }

    public bool IsOnGround { get; set; }
    public bool FacingRight { get; set; } = true;

    public float MineProgress { get; set; }
    public float MineProgressMax { get; set; } = 1f;
    public int   MiningBlockX { get; set; } = -1;
    public int   MiningBlockY { get; set; } = -1;

    public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

    public Player(Vector2 startPos)
    {
        Position = startPos;
        Velocity = Vector2.Zero;
    }
}
