using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grinderino.Models;

namespace Grinderino.Screens;

public class LobbyScreen : IScreen
{
    private readonly GrinderinoGame _game;
    private readonly SpriteFont _font;
    private readonly SpriteFont _titleFont;

    private Rectangle _btnShed;
    private Rectangle _btnMarket;
    private Rectangle _btnMine;
    private Rectangle _btnBack;

    public LobbyScreen(GrinderinoGame game, SpriteFont font, SpriteFont titleFont)
    {
        _game = game;
        _font = font;
        _titleFont = titleFont;
        LayoutButtons();
    }

    private void LayoutButtons()
    {
        int cx = _game.ScreenWidth / 2;
        int startY = 240;
        int bw = 300, bh = 60, gap = 24;

        _btnShed   = new Rectangle(cx - bw / 2, startY,               bw, bh);
        _btnMarket = new Rectangle(cx - bw / 2, startY + bh + gap,    bw, bh);
        _btnMine   = new Rectangle(cx - bw / 2, startY + (bh+gap)*2,  bw, bh);
        _btnBack   = new Rectangle(20, 20, 120, 40);
    }

    public void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb,
                       MouseState mouse, MouseState prevMouse)
    {
        if (DrawHelper.Clicked(_btnShed,   mouse, prevMouse)) _game.GoToShed();
        if (DrawHelper.Clicked(_btnMarket, mouse, prevMouse)) _game.GoToMarket();
        if (DrawHelper.Clicked(_btnMine,   mouse, prevMouse)) _game.GoToMining();
        if (DrawHelper.Clicked(_btnBack,   mouse, prevMouse)) _game.GoToMainMenu();
    }

    public void Draw(SpriteBatch sb)
    {
        int w = _game.ScreenWidth;
        int h = _game.ScreenHeight;

        DrawHelper.FillRect(sb, new Rectangle(0, 0, w, h), new Color(20, 25, 40));

        // Ground
        DrawHelper.FillRect(sb, new Rectangle(0, h - 100, w, 100), new Color(80, 50, 20));
        // Sky gradient hint
        for (int i = 0; i < 80; i++)
            DrawHelper.FillRect(sb, new Rectangle(0, i, w, 1),
                new Color(20 + i / 3, 25 + i / 3, 60 + i));

        string title = "Base Camp";
        Vector2 ts = _titleFont.MeasureString(title);
        sb.DrawString(_titleFont, title,
            new Vector2(w / 2f - ts.X / 2f, 60),
            new Color(220, 200, 150));

        // Money display
        string money = $"$ {_game.SaveData.Money:N0}";
        sb.DrawString(_font, money, new Vector2(w - 220, 20), new Color(255, 215, 0));

        // Tool display
        string tool = $"Tool: {_game.SaveData.CurrentTool.Name}";
        sb.DrawString(_font, tool, new Vector2(w / 2f - _font.MeasureString(tool).X / 2f, 150),
            Color.LightBlue);

        MouseState ms = Mouse.GetState();
        DrawHelper.DrawButton(sb, _font, _btnShed,
            "Upgrade Shed", new Color(60, 40, 20), new Color(180, 130, 60),
            Color.White, DrawHelper.IsHovered(_btnShed, ms));

        DrawHelper.DrawButton(sb, _font, _btnMarket,
            "$ Market", new Color(20, 60, 20), Color.LimeGreen,
            Color.White, DrawHelper.IsHovered(_btnMarket, ms));

        DrawHelper.DrawButton(sb, _font, _btnMine,
            "Go Mining!", new Color(80, 40, 0), new Color(200, 100, 20),
            Color.White, DrawHelper.IsHovered(_btnMine, ms));

        DrawHelper.DrawButton(sb, _font, _btnBack,
            "< Menu", new Color(30, 30, 40), Color.SlateBlue,
            Color.White, DrawHelper.IsHovered(_btnBack, ms));
    }
}
