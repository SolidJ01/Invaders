namespace Invaders.Model;

public class MenuItem
{
    private string _title;
    public string Title => _title;
    private Action<Scene> _action;

    public MenuItem(string title, Action<Scene> action)
    {
        _title = title;
        _action = action;
    }

    public void Select(Scene scene)
    {
        _action(scene);
    }
}