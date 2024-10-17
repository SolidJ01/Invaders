namespace Invaders.Model.Entity;

public class Entity
{
    private readonly List<EntityComponent> _components;

    public Entity(List<EntityComponent> components)
    {
        _components = components;
    }

    public void AddComponent(EntityComponent component)
    {
        _components.Add(component);
    }

    public bool TryFindComponentsByType<T>(out List<T> components) where T : EntityComponent
    {
        components = _components.OfType<T>().ToList();
        return components.Count > 0;
    }
}