using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grinderino.Models;

namespace Grinderino.Screens;

public class GraphsScreen : IScreen
{
    private readonly GrinderinoGame _game;
    private readonly SpriteFont _font;
    private readonly SpriteFont _titleFont;
    private Rectangle _btnBack;

    public GraphsScreen(GrinderinoGame game, SpriteFont font, SpriteFont titleFont)
    {
        _game = game;
        _font = font;
        _titleFont = titleFont;
        _btnBack = new Rectangle(20, 20, 120, 40);
    }

    public void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb,
                       MouseState mouse, MouseState prevMouse)
    {
        if (DrawHelper.Clicked(_btnBack, mouse, prevMouse)) _game.GoToMainMenu();
    }

    public void Draw(SpriteBatch sb)
    {
        int w = _game.ScreenWidth;
        int h = _game.ScreenHeight;

        DrawHelper.FillRect(sb, new Rectangle(0, 0, w, h), new Color(15, 15, 30));

        string title = "Run History";
        Vector2 ts = _titleFont.MeasureString(title);
        sb.DrawString(_titleFont, title,
            new Vector2(w / 2f - ts.X / 2f, 30), new Color(160, 100, 255));

        var history = _game.SaveData.RunHistory;

        if (history.Count == 0)
        {
            string msg = "No runs yet — go dig something!";
            Vector2 ms2 = _font.MeasureString(msg);
            sb.DrawString(_font, msg,
                new Vector2(w / 2f - ms2.X / 2f, h / 2f - 20), Color.Gray);
        }
        else
        {
            // Column headers
            int tableX = 60, tableY = 110, rowH = 30;
            sb.DrawString(_font, "#",       new Vector2(tableX,       tableY), Color.LightGray);
            sb.DrawString(_font, "Date",    new Vector2(tableX + 50,  tableY), Color.LightGray);
            sb.DrawString(_font, "Mined",   new Vector2(tableX + 220, tableY), Color.LightGray);
            sb.DrawString(_font, "Ores",    new Vector2(tableX + 310, tableY), Color.LightGray);
            sb.DrawString(_font, "Artifacts",new Vector2(tableX + 390, tableY), Color.LightGray);
            sb.DrawString(_font, "Earned",  new Vector2(tableX + 490, tableY), Color.LightGray);
            sb.DrawString(_font, "Depth",   new Vector2(tableX + 600, tableY), Color.LightGray);

            DrawHelper.FillRect(sb, new Rectangle(tableX, tableY + 24, w - 80, 1), Color.DimGray);

            int maxRows = (h - 200) / rowH;
            int startIdx = System.Math.Max(0, history.Count - maxRows);

            for (int i = startIdx; i < history.Count; i++)
            {
                var run = history[i];
                int y = tableY + rowH + (i - startIdx) * rowH;
                Color rc = i % 2 == 0 ? new Color(25, 25, 45) : Color.Transparent;
                DrawHelper.FillRect(sb, new Rectangle(tableX, y, w - 80, rowH - 2), rc);

                sb.DrawString(_font, run.RunNumber.ToString(),        new Vector2(tableX,       y + 6), Color.White);
                sb.DrawString(_font, run.Date.ToString("MM/dd"),      new Vector2(tableX + 50,  y + 6), Color.White);
                sb.DrawString(_font, run.BlocksMined.ToString(),      new Vector2(tableX + 220, y + 6), Color.White);
                sb.DrawString(_font, run.OresFound.ToString(),        new Vector2(tableX + 310, y + 6), new Color(255, 215, 0));
                sb.DrawString(_font, run.ArtifactsFound.ToString(),   new Vector2(tableX + 390, y + 6), new Color(100, 220, 255));
                sb.DrawString(_font, $"${run.MoneyEarned}",           new Vector2(tableX + 490, y + 6), Color.LimeGreen);
                sb.DrawString(_font, $"{run.MaxDepthReached}m",       new Vector2(tableX + 600, y + 6), Color.Cyan);
            }

            // Bar chart for money earned
            DrawBarChart(sb, history, w, h);
        }

        MouseState ms = Mouse.GetState();
        DrawHelper.DrawButton(sb, _font, _btnBack, "← Menu",
            new Color(30, 30, 40), Color.SlateBlue, Color.White,
            DrawHelper.IsHovered(_btnBack, ms));
    }

    private void DrawBarChart(SpriteBatch sb, List<RunStats> history, int w, int h)
    {
        if (history.Count < 2) return;

        int chartX = 60, chartY = h - 180, chartW = w - 120, chartH = 120;
        DrawHelper.FillRect(sb, new Rectangle(chartX, chartY, chartW, chartH),
            new Color(20, 20, 35));
        DrawHelper.DrawRect(sb, new Rectangle(chartX, chartY, chartW, chartH), Color.DimGray);

        sb.DrawString(_font, "Money Earned per Run:",
            new Vector2(chartX, chartY - 22), Color.LightGray);

        int maxMoney = 1;
        foreach (var r in history) if (r.MoneyEarned > maxMoney) maxMoney = r.MoneyEarned;

        int barW = (chartW - 20) / history.Count;
        for (int i = 0; i < history.Count; i++)
        {
            int bh = (int)((float)history[i].MoneyEarned / maxMoney * (chartH - 20));
            int bx = chartX + 10 + i * barW;
            int by = chartY + chartH - bh - 4;
            DrawHelper.FillRect(sb, new Rectangle(bx, by, System.Math.Max(2, barW - 2), bh),
                new Color(60, 200, 100));
        }
    }
}
