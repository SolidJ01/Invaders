using SFML.System;

namespace Invaders.System.Binding;

public class AxisBinding2D
{
    private AxisBinding _horizontal;
    private AxisBinding _vertical;

    public Vector2D State
    {
        get
        {
            Vector2D vector = new Vector2D(_horizontal.State, _vertical.State);
            if (vector.Magnitude > 1f)
            {
                vector.Magnitude = 1;
                // float angle = MathF.Atan2(vector.Y, vector.X);
                // vector = new Vector2f(MathF.Cos(angle), MathF.Sin(angle));
            }

            return vector;
        }
    }

    public AxisBinding2D(AxisBinding horizontal, AxisBinding vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }
}