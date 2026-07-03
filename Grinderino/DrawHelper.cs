using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grinderino;

/// <summary>Simple helpers for drawing coloured rectangles and text.</summary>
public static class DrawHelper
{
    private static Texture2D _pixel;
    private static Texture2D _noise;
    private const int NoiseSize = 64;

    public static void Init(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _noise = BuildNoiseTexture(gd);
    }

    /// <summary>Builds a small tile of grimy, grainy noise used to give flat blocks
    /// a rough, gritty look instead of a smooth/shiny fill.</summary>
    private static Texture2D BuildNoiseTexture(GraphicsDevice gd)
    {
        var tex = new Texture2D(gd, NoiseSize, NoiseSize);
        var data = new Color[NoiseSize * NoiseSize];
        var rng = new System.Random(20240609); // fixed seed => stable grit pattern

        for (int i = 0; i < data.Length; i++)
        {
            int roll = rng.Next(100);
            if (roll < 48)
            {
                data[i] = Color.Transparent;
            }
            else if (roll < 85)
            {
                // dark grime speck
                data[i] = new Color(0, 0, 0, rng.Next(20, 90));
            }
            else
            {
                // faint dust/scratch highlight (kept subtle - no shine)
                data[i] = new Color(255, 255, 255, rng.Next(6, 26));
            }
        }

        tex.SetData(data);
        return tex;
    }

    public static void FillRect(SpriteBatch sb, Rectangle rect, Color color)
    {
        sb.Draw(_pixel, rect, color);
    }

    /// <summary>Draws a filled rectangle with a gritty, textured overlay derived from
    /// deterministic noise so each tile looks weathered instead of flat/shiny.</summary>
    public static void FillRectGritty(SpriteBatch sb, Rectangle rect, Color color, int seedX, int seedY)
    {
        FillRect(sb, rect, color);

        if (rect.Width <= 0 || rect.Height <= 0) return;

        int maxOffX = System.Math.Max(1, NoiseSize - rect.Width);
        int maxOffY = System.Math.Max(1, NoiseSize - rect.Height);
        int offX = Mod(seedX * 13 + seedY * 7, maxOffX);
        int offY = Mod(seedX * 5 + seedY * 19, maxOffY);

        int srcW = System.Math.Min(rect.Width, NoiseSize);
        int srcH = System.Math.Min(rect.Height, NoiseSize);
        var src = new Rectangle(offX, offY, srcW, srcH);
        sb.Draw(_noise, rect, src, Color.White);

        // subtle grounded shadow along the bottom edge instead of a bright gloss line
        FillRect(sb, new Rectangle(rect.X, rect.Bottom - 3, rect.Width, 3), new Color(0, 0, 0, 55));
    }

    private static int Mod(int value, int modulus)
    {
        int r = value % modulus;
        return r < 0 ? r + modulus : r;
    }

    public static void FillVerticalGradient(SpriteBatch sb, Rectangle rect, Color top, Color bottom)
    {
        if (rect.Height <= 0) return;
        const int bandHeight = 4;
        for (int y = 0; y < rect.Height; y += bandHeight)
        {
            int h = System.Math.Min(bandHeight, rect.Height - y);
            float t = rect.Height == 1 ? 0f : (y + h / 2f) / (rect.Height - 1);
            FillRect(sb, new Rectangle(rect.X, rect.Y + y, rect.Width, h), Color.Lerp(top, bottom, t));
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
        FillRectGritty(sb, rect, fill, rect.X, rect.Y);
        DrawRect(sb, rect, border, 2);
        FillRect(sb, new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, 2), new Color(0, 0, 0, hovered ? 30 : 45));
        Vector2 size = font.MeasureString(label);
        Vector2 pos  = new Vector2(rect.X + (rect.Width - size.X) / 2f,
                                   rect.Y + (rect.Height - size.Y) / 2f);
        sb.DrawString(font, label, pos + new Vector2(1, 1), new Color(0, 0, 0, 120));
        sb.DrawString(font, label, pos, textColor);
    }

    public static void DrawPanel(SpriteBatch sb, Rectangle rect, Color fill, Color border, int borderThickness = 2)
    {
        FillRect(sb, new Rectangle(rect.X + 4, rect.Y + 5, rect.Width, rect.Height), new Color(0, 0, 0, 70));
        FillRectGritty(sb, rect, fill, rect.X, rect.Y);
        DrawRect(sb, rect, border, borderThickness);
        FillRect(sb, new Rectangle(rect.X + borderThickness, rect.Y + borderThickness, rect.Width - borderThickness * 2, 2),
            new Color(0, 0, 0, 40));
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
