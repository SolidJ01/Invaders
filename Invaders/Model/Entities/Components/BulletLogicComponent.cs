namespace Invaders.Model.Entities.Components;

public class BulletLogicComponent : EntityComponent, ICollidableLogicComponent
{
    private readonly float _damage;
    
    public BulletLogicComponent(float damage, Entity entity) : base(entity)
    {
        _damage = damage;
    }

    public void OnCollision(CollidableComponent other, Scene scene)
    {
        if (other.Entity.TryFindComponentsByType<HealthComponent>(out var components))
        {
            var healthComponent = components.First();
            healthComponent.LoseHealth(_damage);
        }
        if (other.Solid)
            scene.QueueDespawn(Entity);
    }
}