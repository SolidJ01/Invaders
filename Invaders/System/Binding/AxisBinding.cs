namespace Invaders.System.Binding;

public class AxisBinding
{
    private KeyBinding _min;
    private KeyBinding _max;

    private int _minValue => _min.IsPressed ? -1 : 0;
    private int _maxValue => _max.IsPressed ? 1 : 0;
    
    public float State => _minValue + _maxValue;

    public AxisBinding(KeyBinding min, KeyBinding max)
    {
        _min = min;
        _max = max;
    }
}