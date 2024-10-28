namespace Invaders.Model.Entities.Components;

public class TriggerVolumeLogicComponent : EntityComponent, ICollidableLogicComponent
{
    private Action<Entity, Scene> _action;

    private readonly bool _triggerOnce;
    private bool _triggered;
    
    public TriggerVolumeLogicComponent(Action<Entity, Scene> action, Entity entity, bool triggerOnce = false) : base(entity)
    {
        _action = action;
        _triggerOnce = triggerOnce;
        _triggered = false;
    }

    public void OnCollision(CollidableComponent other, Scene scene)
    {
        if (_triggerOnce && _triggered)
            return;
        
        _action(other.Entity, scene);
        
        if (_triggerOnce)
            _triggered = true;
    }
}