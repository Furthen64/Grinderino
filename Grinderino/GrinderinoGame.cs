using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grinderino.Models;
using Grinderino.Screens;

namespace Grinderino;

/// <summary>
/// Core game class. Manages screens, save data, and navigation between views.
/// </summary>
public class GrinderinoGame : Game
{
    // ── Graphics ─────────────────────────────────────────────────────────────
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // ── Fonts ────────────────────────────────────────────────────────────────
    private SpriteFont _font;
    private SpriteFont _titleFont;

    // ── Input ────────────────────────────────────────────────────────────────
    private KeyboardState _kb, _prevKb;
    private MouseState    _mouse, _prevMouse;

    // ── Screens ──────────────────────────────────────────────────────────────
    private IScreen _currentScreen;
    private MainMenuScreen _mainMenuScreen;
    private LobbyScreen    _lobbyScreen;
    private MiningScreen   _miningScreen;
    private ShedScreen     _shedScreen;
    private MarketScreen   _marketScreen;
    private GraphsScreen   _graphsScreen;

    // ── Save data ────────────────────────────────────────────────────────────
    public SaveData SaveData { get; private set; } = new SaveData();

    // ── Window dimensions ────────────────────────────────────────────────────
    public int ScreenWidth  => _graphics.PreferredBackBufferWidth;
    public int ScreenHeight => _graphics.PreferredBackBufferHeight;

    public GrinderinoGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth  = 1280,
            PreferredBackBufferHeight = 720
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Grinderino";
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        DrawHelper.Init(GraphicsDevice);

        _font      = Content.Load<SpriteFont>("DefaultFont");
        _titleFont = Content.Load<SpriteFont>("DefaultFont"); // reuse; scale via DrawString if desired

        // Build screens
        _mainMenuScreen = new MainMenuScreen(this, _font, _titleFont);
        _lobbyScreen    = new LobbyScreen(this, _font, _titleFont);
        _miningScreen   = new MiningScreen(this, _font, _titleFont);
        _shedScreen     = new ShedScreen(this, _font, _titleFont);
        _marketScreen   = new MarketScreen(this, _font, _titleFont);
        _graphsScreen   = new GraphsScreen(this, _font, _titleFont);

        _currentScreen = _mainMenuScreen;
    }

    protected override void Update(GameTime gameTime)
    {
        _prevKb    = _kb;
        _prevMouse = _mouse;
        _kb        = Keyboard.GetState();
        _mouse     = Mouse.GetState();

        _currentScreen?.Update(gameTime, _kb, _prevKb, _mouse, _prevMouse);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        _currentScreen?.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    // ── Navigation helpers ────────────────────────────────────────────────────
    public void GoToMainMenu() => _currentScreen = _mainMenuScreen;
    public void GoToLobby()    => _currentScreen = _lobbyScreen;
    public void GoToShed()     => _currentScreen = _shedScreen;
    public void GoToMarket()   => _currentScreen = _marketScreen;
    public void GoToGraphs()   => _currentScreen = _graphsScreen;

    public void GoToMining()
    {
        _miningScreen.Reset();
        _currentScreen = _miningScreen;
    }

    public void StartNewRun()
    {
        SaveData = SaveData.New();
        GoToLobby();
    }
}
