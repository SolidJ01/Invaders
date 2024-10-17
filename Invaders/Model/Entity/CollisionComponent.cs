using SFML.Graphics;

namespace Invaders.Model.Entity;

public delegate void CollisionEvent(); //TODO: Collision Data

public class CollisionComponent : EntityComponent
{
    public event CollisionEvent Collision;
    
    private readonly FloatRect _bounds;
    public FloatRect Bounds { get => _bounds; }
    
    public CollisionComponent(Entity entity) : base(entity) { }

    public void OnCollide()
    {
        Collision?.Invoke();
    }
}