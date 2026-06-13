using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grinderino.Models;

namespace Grinderino.Screens;

public class MiningScreen : IScreen
{
    // ── Physics constants ────────────────────────────────────────────────────
    private const float Gravity    = 900f;
    private const float JumpSpeed  = -420f;
    private const float MoveSpeed  = 160f;

    // ── Camera ───────────────────────────────────────────────────────────────
    private Vector2 _camera;

    // ── State ────────────────────────────────────────────────────────────────
    private readonly GrinderinoGame _game;
    private readonly SpriteFont    _font;
    private readonly SpriteFont    _titleFont;
    private World  _world;
    private Player _player;
    private RunStats _runStats;

    // ── Mining state ─────────────────────────────────────────────────────────
    private float  _mineTimer;
    private float  _mineNeeded;
    private int    _mineBlockX = -1, _mineBlockY = -1;

    // ── UI ───────────────────────────────────────────────────────────────────
    private Rectangle _btnSurface;
    private string _notification = "";
    private double _notifTimer;

    public MiningScreen(GrinderinoGame game, SpriteFont font, SpriteFont titleFont)
    {
        _game = game;
        _font = font;
        _titleFont = titleFont;
        _btnSurface = new Rectangle(game.ScreenWidth - 160, 10, 150, 36);
        Reset();
    }

    public void Reset()
    {
        _world = new World();
        int spawnX = (World.WorldWidth / 2) * World.BlockSize + (World.BlockSize - Player.Width) / 2;
        int spawnY = 0;
        // Find top of surface
        for (int y = 0; y < World.WorldHeight; y++)
        {
            if (_world.GetBlock(World.WorldWidth / 2, y) != BlockType.Air)
            {
                spawnY = (y - 1) * World.BlockSize - Player.Height;
                break;
            }
        }
        _player = new Player(new Vector2(spawnX, spawnY));
        _runStats = new RunStats
        {
            RunNumber = _game.SaveData.RunHistory.Count + 1,
            Date = DateTime.Now
        };
        _mineBlockX = _mineBlockY = -1;
        _mineTimer  = 0;
        _notifTimer = 0;
        _camera = Vector2.Zero;
    }

