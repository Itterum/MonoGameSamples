using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSamples;

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