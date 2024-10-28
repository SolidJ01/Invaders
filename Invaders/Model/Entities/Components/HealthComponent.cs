namespace Invaders.Model.Entities.Components;

public delegate void HealthLostEvent(float health);

public delegate void DeathEvent(HealthComponent healthComponent);

public class HealthComponent : EntityComponent, IUpdatableLogicComponent, IInstantiatableComponent
{
    public event HealthLostEvent HealthLost;
    public event DeathEvent Death;
    
    private float _health;
    public float Health => _health;
    private readonly float _maxHealth;
    public float MaxHealth => _maxHealth;

    public HealthComponent(Entity entity, float health) : base(entity)
    {
        _health = health;
        _maxHealth = health;
    }

    public void LoseHealth(float damage)
    {
        _health -= damage;
        HealthLost?.Invoke(_health);
    }

    public void Update(Scene scene, float deltaTime)
    {
        if (_health <= 0)
        {
            scene.QueueDespawn(Entity);
            Death?.Invoke(this);
        }
    }

    public void Instantiate(Scene scene)
    {
        if (Entity.TryFindComponentsByType<IDamageableLogicComponent>(out var damageSubscribers))
        {
            foreach (var damageSubscriber in damageSubscribers)
                HealthLost += damageSubscriber.OnHealthLost;
        }

        if (Entity.TryFindComponentsByType<IKillableLogicComponent>(out var deathSubscribers))
        {
            foreach (var subscriber in deathSubscribers)
            {
                Death += subscriber.OnDeath;
            }
        }
    }
}