﻿using System;
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

    public virtual void UnloadContent()
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

        _currentScene?.UnloadContent();
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
    private TeeweeShape _teeweeShape;


    public override void Initialize()
    {
    }

    public override void LoadContent()
    {
        try
        {
            _teeweeShape = new TeeweeShape(Content);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (_teeweeShape == null) return;

        _teeweeShape.Move(gameTime, Graphics);
        _teeweeShape.Update(gameTime, Graphics);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        try
        {
            spriteBatch.Begin();

            _teeweeShape?.Draw(spriteBatch);
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

    public override void UnloadContent()
    {
        _teeweeShape?.Dispose();
    }
}

public interface IShape : IDisposable
{
    void Draw(SpriteBatch spriteBatch);
    void Update(GameTime gameTime, GraphicsDeviceManager graphics);
}

public class Shape(Texture2D texture, Vector2 position, Rectangle sourceRectangle) : IShape
{
    private Texture2D Texture { get; } = texture;
    public Vector2 Position { get; set; } = position; // Добавлен сеттер
    private Rectangle SourceRectangle { get; } = sourceRectangle;

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, SourceRectangle, Color.White);
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
    }

    public void Dispose()
    {
        Texture.Dispose();
    }
}

public class TeeweeShape : IShape
{
    private readonly List<IShape> _shapes;
    private Vector2[] _positions;
    private readonly float _speed = 200f;
    private float _elapsedTime;
    private const int FrameWidth = 32;
    private const int FrameHeight = 32;
    private bool _isOnFloor;

    public TeeweeShape(ContentManager content)
    {
        var texture = content.Load<Texture2D>("tetris_sprite");
        var sourceRectangles = GenerateSourceRectangles(FrameWidth, FrameHeight);

        _positions =
        [
            new Vector2(132, 100),
            new Vector2(100, 100),
            new Vector2(164, 100),
            new Vector2(132, 132)
        ];

        _shapes = _positions.Select(position =>
            new Shape(texture, position, sourceRectangles[0])
        ).ToList<IShape>();
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        if (_isOnFloor)
        {
            ResetPositions();
            _isOnFloor = false;
        }

        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_elapsedTime < 0.1f) return;

        MoveShapes(gameTime, graphics);
        RecreateShapes();

        _isOnFloor = _positions.Any(position => position.Y + FrameHeight >= graphics.PreferredBackBufferHeight);

        _elapsedTime = 0f;
    }

    private void ResetPositions()
    {
        _positions =
        [
            new Vector2(132, 100),
            new Vector2(100, 100),
            new Vector2(164, 100),
            new Vector2(132, 132)
        ];
    }

    private void MoveShapes(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        var delta = _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyboardState = Keyboard.GetState();

        for (var i = 0; i < _positions.Length; i++)
        {
            var newPosition = _positions[i];

            if (keyboardState.IsKeyDown(Keys.Left))
                newPosition.X -= delta;
            else if (keyboardState.IsKeyDown(Keys.Right))
                newPosition.X += delta;

            newPosition.Y += delta;

            newPosition.X = MathHelper.Clamp(newPosition.X, 0, graphics.PreferredBackBufferWidth - FrameWidth);
            newPosition.Y = MathHelper.Clamp(newPosition.Y, 0, graphics.PreferredBackBufferHeight - FrameHeight);

            _positions[i] = newPosition;
        }
    }

    private void RecreateShapes()
    {
        for (var i = 0; i < _positions.Length; i++)
        {
            if (_shapes[i] is Shape shape)
            {
                shape.Position = _positions[i];
            }
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
        Update(gameTime, graphics);
    }

    private static Rectangle[] GenerateSourceRectangles(int frameWidth, int frameHeight)
    {
        return Enumerable.Range(0, 4).Select(i =>
            new Rectangle(i * frameWidth, 0, frameWidth, frameHeight)
        ).ToArray();
    }

    public void Dispose()
    {
        foreach (var shape in _shapes)
        {
            shape.Dispose();
        }
    }
}