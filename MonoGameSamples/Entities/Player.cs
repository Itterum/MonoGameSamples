using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSamples.Entities;

internal class Player
{
    public Texture2D Texture { get; set; }

    public Vector2 Position { get; set; }

    public float Speed { get; init; } = 100f;

    private bool _isFlipped;

    public void Move(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        var delta = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Up))
        {
            Position = new Vector2(Position.X, Position.Y - delta);
        }

        if (keyboardState.IsKeyDown(Keys.Down))
        {
            Position = new Vector2(Position.X, Position.Y + delta);
        }

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            Position = new Vector2(Position.X - delta, Position.Y);
            _isFlipped = true;
        }

        if (keyboardState.IsKeyDown(Keys.Right))
        {
            Position = new Vector2(Position.X + delta, Position.Y);
            _isFlipped = false;
        }

        var screenWidth = graphics.PreferredBackBufferWidth;
        var screenHeight = graphics.PreferredBackBufferHeight;
        var textureWidth = Texture.Width;
        var textureHeight = Texture.Height;

        Position = new Vector2(
            MathHelper.Clamp(Position.X, 0, screenWidth - textureWidth),
            MathHelper.Clamp(Position.Y, 0, screenHeight - textureHeight)
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var spriteEffects = _isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        spriteBatch.Draw(Texture,
            Position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            .5f,
            spriteEffects,
            0f);
    }
}