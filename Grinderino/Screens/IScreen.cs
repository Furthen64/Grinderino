using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Grinderino.Screens;

public interface IScreen
{
    void Update(GameTime gameTime, KeyboardState kb, KeyboardState prevKb,
                MouseState mouse, MouseState prevMouse);
    void Draw(SpriteBatch sb);
}
