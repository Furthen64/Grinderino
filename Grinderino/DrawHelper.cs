using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grinderino;

/// <summary>Simple helpers for drawing coloured rectangles and text.</summary>
public static class DrawHelper
{
    private static Texture2D _pixel;

    public static void Init(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public static void FillRect(SpriteBatch sb, Rectangle rect, Color color)
    {
        sb.Draw(_pixel, rect, color);
    }

    public static void FillVerticalGradient(SpriteBatch sb, Rectangle rect, Color top, Color bottom)
    {
        if (rect.Height <= 0) return;
        for (int y = 0; y < rect.Height; y++)
        {
            float t = rect.Height == 1 ? 0f : y / (float)(rect.Height - 1);
            FillRect(sb, new Rectangle(rect.X, rect.Y + y, rect.Width, 1), Color.Lerp(top, bottom, t));
        }
    }

    public static void DrawRect(SpriteBatch sb, Rectangle rect, Color color, int thickness = 1)
    {
        FillRect(sb, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        FillRect(sb, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
        FillRect(sb, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        FillRect(sb, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
    }

    public static void DrawButton(SpriteBatch sb, SpriteFont font, Rectangle rect,
                                  string label, Color bg, Color border, Color textColor,
                                  bool hovered = false)
    {
        Color fill = hovered ? Lighten(bg) : bg;
        FillRect(sb, new Rectangle(rect.X + 3, rect.Y + 4, rect.Width, rect.Height), new Color(0, 0, 0, 70));
        FillRect(sb, rect, fill);
        DrawRect(sb, rect, border, 2);
        FillRect(sb, new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, 2), new Color(255, 255, 255, hovered ? 70 : 35));
        Vector2 size = font.MeasureString(label);
        Vector2 pos  = new Vector2(rect.X + (rect.Width - size.X) / 2f,
                                   rect.Y + (rect.Height - size.Y) / 2f);
        sb.DrawString(font, label, pos + new Vector2(1, 1), new Color(0, 0, 0, 120));
        sb.DrawString(font, label, pos, textColor);
    }

    public static void DrawPanel(SpriteBatch sb, Rectangle rect, Color fill, Color border, int borderThickness = 2)
    {
        FillRect(sb, new Rectangle(rect.X + 4, rect.Y + 5, rect.Width, rect.Height), new Color(0, 0, 0, 70));
        FillRect(sb, rect, fill);
        DrawRect(sb, rect, border, borderThickness);
        FillRect(sb, new Rectangle(rect.X + borderThickness, rect.Y + borderThickness, rect.Width - borderThickness * 2, 2),
            new Color(255, 255, 255, 24));
    }

    public static void DrawTextShadow(SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color)
    {
        sb.DrawString(font, text, position + new Vector2(1, 1), new Color(0, 0, 0, 140));
        sb.DrawString(font, text, position, color);
    }

    private static Color Lighten(Color c) =>
        new Color(
            System.Math.Min(255, c.R + 40),
            System.Math.Min(255, c.G + 40),
            System.Math.Min(255, c.B + 40));

    public static bool IsHovered(Rectangle rect, Microsoft.Xna.Framework.Input.MouseState ms) =>
        rect.Contains(ms.X, ms.Y);

    public static bool Clicked(Rectangle rect,
                               Microsoft.Xna.Framework.Input.MouseState ms,
                               Microsoft.Xna.Framework.Input.MouseState prev) =>
        rect.Contains(ms.X, ms.Y) &&
        ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
        prev.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
}