    // ── Update ───────────────────────────────────────────────────────────────
    public void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb,
                       MouseState mouse, MouseState prevMouse)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_notifTimer > 0) _notifTimer -= dt;

        HandleInput(kb, prevKb, dt);
        ApplyPhysics(dt);
        UpdateCamera();
        HandleMining(kb, prevKb, dt);

        // Return to surface button
        if (DrawHelper.Clicked(_btnSurface, mouse, prevMouse))
            FinishRun();
    }

    // ── Input & movement ─────────────────────────────────────────────────────
    private void HandleInput(KeyboardState kb, KeyboardState prevKb, float dt)
    {
        Vector2 vel = _player.Velocity;

        // Horizontal
        bool left  = kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left);
        bool right = kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right);
        vel.X = 0;
        if (left)  { vel.X = -MoveSpeed; _player.FacingRight = false; }
        if (right) { vel.X =  MoveSpeed; _player.FacingRight = true;  }

        // Jump
        bool jumpPressed = (kb.IsKeyDown(Keys.W)    || kb.IsKeyDown(Keys.Up) ||
                            kb.IsKeyDown(Keys.Space)) &&
                           !(prevKb.IsKeyDown(Keys.W)    || prevKb.IsKeyDown(Keys.Up) ||
                             prevKb.IsKeyDown(Keys.Space));
        if (jumpPressed && _player.IsOnGround)
            vel.Y = JumpSpeed;

        _player.Velocity = vel;
    }

    // ── Physics ──────────────────────────────────────────────────────────────
    private void ApplyPhysics(float dt)
    {
        Vector2 vel = _player.Velocity;
        vel.Y += Gravity * dt;
        _player.Velocity = vel;

        Vector2 pos = _player.Position + vel * dt;
        _player.IsOnGround = false;

        // Horizontal collision
        pos = ResolveHorizontal(pos);
        // Vertical collision
        pos = ResolveVertical(pos);

        _player.Position = pos;
    }

    private Vector2 ResolveHorizontal(Vector2 pos)
    {
        Rectangle bounds = new Rectangle((int)pos.X, (int)_player.Position.Y,
                                          Player.Width, Player.Height);
        int left   = bounds.Left   / World.BlockSize;
        int right  = (bounds.Right - 1) / World.BlockSize;
        int top    = bounds.Top    / World.BlockSize;
        int bottom = (bounds.Bottom - 1) / World.BlockSize;

        for (int y = top; y <= bottom; y++)
        {
            if (_player.Velocity.X < 0)
            {
                if (IsSolid(left, y))
                {
                    pos.X = (left + 1) * World.BlockSize;
                    _player.Velocity = new Vector2(0, _player.Velocity.Y);
                    break;
                }
            }
            else if (_player.Velocity.X > 0)
            {
                if (IsSolid(right, y))
                {
                    pos.X = right * World.BlockSize - Player.Width;
                    _player.Velocity = new Vector2(0, _player.Velocity.Y);
                    break;
                }
            }
        }
        return pos;
    }

    private Vector2 ResolveVertical(Vector2 pos)
    {
        Rectangle bounds = new Rectangle((int)_player.Position.X, (int)pos.Y,
                                          Player.Width, Player.Height);
        int left   = bounds.Left   / World.BlockSize;
        int right  = (bounds.Right - 1) / World.BlockSize;
        int top    = bounds.Top    / World.BlockSize;
        int bottom = (bounds.Bottom - 1) / World.BlockSize;

        for (int x = left; x <= right; x++)
        {
            if (_player.Velocity.Y < 0)
            {
                if (IsSolid(x, top))
                {
                    pos.Y = (top + 1) * World.BlockSize;
                    _player.Velocity = new Vector2(_player.Velocity.X, 0);
                    break;
                }
            }
            else if (_player.Velocity.Y > 0)
            {
                if (IsSolid(x, bottom))
                {
                    pos.Y = bottom * World.BlockSize - Player.Height;
                    _player.Velocity = new Vector2(_player.Velocity.X, 0);
                    _player.IsOnGround = true;
                    break;
                }
            }
        }
        return pos;
    }

    private bool IsSolid(int bx, int by)
    {
        BlockType t = _world.GetBlock(bx, by);
        return t != BlockType.Air;
    }

    // ── Camera ───────────────────────────────────────────────────────────────
    private void UpdateCamera()
    {
        int sw = _game.ScreenWidth;
        int sh = _game.ScreenHeight;
        float targetX = _player.Position.X + Player.Width / 2f - sw / 2f;
        float targetY = _player.Position.Y + Player.Height / 2f - sh / 2f;
        targetX = MathHelper.Clamp(targetX, 0,
                                   World.WorldWidth * World.BlockSize - sw);
        targetY = MathHelper.Clamp(targetY, 0,
                                   World.WorldHeight * World.BlockSize - sh);
        _camera = Vector2.Lerp(_camera, new Vector2(targetX, targetY), 0.12f);
    }

    // ── Mining ───────────────────────────────────────────────────────────────
    private void HandleMining(KeyboardState kb, KeyboardState prevKb, float dt)
    {
        bool mineKey = kb.IsKeyDown(Keys.Z) || kb.IsKeyDown(Keys.E) ||
                       kb.IsKeyDown(Keys.LeftControl);

        // Determine target block
        int bx = (int)(_player.Position.X + Player.Width / 2f) / World.BlockSize;
        int by;
        bool diggingDown = kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down);
        if (diggingDown)
            by = (int)(_player.Position.Y + Player.Height + 1) / World.BlockSize;
        else
            by = (int)(_player.Position.Y + Player.Height / 2f) / World.BlockSize;

        // Side mining
        if (!diggingDown)
        {
            int side = _player.FacingRight ? bx + 1 : bx - 1;
            BlockType sideBlock = _world.GetBlock(side, by);
            if (sideBlock != BlockType.Air && BlockData.IsMineable(sideBlock))
                bx = side;
            else
            {
                // Mine block in front at chest height
                bx = (int)(_player.Position.X + (_player.FacingRight ? Player.Width + 2 : -2))
                     / World.BlockSize;
                by = (int)(_player.Position.Y + Player.Height / 2f) / World.BlockSize;
            }
        }

        if (!mineKey)
        {
            _mineBlockX = -1;
            _mineBlockY = -1;
            _mineTimer  = 0;
            return;
        }

        BlockType target = _world.GetBlock(bx, by);
        if (!BlockData.IsMineable(target)) return;

        // Hardness check
        Tool tool = _game.SaveData.CurrentTool;
        if (BlockData.GetHardness(target) > tool.EffectivePower)
        {
            ShowNotification("Need a better tool!");
            return;
        }

        // Reset if different block
        if (_mineBlockX != bx || _mineBlockY != by)
        {
            _mineBlockX = bx;
            _mineBlockY = by;
            _mineTimer  = 0;
            _mineNeeded = (float)BlockData.GetHardness(target) / (tool.MineSpeed * 2f);
        }

        _mineTimer += dt;
        if (_mineTimer >= _mineNeeded)
        {
            // Collect block
            CollectBlock(bx, by, target);
            _world.SetBlock(bx, by, BlockType.Air);
            _mineBlockX = -1;
            _mineBlockY = -1;
            _mineTimer  = 0;
        }
    }

    private void CollectBlock(int bx, int by, BlockType type)
    {
        _runStats.BlocksMined++;
        int depth = by; // block y = depth in blocks
        if (depth > _runStats.MaxDepthReached) _runStats.MaxDepthReached = depth;

        if (type == BlockType.Treasure)
        {
            if (!_game.SaveData.HasMetalDetector)
            {
                ShowNotification("Found something... (need Metal Detector!)");
                return;
            }
            _game.SaveData.Inventory.Add(type);
            _runStats.ArtifactsFound++;
            ShowNotification($"Artifact found! (+${BlockData.GetValue(type)})");
        }
        else if (BlockData.GetValue(type) > 0)
        {
            _game.SaveData.Inventory.Add(type);
            _runStats.OresFound++;
        }
    }

    private void ShowNotification(string msg)
    {
        _notification = msg;
        _notifTimer   = 2.5;
    }

    private void FinishRun()
    {
        _game.SaveData.RunHistory.Add(_runStats);
        _game.GoToLobby();
    }

    // ── Draw ─────────────────────────────────────────────────────────────────
    public void Draw(SpriteBatch sb)
    {
        int sw = _game.ScreenWidth;
        int sh = _game.ScreenHeight;

        // Sky
        DrawHelper.FillRect(sb, new Rectangle(0, 0, sw, sh), new Color(100, 160, 220));

        // Calculate visible tile range
        int camX = (int)_camera.X;
        int camY = (int)_camera.Y;

        int startTileX = camX / World.BlockSize;
        int startTileY = camY / World.BlockSize;
        int endTileX = startTileX + sw / World.BlockSize + 2;
        int endTileY = startTileY + sh / World.BlockSize + 2;

        startTileX = Math.Max(0, startTileX);
        startTileY = Math.Max(0, startTileY);
        endTileX   = Math.Min(World.WorldWidth - 1,  endTileX);
        endTileY   = Math.Min(World.WorldHeight - 1, endTileY);

        // Underground background
        int undergroundScreenY = Math.Max(0, -camY);
        if (undergroundScreenY < sh)
            DrawHelper.FillRect(sb, new Rectangle(0, undergroundScreenY, sw, sh - undergroundScreenY),
                new Color(30, 20, 10));

        for (int bx = startTileX; bx <= endTileX; bx++)
        {
            for (int by = startTileY; by <= endTileY; by++)
            {
                BlockType t = _world.GetBlock(bx, by);
                if (t == BlockType.Air) continue;

                int sx = bx * World.BlockSize - camX;
                int sy = by * World.BlockSize - camY;
                Rectangle screenRect = new Rectangle(sx, sy, World.BlockSize, World.BlockSize);

                // Hidden treasures only visible with metal detector
                if (t == BlockType.Treasure && !_game.SaveData.HasMetalDetector)
                {
                    // Draw as stone, undetected
                    DrawHelper.FillRect(sb, screenRect, BlockData.GetColor(BlockType.Stone));
                    DrawHelper.DrawRect(sb, screenRect, new Color(60, 60, 60), 1);
                    continue;
                }

                DrawHelper.FillRect(sb, screenRect, BlockData.GetColor(t));
                DrawHelper.DrawRect(sb, screenRect, new Color(0, 0, 0, 80), 1);

                // Ore label
                if (t == BlockType.CoalOre || t == BlockType.IronOre ||
                    t == BlockType.GoldOre  || t == BlockType.DiamondOre ||
                    t == BlockType.Treasure)
                {
                    string ore = t switch
                    {
                        BlockType.CoalOre    => "C",
                        BlockType.IronOre    => "Fe",
                        BlockType.GoldOre    => "Au",
                        BlockType.DiamondOre => "*",
                        BlockType.Treasure   => "!",
                        _                    => ""
                    };
                    Vector2 oreSize = _font.MeasureString(ore);
                    sb.DrawString(_font, ore,
                        new Vector2(sx + (World.BlockSize - oreSize.X) / 2f,
                                    sy + (World.BlockSize - oreSize.Y) / 2f),
                        Color.White);
                }

                // Mining progress overlay
                if (_mineBlockX == bx && _mineBlockY == by && _mineNeeded > 0)
                {
                    float pct = _mineTimer / _mineNeeded;
                    int oh = (int)(World.BlockSize * pct);
                    DrawHelper.FillRect(sb,
                        new Rectangle(sx, sy + World.BlockSize - oh, World.BlockSize, oh),
                        new Color(255, 255, 255, 80));
                    DrawHelper.FillRect(sb,
                        new Rectangle(sx, sy + World.BlockSize - 4, (int)(World.BlockSize * pct), 4),
                        Color.Yellow);
                }
            }
        }

        // Draw player
        int px = (int)_player.Position.X - camX;
        int py = (int)_player.Position.Y - camY;
        DrawHelper.FillRect(sb, new Rectangle(px, py, Player.Width, Player.Height),
            new Color(80, 160, 240));
        // Eyes
        int eyeX = _player.FacingRight ? px + 14 : px + 6;
        DrawHelper.FillRect(sb, new Rectangle(eyeX, py + 6, 5, 5), Color.White);
        DrawHelper.FillRect(sb, new Rectangle(eyeX + 1, py + 7, 3, 3), Color.DarkBlue);
        // Hat
        DrawHelper.FillRect(sb, new Rectangle(px + 2, py - 6, Player.Width - 4, 8), new Color(80, 50, 20));
        // Lamp on hat if deep enough
        int depthBlocks = (int)(_player.Position.Y / World.BlockSize);
        if (depthBlocks > 10)
        {
            DrawHelper.FillRect(sb, new Rectangle(
                _player.FacingRight ? px + Player.Width - 6 : px, py - 5, 6, 4),
                new Color(255, 240, 100));
        }

        DrawHUD(sb, sw, sh);
    }

    private void DrawHUD(SpriteBatch sb, int sw, int sh)
    {
        // Top bar
        DrawHelper.FillRect(sb, new Rectangle(0, 0, sw, 48), new Color(0, 0, 0, 160));

        int depthBlocks = (int)(_player.Position.Y / World.BlockSize);
        int depthMeters = depthBlocks * 2;
        sb.DrawString(_font, $"Depth: {depthMeters}m", new Vector2(10, 14), Color.Cyan);
        sb.DrawString(_font, $"$ {_game.SaveData.Money:N0}", new Vector2(200, 14), new Color(255, 215, 0));

        Tool tool = _game.SaveData.CurrentTool;
        sb.DrawString(_font, $"Tool: {tool.Name}  Pwr:{tool.EffectivePower}",
            new Vector2(sw / 2f - 100, 14), Color.LightBlue);

        if (_game.SaveData.HasMetalDetector)
            sb.DrawString(_font, "[Metal Detector]", new Vector2(sw - 300, 14), new Color(100, 220, 255));

        // Inventory summary
        sb.DrawString(_font, $"Bag: {GetInventorySummary()}",
            new Vector2(10, sh - 30), Color.White);

        // Controls hint
        sb.DrawString(_font, "A/D:Move  W/Space:Jump  Z:Mine  S:Dig Down",
            new Vector2(sw / 2f - 200, sh - 30), new Color(180, 180, 180));

        // Return button
        MouseState ms = Mouse.GetState();
        DrawHelper.DrawButton(sb, _font, _btnSurface, "^ Surface",
            new Color(60, 40, 10), new Color(200, 140, 50),
            Color.White, DrawHelper.IsHovered(_btnSurface, ms));

        // Notification
        if (_notifTimer > 0)
        {
            float alpha = (float)(_notifTimer / 2.5);
            Vector2 ns = _font.MeasureString(_notification);
            DrawHelper.FillRect(sb, new Rectangle(
                (int)(sw / 2f - ns.X / 2f - 10), 60, (int)ns.X + 20, 28),
                new Color(0, 0, 0, (int)(180 * alpha)));
            sb.DrawString(_font, _notification,
                new Vector2(sw / 2f - ns.X / 2f, 64), Color.Yellow * alpha);
        }
    }

    private string GetInventorySummary()
    {
        var items = _game.SaveData.Inventory.Items;
        if (items.Count == 0) return "(empty)";
        var parts = new System.Text.StringBuilder();
        foreach (var kv in items)
            if (kv.Value > 0) parts.Append($"{BlockData.GetName(kv.Key)}x{kv.Value}  ");
        return parts.ToString().TrimEnd();
    }
}
