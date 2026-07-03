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

        DrawHelper.FillVerticalGradient(sb, new Rectangle(0, 0, w, h),
            new Color(18, 24, 44), new Color(8, 10, 18));
        DrawHelper.FillRect(sb, new Rectangle(0, h - 120, w, 120), new Color(60, 38, 16));
        DrawHelper.FillRect(sb, new Rectangle(0, h - 122, w, 4), new Color(126, 86, 38));

        DrawHelper.DrawPanel(sb, new Rectangle(w / 2 - 360, 44, 720, 160),
            new Color(20, 24, 40), new Color(90, 100, 140));
        DrawHelper.DrawPanel(sb, new Rectangle(w / 2 - 170, 230, 340, 300),
            new Color(18, 20, 32), new Color(120, 130, 170));

        string title = "GRINDERINO";
        Vector2 ts = _titleFont.MeasureString(title);
        DrawHelper.DrawTextShadow(sb, _titleFont, title,
            new Vector2(w / 2f - ts.X / 2f, 78), new Color(255, 214, 102));

        string sub = "Mine. Dig. Sell. Upgrade.";
        Vector2 ss = _font.MeasureString(sub);
        DrawHelper.DrawTextShadow(sb, _font, sub,
            new Vector2(w / 2f - ss.X / 2f, 150), new Color(190, 198, 220));

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
