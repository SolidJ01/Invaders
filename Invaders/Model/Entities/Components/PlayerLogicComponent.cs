using Invaders.System;
using Invaders.System.Binding;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Invaders.Model.Entities.Components;

public class PlayerLogicComponent : EntityComponent,  IUpdatableLogicComponent, ICollidableLogicComponent, IDamageableLogicComponent
{

    private AxisBinding2D _movementBinding;
    private KeyBinding _fireBinding;

    private readonly float _maxSpeed;
    private float _speed;
    private readonly float _acceleration;
    private readonly float _deceleration;
    private float _lastDeltaTime;

    private readonly float _invulnerabilityGracePeriod;
    private float _currentInvulnerabilityPeriod;

    private float _timeSinceScoreGain;
    
    public PlayerLogicComponent(Entity entity, float maxSpeed = 300f, float acceleration = 200f, float deceleration = 25f, float invulnerabilityGracePeriod = 3f) : base(entity)
    {
        _movementBinding = new AxisBinding2D(
            new AxisBinding(
                new KeyBinding([Keyboard.Key.A, Keyboard.Key.Left]),
                new KeyBinding([Keyboard.Key.D, Keyboard.Key.Right])
            ), new AxisBinding(
                new KeyBinding([Keyboard.Key.W, Keyboard.Key.Up]),
                new KeyBinding([Keyboard.Key.S, Keyboard.Key.Down])
            )
        );
        _fireBinding = new KeyBinding([Keyboard.Key.Space, Keyboard.Key.Numpad0]);

        _maxSpeed = maxSpeed;
        _speed = 0;
        _acceleration = acceleration;
        _deceleration = deceleration;
        
        _invulnerabilityGracePeriod = invulnerabilityGracePeriod;
        _currentInvulnerabilityPeriod = 0;
        
        _timeSinceScoreGain = 0;
    }

    public void Update(Scene scene, float deltaTime)
    {
        VelocityComponent? velocityComponent = null;
        if (Entity.TryFindComponentsByType<VelocityComponent>(out var velocityComponents))
            velocityComponent = velocityComponents.First();

        Vector2D acceleration = _movementBinding.State;
        acceleration.Magnitude = _acceleration * acceleration.Magnitude;

        if (velocityComponent is not null)
        {
            velocityComponent.Velocity.Magnitude = MathF.Max(velocityComponent.Velocity.Magnitude - _deceleration * deltaTime, 0);
            velocityComponent.Velocity += acceleration * deltaTime;
            velocityComponent.Velocity.Magnitude = MathF.Min(velocityComponent.Velocity.Magnitude, _maxSpeed);
        }

        if (Entity.TryFindComponentsByType<WeaponComponent>(out var components))
        {
            foreach (var component in components)
            {
                component.Firing = _fireBinding.IsPressed;
            }
        }

        if (_currentInvulnerabilityPeriod > 0)
        {
            _currentInvulnerabilityPeriod -= deltaTime;

            if (_currentInvulnerabilityPeriod <= 0)
            {
                if (Entity.TryFindComponentsByType<CollidableComponent>(out var colliders))
                {
                    var component = colliders.First();
                    component.WhitelistTag("enemy");
                    component.WhitelistTag("bullet");
                }
                if (Entity.TryFindComponentsByType<RenderableComponent>(out var renderableComponents))
                {
                    var component = renderableComponents.First();
                    component.SetColor(Color.White);
                }
            }
        }

        _timeSinceScoreGain += deltaTime;
        if (_timeSinceScoreGain >= 1)
        {
            if (Entity.TryFindComponentsByType<ScoreComponent>(out var scoreComponents))
            {
                var component = scoreComponents.First();
                component.GainScore((int)Math.Floor(_timeSinceScoreGain));
                _timeSinceScoreGain -= (float)Math.Floor(_timeSinceScoreGain);
            }
        }

        _lastDeltaTime = deltaTime;
    }

    public void OnCollision(CollidableComponent other, Scene scene)
    {
        if (other.Solid)
        {
            VelocityComponent? velocityComponent = null;
            if (Entity.TryFindComponentsByType<VelocityComponent>(out var velocityComponents))
                velocityComponent = velocityComponents.First();
            if (velocityComponent is not null)
            {
                Entity.Position -= velocityComponent.Velocity * _lastDeltaTime;
                velocityComponent.Velocity.Magnitude = 0;
            }
        }

        if (other.Entity.TryFindComponentsByType<HealthComponent>(out var components))
        {
            var component = components.First();
            component.LoseHealth(100);
        }
    }

    public void OnHealthLost(float health)
    {
        _currentInvulnerabilityPeriod = _invulnerabilityGracePeriod;
        if (Entity.TryFindComponentsByType<CollidableComponent>(out var components))
        {
            var component = components.First();
            component.BlacklistTag("enemy");
            component.BlacklistTag("bullet");
        }

        if (Entity.TryFindComponentsByType<RenderableComponent>(out var renderableComponents))
        {
            var component = renderableComponents.First();
            component.SetColor(new Color(255, 255, 255, 50));
        }
    }
}