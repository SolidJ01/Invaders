using Invaders.System;
using SFML.Graphics;
using SFML.System;

namespace Invaders.Model.Entities;

public class Collider
{
    private Shape _shape;
    public Shape Shape => _shape;
    private Vector2D _offset;

    public Vector2D Position
    {
        set => _shape.Position = value + _offset;
    }

    public Collider(Shape shape, Vector2D offset)
    {
        _shape = shape;
        _shape.Origin = _shape.Position + new Vector2f(_shape.GetGlobalBounds().Width / 2, _shape.GetGlobalBounds().Height / 2);
        _offset = offset;
    }
}