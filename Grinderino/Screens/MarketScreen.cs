using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grinderino.Models;

namespace Grinderino.Screens;

public class MarketScreen : IScreen
{
    private readonly GrinderinoGame _game;
    private readonly SpriteFont _font;
    private readonly SpriteFont _titleFont;

    private Rectangle _btnSellAll;
    private Rectangle _btnBack;
    private Rectangle _btnMetalDetector;

    private readonly List<(BlockType type, Rectangle btn)> _sellButtons = new();

    private string _message = "";
    private double _messageTimer;

    private const int MetalDetectorCost = 250;

    public MarketScreen(GrinderinoGame game, SpriteFont font, SpriteFont titleFont)
    {
        _game = game;
        _font = font;
        _titleFont = titleFont;
    }

    private void Layout()
    {
        _sellButtons.Clear();
        int startX = 80, startY = 200, bw = 240, bh = 40, gap = 10;
        int idx = 0;
        foreach (var kv in _game.SaveData.Inventory.Items)
        {
            if (kv.Value > 0 && BlockData.GetValue(kv.Key) > 0)
            {
                _sellButtons.Add((kv.Key,
                    new Rectangle(startX, startY + idx * (bh + gap), bw, bh)));
                idx++;
            }
        }

        _btnSellAll = new Rectangle(80, startY + idx * (bh + gap) + 20, 260, 48);
        _btnMetalDetector = new Rectangle(_game.ScreenWidth - 340, 200, 300, 48);
        _btnBack = new Rectangle(20, 20, 120, 40);
    }

    public void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb,
                       MouseState mouse, MouseState prevMouse)
    {
        Layout();

        if (_messageTimer > 0) _messageTimer -= gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var (type, btn) in _sellButtons)
        {
            if (DrawHelper.Clicked(btn, mouse, prevMouse))
            {
                int earned = _game.SaveData.Inventory.SellItem(type, 1);
                _game.SaveData.Money += earned;
                ShowMessage($"+${earned} for {BlockData.GetName(type)}");
            }
        }

        if (DrawHelper.Clicked(_btnSellAll, mouse, prevMouse))
        {
            int earned = _game.SaveData.Inventory.SellAll();
            _game.SaveData.Money += earned;
            ShowMessage($"Sold everything for ${earned}!");
        }

        if (!_game.SaveData.HasMetalDetector &&
            DrawHelper.Clicked(_btnMetalDetector, mouse, prevMouse))
        {
            if (_game.SaveData.Money >= MetalDetectorCost)
            {
                _game.SaveData.Money -= MetalDetectorCost;
                _game.SaveData.HasMetalDetector = true;
                ShowMessage("Metal Detector purchased!");
            }
            else
            {
                ShowMessage($"Not enough money! Need ${MetalDetectorCost}");
            }
        }

        if (DrawHelper.Clicked(_btnBack, mouse, prevMouse)) _game.GoToLobby();
    }

    private void ShowMessage(string msg) { _message = msg; _messageTimer = 3.0; }

    public void Draw(SpriteBatch sb)
    {
        int w = _game.ScreenWidth;
        int h = _game.ScreenHeight;

        DrawHelper.FillRect(sb, new Rectangle(0, 0, w, h), new Color(15, 30, 15));

        string title = "Market";
        Vector2 ts = _titleFont.MeasureString(title);
        sb.DrawString(_titleFont, title,
            new Vector2(w / 2f - ts.X / 2f, 30), new Color(100, 220, 100));

        string money = $"$ {_game.SaveData.Money:N0}";
        sb.DrawString(_font, money, new Vector2(w - 220, 20), new Color(255, 215, 0));

        sb.DrawString(_font, "Your Inventory:", new Vector2(80, 160), Color.LightGreen);

        MouseState ms = Mouse.GetState();
        var inv = _game.SaveData.Inventory.Items;

        if (!inv.Any(kv => kv.Value > 0 && BlockData.GetValue(kv.Key) > 0))
        {
            sb.DrawString(_font, "(nothing to sell)", new Vector2(80, 210), Color.Gray);
        }
        else
        {
            foreach (var (type, btn) in _sellButtons)
            {
                int count = _game.SaveData.Inventory.Count(type);
                string label = $"Sell {BlockData.GetName(type)} x{count}  (${BlockData.GetValue(type)} ea)";
                DrawHelper.DrawButton(sb, _font, btn, label,
                    BlockData.GetColor(type) * 0.5f, BlockData.GetColor(type),
                    Color.White, DrawHelper.IsHovered(btn, ms));
            }

            DrawHelper.DrawButton(sb, _font, _btnSellAll, "Sell ALL",
                new Color(80, 20, 20), Color.IndianRed, Color.White,
                DrawHelper.IsHovered(_btnSellAll, ms));
        }

        // Metal detector shop
        DrawHelper.FillRect(sb, new Rectangle(_game.ScreenWidth - 360, 160,
            340, _game.SaveData.HasMetalDetector ? 120 : 160), new Color(30, 30, 60));
        DrawHelper.DrawRect(sb, new Rectangle(_game.ScreenWidth - 360, 160, 340,
            _game.SaveData.HasMetalDetector ? 120 : 160), Color.SlateBlue, 2);

        sb.DrawString(_font, "Equipment Shop",
            new Vector2(_game.ScreenWidth - 350, 170), Color.LightBlue);

        if (_game.SaveData.HasMetalDetector)
        {
            sb.DrawString(_font, "Metal Detector owned",
                new Vector2(_game.ScreenWidth - 350, 210), Color.LimeGreen);
        }
        else
        {
            DrawHelper.DrawButton(sb, _font, _btnMetalDetector,
                $"Metal Detector  (${MetalDetectorCost})",
                new Color(20, 40, 80), Color.CornflowerBlue, Color.White,
                DrawHelper.IsHovered(_btnMetalDetector, ms));
            sb.DrawString(_font, "Reveals buried artifacts!",
                new Vector2(_game.ScreenWidth - 350, 260), Color.LightBlue);
        }

        if (_messageTimer > 0)
        {
            float alpha = (float)(_messageTimer / 3.0);
            sb.DrawString(_font, _message,
                new Vector2(w / 2f - _font.MeasureString(_message).X / 2f, h - 80),
                Color.Yellow * alpha);
        }

        DrawHelper.DrawButton(sb, _font, _btnBack, "< Back",
            new Color(30, 30, 40), Color.SlateBlue, Color.White,
            DrawHelper.IsHovered(_btnBack, ms));
    }
}
