namespace Invaders.Model.Entities;

public abstract class EntityComponent
{
    private readonly Entity _this;
    public Entity Entity { get => _this; }

    protected EntityComponent(Entity entity)
    {
        _this = entity;
    }
}