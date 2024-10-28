using SFML.Window;

namespace Invaders.System.Binding;

public class KeyBinding
{
    private List<Keyboard.Key> _keys;

    public bool IsPressed => _keys.Any(Keyboard.IsKeyPressed);

    public KeyBinding(List<Keyboard.Key> keys)
    {
        _keys = keys;
    }
}