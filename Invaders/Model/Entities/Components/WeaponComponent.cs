using Invaders.System;

namespace Invaders.Model.Entities.Components;

public class WeaponComponent : EntityComponent, IUpdatableLogicComponent
{
    private readonly Func<Dictionary<string, string>?, Entity> _spawnBullet;

    private readonly Vector2D _offset;
    private Vector2D Position => Entity.Position + _offset;
    private readonly Vector2D _muzzleVelocity;
    
    private bool _firing;
    public bool Firing { get => _firing; set => _firing = value; }
    private readonly float _timeBetweenShots;
    private float _timeSinceLastShot;

    private readonly Dictionary<string, string> _bulletArgs;
    
    public WeaponComponent(Entity entity, Func<Dictionary<string, string>?, Entity> spawnBullet, Vector2D offset, Vector2D muzzleVelocity, float timeBetweenShots, Dictionary<string, string> bulletArgs) : base(entity)
    {
        _spawnBullet = spawnBullet;
        _offset = offset;
        _muzzleVelocity = muzzleVelocity;
        _timeBetweenShots = timeBetweenShots;
        _timeSinceLastShot = 0;
        _bulletArgs = bulletArgs;
    }

    public void Update(Scene scene, float deltaTime)
    {
        if (_firing)
        {
            _timeSinceLastShot += deltaTime;
            if (_timeSinceLastShot >= _timeBetweenShots)
            {
                Entity bullet = _spawnBullet(_bulletArgs);
                bullet.Position = Position;
                if (bullet.TryFindComponentsByType<VelocityComponent>(out var components) && Entity.TryFindComponentsByType<VelocityComponent>( out var shipComponents))
                {
                    VelocityComponent velocityComponent = components.First();
                    VelocityComponent shipVelocityComponent = shipComponents.First();
                    velocityComponent.Velocity = _muzzleVelocity + shipVelocityComponent.Velocity;
                }
                scene.QueueSpawn(bullet);
                _timeSinceLastShot = 0;
            }
        }
        else
        {
            _timeSinceLastShot = 0;
        }
    }
}