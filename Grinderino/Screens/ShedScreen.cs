using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grinderino.Models;

namespace Grinderino.Screens;

public class ShedScreen : IScreen
{
    private readonly GrinderinoGame _game;
    private readonly SpriteFont _font;
    private readonly SpriteFont _titleFont;

    private Rectangle _btnSharpen;
    private Rectangle _btnHarden;
    private Rectangle _btnBuyStone;
    private Rectangle _btnBuyIron;
    private Rectangle _btnBuyGold;
    private Rectangle _btnBuyDiamond;
    private Rectangle _btnBack;

    private string _message = "";
    private double _messageTimer;

    public ShedScreen(GrinderinoGame game, SpriteFont font, SpriteFont titleFont)
    {
        _game = game;
        _font = font;
        _titleFont = titleFont;
        LayoutButtons();
    }

    private void LayoutButtons()
    {
        int leftX = 80;
        int rightX = _game.ScreenWidth / 2 + 40;
        int bw = 280, bh = 48, gap = 14;
        int upgradeY = 220;
        int shopY = 220;

        _btnSharpen = new Rectangle(leftX, upgradeY, bw, bh);
        _btnHarden  = new Rectangle(leftX, upgradeY + bh + gap, bw, bh);

        _btnBuyStone   = new Rectangle(rightX, shopY, bw, bh);
        _btnBuyIron    = new Rectangle(rightX, shopY + (bh + gap),   bw, bh);
        _btnBuyGold    = new Rectangle(rightX, shopY + (bh + gap)*2, bw, bh);
        _btnBuyDiamond = new Rectangle(rightX, shopY + (bh + gap)*3, bw, bh);

        _btnBack = new Rectangle(20, 20, 120, 40);
    }

