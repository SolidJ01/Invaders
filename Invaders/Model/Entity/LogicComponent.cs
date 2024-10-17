namespace Invaders.Model.Entity;

public abstract class LogicComponent : EntityComponent
{
    protected LogicComponent(Entity entity) : base(entity)
    {
        
    }

    public abstract void Update(float deltaTime);
}