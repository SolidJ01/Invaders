namespace Invaders.Model.Entity;

public abstract class EntityComponent
{
    protected readonly Entity _this;

    protected EntityComponent(Entity entity)
    {
        _this = entity;
    }
}