    public void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb,
                       MouseState mouse, MouseState prevMouse)
    {
        if (_messageTimer > 0) _messageTimer -= gameTime.ElapsedGameTime.TotalSeconds;

        Tool tool = _game.SaveData.CurrentTool;

        // Sharpen blades
        if (DrawHelper.Clicked(_btnSharpen, mouse, prevMouse))
        {
            if (tool.SharpnessLevel >= tool.MaxSharpnessLevel)
                ShowMessage("Already max sharpness!");
            else if (_game.SaveData.Money < tool.UpgradeCostSharpness)
                ShowMessage($"Need ${tool.UpgradeCostSharpness} to sharpen!");
            else
            {
                _game.SaveData.Money -= tool.UpgradeCostSharpness;
                tool.SharpnessLevel++;
                ShowMessage($"Blades sharpened! Level {tool.SharpnessLevel}");
            }
        }

        // Harden steel
        if (DrawHelper.Clicked(_btnHarden, mouse, prevMouse))
        {
            if (tool.SteelLevel >= tool.MaxSteelLevel)
                ShowMessage("Already max hardness!");
            else if (_game.SaveData.Money < tool.UpgradeCostSteel)
                ShowMessage($"Need ${tool.UpgradeCostSteel} to harden steel!");
            else
            {
                _game.SaveData.Money -= tool.UpgradeCostSteel;
                tool.SteelLevel++;
                ShowMessage($"Steel hardened! Level {tool.SteelLevel}");
            }
        }

        // Buy new pickaxe
        TryBuyPickaxe(_btnBuyStone,   ToolTier.Stone,   ToolCatalogue.StonePickaxe,   mouse, prevMouse);
        TryBuyPickaxe(_btnBuyIron,    ToolTier.Iron,    ToolCatalogue.IronPickaxe,    mouse, prevMouse);
        TryBuyPickaxe(_btnBuyGold,    ToolTier.Gold,    ToolCatalogue.GoldPickaxe,    mouse, prevMouse);
        TryBuyPickaxe(_btnBuyDiamond, ToolTier.Diamond, ToolCatalogue.DiamondPickaxe, mouse, prevMouse);

        if (DrawHelper.Clicked(_btnBack, mouse, prevMouse)) _game.GoToLobby();
    }

    private void TryBuyPickaxe(Rectangle btn, ToolTier tier,
                                System.Func<Tool> factory,
                                MouseState mouse, MouseState prevMouse)
    {
        if (!DrawHelper.Clicked(btn, mouse, prevMouse)) return;
        if (_game.SaveData.CurrentTool.Tier >= tier)
        {
            ShowMessage("Already have an equal or better tool!");
            return;
        }
        int cost = ToolCatalogue.BuyCost(tier);
        if (_game.SaveData.Money < cost)
        {
            ShowMessage($"Need ${cost} to buy this pickaxe!");
            return;
        }
        _game.SaveData.Money -= cost;
        _game.SaveData.CurrentTool = factory();
        ShowMessage($"Bought {_game.SaveData.CurrentTool.Name}!");
    }

    private void ShowMessage(string msg) { _message = msg; _messageTimer = 3.0; }

    public void Draw(SpriteBatch sb)
    {
        int w = _game.ScreenWidth;
        int h = _game.ScreenHeight;

        DrawHelper.FillVerticalGradient(sb, new Rectangle(0, 0, w, h),
            new Color(34, 22, 12), new Color(14, 10, 8));
        DrawHelper.DrawPanel(sb, new Rectangle(44, 96, w - 88, h - 150),
            new Color(28, 18, 10), new Color(146, 106, 56));
        DrawHelper.DrawPanel(sb, new Rectangle(60, 130, 360, 100),
            new Color(44, 28, 12), new Color(184, 136, 76));

        string title = "Upgrade Shed";
        Vector2 ts = _titleFont.MeasureString(title);
        DrawHelper.DrawTextShadow(sb, _titleFont, title,
            new Vector2(w / 2f - ts.X / 2f, 34), new Color(244, 198, 92));

        string money = $"$ {_game.SaveData.Money:N0}";
        DrawHelper.DrawTextShadow(sb, _font, money, new Vector2(w - 220, 20), new Color(255, 215, 0));

        Tool tool = _game.SaveData.CurrentTool;

        // Current tool info panel
        Rectangle infoPanel = new Rectangle(80, 130, 340, 70);
        DrawHelper.DrawPanel(sb, infoPanel, new Color(54, 34, 12), new Color(184, 130, 60));
        sb.DrawString(_font, $"Current: {tool.Name}", new Vector2(infoPanel.X + 10, infoPanel.Y + 8), Color.White);
        sb.DrawString(_font,
            $"Power: {tool.EffectivePower}  Sharp:{tool.SharpnessLevel}/{tool.MaxSharpnessLevel}  Steel:{tool.SteelLevel}/{tool.MaxSteelLevel}",
            new Vector2(infoPanel.X + 10, infoPanel.Y + 34), Color.LightYellow);

        // Upgrade section header
        sb.DrawString(_font, "-- Upgrades --", new Vector2(80, 185), new Color(220, 180, 80));

        MouseState ms = Mouse.GetState();

        bool canSharpen = tool.SharpnessLevel < tool.MaxSharpnessLevel;
        DrawHelper.DrawButton(sb, _font, _btnSharpen,
            canSharpen ? $"Sharpen Blades  (${tool.UpgradeCostSharpness})"
                       : "Sharpen Blades  (MAX)",
            canSharpen ? new Color(40, 60, 20) : new Color(40, 40, 40),
            canSharpen ? new Color(150, 220, 80) : Color.DimGray,
            canSharpen ? Color.White : Color.Gray,
            canSharpen && DrawHelper.IsHovered(_btnSharpen, ms));

        bool canHarden = tool.SteelLevel < tool.MaxSteelLevel;
        DrawHelper.DrawButton(sb, _font, _btnHarden,
            canHarden ? $"Harden Steel  (${tool.UpgradeCostSteel})"
                      : "Harden Steel  (MAX)",
            canHarden ? new Color(40, 20, 60) : new Color(40, 40, 40),
            canHarden ? new Color(160, 80, 220) : Color.DimGray,
            canHarden ? Color.White : Color.Gray,
            canHarden && DrawHelper.IsHovered(_btnHarden, ms));

        // Shop section
        int rx = w / 2 + 40;
        sb.DrawString(_font, "-- New Pickaxes --", new Vector2(rx, 185), new Color(220, 180, 80));

        DrawShopButton(sb, ms, _btnBuyStone,   ToolTier.Stone,   ToolCatalogue.StonePickaxe(),   tool);
        DrawShopButton(sb, ms, _btnBuyIron,    ToolTier.Iron,    ToolCatalogue.IronPickaxe(),    tool);
        DrawShopButton(sb, ms, _btnBuyGold,    ToolTier.Gold,    ToolCatalogue.GoldPickaxe(),    tool);
        DrawShopButton(sb, ms, _btnBuyDiamond, ToolTier.Diamond, ToolCatalogue.DiamondPickaxe(), tool);

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

    private void DrawShopButton(SpriteBatch sb, MouseState ms, Rectangle btn,
                                 ToolTier tier, Tool proto, Tool current)
    {
        bool owned     = current.Tier >= tier;
        int  cost      = ToolCatalogue.BuyCost(tier);
        bool canAfford = _game.SaveData.Money >= cost && !owned;
        string label   = owned
            ? $"{proto.Name}  (owned)"
            : $"{proto.Name}  (${cost})";
        Color bg     = owned ? new Color(20, 40, 20) : (canAfford ? new Color(30, 50, 70) : new Color(40,40,40));
        Color border = owned ? Color.LimeGreen : (canAfford ? Color.CornflowerBlue : Color.DimGray);
        Color text   = owned ? Color.LimeGreen : (canAfford ? Color.White : Color.Gray);
        DrawHelper.DrawButton(sb, _font, btn, label, bg, border, text,
            !owned && canAfford && DrawHelper.IsHovered(btn, ms));
    }
}
