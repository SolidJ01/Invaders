using SFML.Graphics;
using SFML.System;
using SFML.Window;

using (var window = new RenderWindow(new VideoMode(1600, 900), "Invaders"))
{
    window.Closed += (o, e) => window.Close();
    // TODO: Initialize
    window.SetView(new View(
        new Vector2f(800, 450),
        new Vector2f(1600, 900)));
    Clock clock = new Clock();
    
    while (window.IsOpen)
    {
        window.DispatchEvents();
        float deltaTime = clock.Restart().AsSeconds();
        deltaTime = MathF.Min(deltaTime, 0.01f);
        
        // TODO: Updates
        
        window.Clear(new Color(223, 246, 245));
        
        // TODO: Drawing
        
        window.Display();
    }
}