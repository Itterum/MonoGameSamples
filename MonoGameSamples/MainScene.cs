using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSamples;

public class MainScene(GraphicsDeviceManager graphics, ContentManager content) : Scene(graphics, content)
{
    private Ball _ball;

    public override void Initialize()
    {
        _ball = new Ball
        {
            Position = new Vector2(200, 200),
            Speed = 100f
        };

        base.Initialize();
    }

    public override void LoadContent()
    {
        try
        {
            _ball.Texture = Content.Load<Texture2D>("ball");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading ball texture: {ex.Message}");
        }

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (_ball.Texture != null)
        {
            _ball.Move(gameTime, Graphics);
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        if (_ball.Texture != null)
        {
            spriteBatch.Draw(_ball.Texture, _ball.Position, Color.White);
        }
        else
        {
            Console.WriteLine("Ball texture is not loaded.");
        }

        spriteBatch.End();

        base.Draw(gameTime, spriteBatch);
    }
}

internal class Ball
{
    public Texture2D Texture { get; set; }

    public Vector2 Position { get; set; }

    public float Speed { get; init; } = 100f;

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
        }

        if (keyboardState.IsKeyDown(Keys.Right))
        {
            Position = new Vector2(Position.X + delta, Position.Y);
        }

        var halfWidth = Texture.Width / 2f;
        var halfHeight = Texture.Height / 2f;

        Position = new Vector2(
            Math.Clamp(Position.X, halfWidth, graphics.PreferredBackBufferWidth - halfWidth),
            Math.Clamp(Position.Y, halfHeight, graphics.PreferredBackBufferHeight - halfHeight)
        );
    }
}