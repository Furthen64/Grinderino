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

        DrawHelper.FillVerticalGradient(sb, new Rectangle(0, 0, w, h),
            new Color(24, 30, 48), new Color(12, 14, 24));
        DrawHelper.FillRect(sb, new Rectangle(0, h - 110, w, 110), new Color(70, 44, 18));

        DrawHelper.DrawPanel(sb, new Rectangle(w / 2 - 260, 42, 520, 90),
            new Color(22, 24, 36), new Color(104, 116, 156));
        DrawHelper.DrawPanel(sb, new Rectangle(w / 2 - 190, 210, 380, 280),
            new Color(18, 20, 32), new Color(112, 126, 166));

        string title = "Base Camp";
        Vector2 ts = _titleFont.MeasureString(title);
        DrawHelper.DrawTextShadow(sb, _titleFont, title,
            new Vector2(w / 2f - ts.X / 2f, 62), new Color(224, 205, 150));

        // Money display
        string money = $"$ {_game.SaveData.Money:N0}";
        DrawHelper.DrawTextShadow(sb, _font, money, new Vector2(w - 220, 20), new Color(255, 215, 0));

        // Tool display
        string tool = $"Tool: {_game.SaveData.CurrentTool.Name}";
        DrawHelper.DrawTextShadow(sb, _font, tool,
            new Vector2(w / 2f - _font.MeasureString(tool).X / 2f, 148), Color.LightBlue);

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
