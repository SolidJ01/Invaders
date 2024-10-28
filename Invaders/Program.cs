using Invaders.Model;
using Invaders.Model.Entities;
using Invaders.Model.Entities.Components;
using Invaders.System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using (var window = new RenderWindow(new VideoMode(1600, 900), "Invaders"))
{
    window.Closed += (o, e) => window.Close();
    
    window.SetView(new View(
        new Vector2f(800, 450),
        new Vector2f(1600, 900)));
    Clock clock = new Clock();

    SceneLoader sceneLoader = new SceneLoader();
    AssetManager assetManager = new AssetManager();
    Scene scene = new Scene(sceneLoader, assetManager);
    scene.SceneLoader.LoadScene(scene, "MainMenu");
    

    /*Entity debugPlayer = new Entity();
    debugPlayer.AddComponent(new RenderableComponent("Shuttle", new IntRect(0, 0, 64, 64), debugPlayer));
    debugPlayer.AddComponent(new VelocityComponent(new Vector2D(), debugPlayer));
    debugPlayer.AddComponent(new PlayerLogicComponent(debugPlayer));
    debugPlayer.AddComponent(new WeaponComponent(debugPlayer, () =>
    {
        Entity bullet = new Entity();
        bullet.AddComponent(new RenderableComponent("Spritesheet", new IntRect(64, 0, 3, 8), bullet));
        bullet.AddComponent(new VelocityComponent(new Vector2D(), bullet));
        return bullet;
    }, new Vector2D(13, 0), new Vector2D(0, -500), 0.075f));
    debugPlayer.AddComponent(new WeaponComponent(debugPlayer, () =>
    {
        Entity bullet = new Entity();
        bullet.AddComponent(new RenderableComponent("Spritesheet", new IntRect(64, 0, 3, 8), bullet));
        bullet.AddComponent(new VelocityComponent(new Vector2D(), bullet));
        return bullet;
    }, new Vector2D(-13, 0), new Vector2D(0, -500), 0.075f));
    scene.QueueSpawn(debugPlayer);*/
    
    while (window.IsOpen)
    {
        window.DispatchEvents();
        float deltaTime = clock.Restart().AsSeconds();
        deltaTime = MathF.Min(deltaTime, 0.01f);
        
        scene.Update(deltaTime);
        
        window.Clear(new Color(0,0,0));
        
        scene.Render(window);
        
        window.Display();
    }
}