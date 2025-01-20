using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSamples;

public abstract class Scene(GraphicsDeviceManager graphics, ContentManager content)
{
    protected readonly GraphicsDeviceManager Graphics = graphics;
    protected readonly ContentManager Content = content;

    public virtual void Initialize()
    {
    }

    public virtual void LoadContent()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
    }
}