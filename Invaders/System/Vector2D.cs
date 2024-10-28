using SFML.System;

namespace Invaders.System;

public struct Vector2D
{
    private float _x, _y;
    public float X => _x;
    public float Y => _y;
    
    public float Magnitude
    {
        get => MathF.Sqrt(_x * _x + _y * _y);
        set
        {
            float angle = AngleRadians;
            _x = MathF.Cos(angle) * value;
            _y = MathF.Sin(angle) * value;
        }
    }
    public float AngleRadians
    {
        get => MathF.Atan2(_y, _x);
        set
        {
            float magnitude = Magnitude;
            _x = MathF.Cos(value) * magnitude;
            _y = MathF.Sin(value) * magnitude;
        }
    }
    public float AngleDegrees
    {
        get => AngleRadians * 180 / MathF.PI;
        set => AngleRadians = value * MathF.PI / 180;
    }

    public Vector2D(float x = 0f, float y = 0f)
    {
        _x = x;
        _y = y;
    }

    public Vector2D(float xy)
    {
        _x = xy;
        _y = xy;
    }
    
    private static Vector2D Add(Vector2D v1, Vector2D v2) => new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
    private static Vector2D Subtract(Vector2D v1, Vector2D v2) => new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
    private static Vector2D Multiply(Vector2D v, float f) => new Vector2D(v.X * f, v.Y * f);
    
    public static Vector2D operator +(Vector2D v1, Vector2D v2) => Add(v1, v2);
    public static Vector2D operator -(Vector2D v1, Vector2D v2) => Subtract(v1, v2);
    public static Vector2D operator *(Vector2D v, float f) => Multiply(v, f);
    
    public static implicit operator Vector2f(Vector2D v) => new(v.X, v.Y);
    public static implicit operator Vector2D(Vector2f v) => new(v.X, v.Y);

}