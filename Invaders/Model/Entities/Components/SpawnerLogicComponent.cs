namespace Invaders.Model.Entities.Components;

public class SpawnerLogicComponent : EntityComponent, IUpdatableLogicComponent
{
    private readonly Random random;
    private Func<Dictionary<string, string>?, Entity> _spawn;
    private Dictionary<string, string> _args;
    private readonly int _maxSpawned;
    private readonly string _spawnedTag;
    private readonly float _baseSpawnInterval;
    private readonly float _targetSpawnInterval;
    private readonly float _deltaSpawnInterval;
    private readonly int _xmin;
    private readonly int _xmax;
    private readonly int _ymin;
    private readonly int _ymax;
    
    private float _timeSinceLastSpawn;
    private float _currentSpawnInterval;

    public SpawnerLogicComponent(Func<Dictionary<string, string>?, Entity> spawn, int maxSpawned, string spawnedTag, float baseSpawnInterval, float targetSpawnInterval, float deltaSpawnInterval, int xmin, int xmax, int ymin, int ymax, Entity entity, Dictionary<string, string>? args = null) : base(entity)
    {
        random = new Random();
        _spawn = spawn;
        _args = args ?? new Dictionary<string, string>();
        _maxSpawned = maxSpawned;
        _spawnedTag = spawnedTag;
        _baseSpawnInterval = baseSpawnInterval;
        _targetSpawnInterval = targetSpawnInterval;
        _deltaSpawnInterval = deltaSpawnInterval;
        _xmin = xmin;
        _xmax = xmax;
        _ymin = ymin;
        _ymax = ymax;
        
        _timeSinceLastSpawn = 0;
        _currentSpawnInterval = baseSpawnInterval;
    }

    public void Update(Scene scene, float deltaTime)
    {
        _timeSinceLastSpawn += deltaTime;
        
        
        if (_currentSpawnInterval != _targetSpawnInterval)
        {
            _currentSpawnInterval += _deltaSpawnInterval * deltaTime;
            if ((_targetSpawnInterval > _baseSpawnInterval && _currentSpawnInterval > _targetSpawnInterval) || (_targetSpawnInterval < _baseSpawnInterval && _currentSpawnInterval < _targetSpawnInterval))
                _currentSpawnInterval = _targetSpawnInterval;
        }

        if (_timeSinceLastSpawn >= _currentSpawnInterval && scene.Entities.Count(x => x.Tag == _spawnedTag) < _maxSpawned)
        {
            int x = random.Next(_xmin, _xmax + 1);
            int y = random.Next(_ymin, _ymax + 1);
            
            if (_args.ContainsKey("x"))
                _args["x"] = x.ToString();
            else 
                _args.Add("x", x.ToString());
            
            if (_args.ContainsKey("y"))
                _args["y"] = y.ToString();
            else
                _args.Add("y", y.ToString());

            scene.QueueSpawn(_spawn(_args));
            _timeSinceLastSpawn = 0;
        }
    }
}