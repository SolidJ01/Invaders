using Invaders.System;
using Color = SFML.Graphics.Color;

namespace Invaders.Model.Entities.Components;

public class EnemyLogicComponent : EntityComponent, IUpdatableLogicComponent, ICollidableLogicComponent, IDamageableLogicComponent, IKillableLogicComponent
{
    private Random random;
    private readonly int _minSalvoLength;
    private readonly int _maxSalvoLength;
    private readonly float _salvoCheckInterval;
    private readonly float _salvoChance;
    private readonly float _damageBlinkLength;
    
    private float _currentSalvoLength;
    private float _currentSalvoLengthTarget;
    private float _timeSinceSalvoCheck;
    private float _currentDamageBlinkLength;
    
    private bool _firing;
    
    public EnemyLogicComponent(Entity entity, int minSalvoLength = 1, int maxSalvoLength = 6, float salvoCheckInterval = 2.5f, float salvoChance = 0.75f, float damageBlinkLength = 0.1f) : base(entity)
    {
        random = new Random();
        _minSalvoLength = minSalvoLength;
        _maxSalvoLength = maxSalvoLength;
        _salvoCheckInterval = salvoCheckInterval;
        _salvoChance = salvoChance;
        _damageBlinkLength = damageBlinkLength;
        
        _currentSalvoLength = 0;
        _currentSalvoLengthTarget = 0;
        _timeSinceSalvoCheck = 0;
        _currentDamageBlinkLength = 0;
        _firing = false;
    }

    public void Update(Scene scene, float deltaTime)
    {
        if (!_firing)
        {
            _timeSinceSalvoCheck += deltaTime;
            if (_timeSinceSalvoCheck >= _salvoCheckInterval)
            {
                _firing = random.NextDouble() < _salvoChance;
                _timeSinceSalvoCheck = 0;

                if (_firing)
                {
                    _currentSalvoLengthTarget = random.Next(_minSalvoLength, _maxSalvoLength + 1);
                }
            }
        }

        if (_firing)
        {

            if (_currentSalvoLength >= _currentSalvoLengthTarget)
            {
                _firing = false;
                _currentSalvoLength = 0;
            }
            
            if (Entity.TryFindComponentsByType<WeaponComponent>(out var components))
            {
                foreach (var component in components)
                {
                    component.Firing = _firing;
                }
            }
            if (_firing)
                _currentSalvoLength += deltaTime;
        }

        if (_currentDamageBlinkLength > 0)
        {
            _currentDamageBlinkLength -= deltaTime;
            if (_currentDamageBlinkLength <= 0 && Entity.TryFindComponentsByType<RenderableComponent>(out var components))
                components.First().SetColor(Color.White);
        }
    }

    public void OnCollision(CollidableComponent other, Scene scene)
    {
        VelocityComponent velocityComponent = null;
        if (Entity.TryFindComponentsByType<VelocityComponent>(out var components))
            velocityComponent = components.First();

        if (velocityComponent is not null && other.Solid)
        {
            velocityComponent.Velocity = new Vector2D(velocityComponent.Velocity.X * -1, velocityComponent.Velocity.Y);
        }
        
        if (other.Entity.TryFindComponentsByType<HealthComponent>(out var healthComponents))
        {
            HealthComponent otherHealthComponent = healthComponents.First();
            otherHealthComponent.LoseHealth(100);
        }
    }

    public void OnHealthLost(float health)
    {
        _currentDamageBlinkLength = _damageBlinkLength;
        if (Entity.TryFindComponentsByType<RenderableComponent>(out var components))
            components.First().SetColor(Color.Red);
    }

    public void OnDeath(HealthComponent healthComponent)
    {
        //TODO: trigger explosion particle effect
    }
}