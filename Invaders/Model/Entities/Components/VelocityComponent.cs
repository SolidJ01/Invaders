using System.Numerics;
using Invaders.System;
using SFML.System;

namespace Invaders.Model.Entities.Components;

public class VelocityComponent : EntityComponent, IUpdatableLogicComponent
{
    private Vector2D _velocity;

    public Vector2D Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }
    
    public VelocityComponent(Entity entity) : base(entity)
    {
        _velocity = new Vector2D();
    }

    public VelocityComponent(Vector2D velocity, Entity entity) : base(entity)
    {
        _velocity = velocity;
    }

    public virtual void Update(Scene scene, float deltaTime)
    {
        Entity.Position += _velocity * deltaTime;
    }
}