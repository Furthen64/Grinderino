using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grinderino.Models;

namespace Grinderino.Screens;

public class MainMenuScreen : IScreen
{
    private readonly GrinderinoGame _game;
    private readonly SpriteFont _font;
    private readonly SpriteFont _titleFont;

    private Rectangle _btnNewRun;
    private Rectangle _btnContinue;
    private Rectangle _btnGraphs;
    private Rectangle _btnExit;

    public MainMenuScreen(GrinderinoGame game, SpriteFont font, SpriteFont titleFont)
    {
        _game = game;
        _font = font;
        _titleFont = titleFont;
        LayoutButtons();
    }

    private void LayoutButtons()
    {
        int cx = _game.ScreenWidth / 2;
        int startY = 260;
        int bw = 280, bh = 50, gap = 20;

        _btnNewRun   = new Rectangle(cx - bw / 2, startY,             bw, bh);
        _btnContinue = new Rectangle(cx - bw / 2, startY + bh + gap,  bw, bh);
        _btnGraphs   = new Rectangle(cx - bw / 2, startY + (bh+gap)*2, bw, bh);
        _btnExit     = new Rectangle(cx - bw / 2, startY + (bh+gap)*3, bw, bh);
    }

    public void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb,
                       MouseState mouse, MouseState prevMouse)
    {
        if (DrawHelper.Clicked(_btnNewRun, mouse, prevMouse))
        {
            _game.StartNewRun();
        }
        else if (DrawHelper.Clicked(_btnContinue, mouse, prevMouse) && _game.SaveData.HasSave)
        {
            _game.GoToLobby();
        }
        else if (DrawHelper.Clicked(_btnGraphs, mouse, prevMouse))
        {
            _game.GoToGraphs();
        }
        else if (DrawHelper.Clicked(_btnExit, mouse, prevMouse))
        {
            _game.Exit();
        }
    }

    public void Draw(SpriteBatch sb)
    {
        int w = _game.ScreenWidth;
        int h = _game.ScreenHeight;

        DrawHelper.FillRect(sb, new Rectangle(0, 0, w, h), new Color(15, 20, 35));

        // Decorative ground strip
        DrawHelper.FillRect(sb, new Rectangle(0, h - 80, w, 80), new Color(80, 50, 20));
        DrawHelper.FillRect(sb, new Rectangle(0, h - 80, w, 4), new Color(100, 70, 30));

        // Title
        string title = "GRINDERINO";
        Vector2 ts = _titleFont.MeasureString(title);
        sb.DrawString(_titleFont, title,
            new Vector2(w / 2f - ts.X / 2f, 80),
            new Color(255, 215, 0));

        string sub = "Mine. Dig. Sell. Upgrade.";
        Vector2 ss = _font.MeasureString(sub);
        sb.DrawString(_font, sub,
            new Vector2(w / 2f - ss.X / 2f, 160),
            new Color(180, 180, 200));

        MouseState ms = Mouse.GetState();
        bool canContinue = _game.SaveData.HasSave;

        DrawHelper.DrawButton(sb, _font, _btnNewRun, "New Run",
            new Color(30, 80, 30), Color.LimeGreen, Color.White,
            DrawHelper.IsHovered(_btnNewRun, ms));

        Color contBg     = canContinue ? new Color(30, 60, 100) : new Color(40, 40, 40);
        Color contBorder = canContinue ? Color.CornflowerBlue   : Color.DimGray;
        Color contText   = canContinue ? Color.White            : Color.DimGray;
        DrawHelper.DrawButton(sb, _font, _btnContinue, "Continue",
            contBg, contBorder, contText,
            canContinue && DrawHelper.IsHovered(_btnContinue, ms));

        DrawHelper.DrawButton(sb, _font, _btnGraphs, "Graphs",
            new Color(60, 30, 80), Color.MediumPurple, Color.White,
            DrawHelper.IsHovered(_btnGraphs, ms));

        DrawHelper.DrawButton(sb, _font, _btnExit, "Exit",
            new Color(80, 20, 20), Color.IndianRed, Color.White,
            DrawHelper.IsHovered(_btnExit, ms));
    }
}
