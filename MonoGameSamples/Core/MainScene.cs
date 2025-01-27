using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MonoGameSamples.Core;

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

public static class SceneManager
{
    private static readonly Dictionary<string, Scene> Scenes = new();
    private static Scene _currentScene;

    public static void AddScene(string name, Scene scene)
    {
        Scenes[name] = scene;
    }

    public static void ChangeScene(string name)
    {
        if (!Scenes.TryGetValue(name, out var scene)) return;
        _currentScene = scene;
        _currentScene.Initialize();
        _currentScene.LoadContent();
    }

    public static void Update(GameTime gameTime)
    {
        _currentScene?.Update(gameTime);
    }

    public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _currentScene?.Draw(gameTime, spriteBatch);
    }
}

public class MainScene(GraphicsDeviceManager graphics, ContentManager content) : Scene(graphics, content)
{
    private TeeweeShape _tTeeweeShape;


    public override void Initialize()
    {
    }

    public override void LoadContent()
    {
        try
        {
            _tTeeweeShape = new TeeweeShape(Content);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    public override void Update(GameTime gameTime)
    {
        _tTeeweeShape?.Move(gameTime, Graphics);

        _tTeeweeShape?.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        try
        {
            spriteBatch.Begin();

            _tTeeweeShape.Draw(spriteBatch);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
        finally
        {
            spriteBatch.End();
        }
    }
}

public interface IShape
{
    void Draw(SpriteBatch spriteBatch);
}

public class Shape(Texture2D texture, Vector2 position, Rectangle sourceRectangle) : IShape
{
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, sourceRectangle, Color.White);
    }
}

public class TeeweeShape : IShape
{
    private readonly List<IShape> _shapes;

    private readonly Vector2[] _positions =
    [
        new(132, 100),
        new(100, 100),
        new(164, 100),
        new(132, 132)
    ];

    private readonly float _speed = 100;
    private float _elapsedTime;
    private readonly Texture2D _texture;
    private const int FrameWidth = 32;
    private const int FrameHeight = 32;

    public TeeweeShape(ContentManager content)
    {
        _texture = content.Load<Texture2D>("tetris_sprite");

        var sourceRectangles = GenerateSourceRectangles(FrameWidth, FrameHeight);

        _shapes = _positions.Select(position =>
            new Shape(_texture, position, sourceRectangles[0])
        ).ToList<IShape>();
    }

    public void Update(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!(_elapsedTime >= 0.1f)) return;

        for (var i = 0; i < _positions.Length; i++)
        {
            _positions[i] = new Vector2(_positions[i].X, _positions[i].Y + _speed * _elapsedTime);
        }

        RecreateShapes();

        _elapsedTime = 0f;
    }

    private void RecreateShapes()
    {
        var sourceRectangles = GenerateSourceRectangles(FrameWidth, FrameHeight);
        for (var i = 0; i < _positions.Length; i++)
        {
            _shapes[i] = new Shape(_texture, _positions[i], sourceRectangles[0]);
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(spriteBatch);
        }
    }

    public void Move(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        var delta = _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            for (var i = 0; i < _positions.Length; i++)
            {
                _positions[i] = new Vector2(_positions[i].X - delta, _positions[i].Y);
            }
        }

        if (!keyboardState.IsKeyDown(Keys.Right)) return;
        {
            for (var i = 0; i < _positions.Length; i++)
            {
                _positions[i] = new Vector2(_positions[i].X + delta, _positions[i].Y);
            }
        }

        var screenWidth = graphics.PreferredBackBufferWidth;
        var screenHeight = graphics.PreferredBackBufferHeight;

        for (var i = 0; i < _positions.Length; i++)
        {
            _positions[i] = new Vector2(
                MathHelper.Clamp(_positions[i].X, 0, screenWidth - FrameWidth),
                MathHelper.Clamp(_positions[i].Y, 0, screenHeight - FrameHeight)
            );
        }
    }

    private static Rectangle[] GenerateSourceRectangles(int frameWidth, int frameHeight)
    {
        return
        [
            new Rectangle(0 * frameWidth, 0, frameWidth, frameHeight),
            new Rectangle(1 * frameWidth, 0, frameWidth, frameHeight),
            new Rectangle(2 * frameWidth, 0, frameWidth, frameHeight),
            new Rectangle(3 * frameWidth, 0, frameWidth, frameHeight)
        ];
    }
}