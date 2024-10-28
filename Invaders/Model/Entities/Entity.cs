using Invaders.Model.Entities.Components;
using Invaders.System;
using SFML.Graphics;
using SFML.System;

namespace Invaders.Model.Entities;

public class Entity
{
    private readonly List<EntityComponent> _components;
    
    private Vector2D _position;
    private readonly int _layer;
    public int Layer => _layer;

    private readonly string _tag;
    public string Tag => _tag;

    public Vector2D Position
    {
        get => _position;
        set
        {
            _position = value;
            if (TryFindComponentsByType<RenderableComponent>(out var renderableComponents))
            {
                foreach (var component in renderableComponents)
                {
                    component.Position = value;
                }
            }

            if (TryFindComponentsByType<CollidableComponent>(out var collidableComponents))
            {
                foreach (var component in collidableComponents)
                {
                    component.Position = value;
                }
            }
        }
    }

    public Entity(string tag, int layer = 0)
    {
        _components = new List<EntityComponent>();
        _position = new Vector2D();
        _layer = layer;
        _tag = tag;
    }

    public void Update(Scene scene, float deltaTime)
    {
        if (TryFindComponentsByType<IUpdatableLogicComponent>(out var components))
        {
            foreach (var component in components)
            {
                component.Update(scene, deltaTime);
            }
        }
    }

    public void DispatchEvents(Scene scene)
    {
        if (TryFindComponentsByType<IDispatchableEventComponent>(out var components))
        {
            foreach (var component in components)
            {
                component.DispatchEvents(scene);
            }
        }
    }

    public void Render(RenderTarget target, bool isDebug = false)
    {
        if (TryFindComponentsByType<IRenderableComponent>(out var components))
        {
            foreach (var component in components)
            {
                component.Render(target);
            }
        }

        if (!isDebug)
            return;
        if (TryFindComponentsByType<CollidableComponent>(out var collidableComponents))
        {
            foreach (var component in collidableComponents)
            {
                foreach (var collider in component.Colliders)
                {
                    collider.Shape.FillColor = Color.Red;
                    target.Draw(collider.Shape);
                }
            }
        }
    }

    public void AddComponent(EntityComponent component)
    {
        _components.Add(component);
    }

    public void Instantiate(Scene scene)
    {
        if (TryFindComponentsByType<IInstantiatableComponent>(out var components))
        {
            foreach (var component in components)
            {
                component.Instantiate(scene);
            }
        }

        /*if (TryFindComponentsByType<CollidableComponent>(out var collisionPublishers))
        {
            foreach (var component in collisionPublishers)
            {
                component.Position = _position;
            }
            
            if (TryFindComponentsByType<ICollidableLogicComponent>(out var collisionSubscribers))
            {
                foreach (var subscriber in collisionSubscribers)
                {
                    foreach (var publisher in collisionPublishers)
                    {
                        publisher.Collision += subscriber.OnCollision;
                    }
                }
            }
        }

        if (TryFindComponentsByType<HealthComponent>(out var healthPublishers))
        {
            if (TryFindComponentsByType<IDamageableLogicComponent>(out var damageSubscribers))
            {
                foreach (var subscriber in damageSubscribers)
                {
                    foreach (var publisher in healthPublishers)
                    {
                        publisher.HealthLost += subscriber.OnHealthLost;
                    }
                }
            }

            if (TryFindComponentsByType<IKillableLogicComponent>(out var deathSubscribers))
            {
                foreach (var subscriber in deathSubscribers)
                {
                    foreach (var publisher in healthPublishers)
                    {
                        publisher.Death += subscriber.OnDeath;
                    }
                }
            }
        }

        if (TryFindComponentsByType<ScoreComponent>(out var scorePublishers))
        {
            if (TryFindComponentsByType<IScoreableLogicComponent>(out var scoreSubscribers))
            {
                foreach (var subscriber in scoreSubscribers)
                {
                    foreach (var publisher in scorePublishers)
                    {
                        publisher.ScoreGained += subscriber.OnScoreGained;
                    }
                }
            }
        }*/
    }

    public bool TryFindComponentsByType<T>(out List<T> components)
    {
        components = _components.OfType<T>().ToList();
        return components.Count > 0;
    }
}