using System.Numerics;
using Invaders.System;
using SFML.Graphics;
using SFML.System;

namespace Invaders.Model.Entities;

public delegate void CollisionEvent(CollidableComponent other, Scene scene);

public class CollidableComponent : EntityComponent, IDispatchableEventComponent, IInstantiatableComponent
{
    public event CollisionEvent Collision;

    private readonly List<Collider> _colliders;
    public List<Collider> Colliders => _colliders;

    public Vector2D Position
    {
        set
        {
            foreach (Collider collider in _colliders)
            {
                collider.Position = value;
            }
        }
    }

    private readonly bool _solid;
    public bool Solid => _solid;

    private readonly List<string> _tagBlacklist;
    public List<string> TagBlacklist => _tagBlacklist;
    
    private readonly List<CollidableComponent> _collisionQueue;

    public CollidableComponent(Entity entity, List<Collider> colliders, List<string> tagBlacklist, bool solid = false) : base(entity)
    {
        _colliders = colliders;
        _solid = solid;
        _tagBlacklist = tagBlacklist;
        _collisionQueue = new List<CollidableComponent>();
    }

    public CollidableComponent(Entity entity, List<Collider> colliders, bool solid = false) : base(entity)
    {
        _colliders = colliders;
        _solid = solid;
        _tagBlacklist = new List<string>();
        _collisionQueue = new List<CollidableComponent>();
    }

    public void DispatchEvents(Scene scene)
    {
        foreach (var other in _collisionQueue)
            Collision?.Invoke(other, scene);
        _collisionQueue.Clear();
    }

    public void BlacklistTag(string tag)
    {
        _tagBlacklist.Add(tag);
    }

    public bool WhitelistTag(string tag)
    {
        return _tagBlacklist.Remove(tag);
    }

    public void CheckCollision(Scene scene)
    {
        foreach (var other in scene.Entities.Where(x => x.TryFindComponentsByType<CollidableComponent>(out _)))
        {
            if (other == Entity)
                continue;
            other.TryFindComponentsByType<CollidableComponent>(out var colliders);
            var otherCollider = colliders.First();
            if (IsColliding(otherCollider))
                _collisionQueue.Add(otherCollider);
        }
    }

    public bool IsColliding(CollidableComponent other)
    {
        if (_tagBlacklist.Contains(other.Entity.Tag) || other.TagBlacklist.Contains(Entity.Tag))
            return false;
        
        foreach (var collider in _colliders)
        {
            foreach (var otherCollider in other.Colliders)
            {
                switch (collider.Shape)
                {
                    case RectangleShape rectangle:
                        switch (otherCollider.Shape)
                        {
                            case RectangleShape otherRectangle when IsColliding(rectangle, otherRectangle):
                            case CircleShape otherCircle when IsColliding(rectangle, otherCircle):
                                return true;
                        }
                        break;
                    case CircleShape circle:
                        switch (otherCollider.Shape)
                        {
                            case RectangleShape otherRectangle when IsColliding(otherRectangle, circle):
                            case CircleShape otherCircle when IsColliding(circle, otherCircle):
                                return true;
                        }
                        break;
                }
            }
        }

        return false;
    }

    private static bool IsColliding(RectangleShape rectangle, RectangleShape otherRectangle)
    {
        FloatRect rect = new FloatRect(rectangle.Position.X - rectangle.Size.X / 2, rectangle.Position.Y - rectangle.Size.Y / 2, rectangle.Size.X, rectangle.Size.Y);
        FloatRect otherRect = new FloatRect(otherRectangle.Position.X - otherRectangle.Size.X / 2, otherRectangle.Position.Y - otherRectangle.Size.Y / 2, otherRectangle.Size.X, otherRectangle.Size.Y);
        return rect.Intersects(otherRect);
    }

    private static bool IsColliding(RectangleShape rectangle, CircleShape circle)
    {
        if (((Vector2D)circle.Position - new Vector2D(rectangle.Position.X - rectangle.Size.X / 2, rectangle.Position.Y - rectangle.Size.Y / 2)).Magnitude <= circle.Radius ||
            ((Vector2D)circle.Position - new Vector2D(rectangle.Position.X + rectangle.Size.X / 2, rectangle.Position.Y - rectangle.Size.Y / 2)).Magnitude <= circle.Radius ||
            ((Vector2D)circle.Position - new Vector2D(rectangle.Position.X - rectangle.Size.X / 2, rectangle.Position.Y + rectangle.Size.Y / 2)).Magnitude <= circle.Radius ||
            ((Vector2D)circle.Position - new Vector2D(rectangle.Position.X + rectangle.Size.X / 2, rectangle.Position.Y + rectangle.Size.Y / 2)).Magnitude <= circle.Radius)
        {
            return true;
        }
        if (ContainsPoint(rectangle, circle.Position + new Vector2f(circle.Radius, 0)) ||
            ContainsPoint(rectangle, circle.Position + new Vector2f(circle.Radius * -1, 0)) ||
            ContainsPoint(rectangle, circle.Position + new Vector2f(0, circle.Radius)) ||
            ContainsPoint(rectangle, circle.Position + new Vector2f(0, circle.Radius * -1)) ||
            ContainsPoint(rectangle, circle.Position))
            return true;
            
        return false; 
    }

    private static bool IsColliding(CircleShape circle, CircleShape otherCircle)
    {
        return ((Vector2D)circle.Position - (Vector2D)otherCircle.Position).Magnitude <= circle.Radius + otherCircle.Radius;
    }

    private static bool ContainsPoint(RectangleShape rectangle, Vector2D point)
    {
        return (rectangle.Position.X - rectangle.Size.X / 2 < point.X &&
                rectangle.Position.Y - rectangle.Size.Y / 2 < point.Y &&
                rectangle.Position.X + rectangle.Size.X / 2 > point.X &&
                rectangle.Position.Y + rectangle.Size.Y / 2 > point.Y);
    }

    public void Instantiate(Scene scene)
    {
        Position = Entity.Position;
        if (Entity.TryFindComponentsByType<ICollidableLogicComponent>(out var collisionSubscribers))
        {
            foreach (var subscribers in collisionSubscribers)
            {
                Collision += subscribers.OnCollision;
            }
        }
    }
}