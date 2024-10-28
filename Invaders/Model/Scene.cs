using Invaders.System;
using Invaders.Model.Entities;
using SFML.Graphics;

namespace Invaders.Model;

public class Scene
{
    private readonly SceneLoader _sceneLoader;
    public SceneLoader SceneLoader => _sceneLoader;
    
    private readonly AssetManager _assetManager;
    public AssetManager AssetManager { get => _assetManager; }
    
    private readonly List<Entity> _entities;
    public List<Entity> Entities => _entities;
    private readonly List<Entity> _spawnQueue;
    private readonly List<Entity> _despawnQueue;
    
    private string _currentScene;
    public string CurrentScene { get => _currentScene; set => _currentScene = value; }
    
    private bool _loading;

    public Scene(SceneLoader sceneLoader, AssetManager assetManager)
    {
        _sceneLoader = sceneLoader;
        _assetManager = assetManager;
        _entities = [];
        _spawnQueue = [];
        _despawnQueue = [];
        _loading = false;
    }

    private void Spawn(Entity entity)
    {
        _entities.Add(entity);
        entity.Instantiate(this);
    }

    public void QueueSpawn(Entity entity)
    {
        _spawnQueue.Add(entity);
    }

    private void Despawn(Entity entity)
    {
        _entities.Remove(entity);
        //TODO: entity.Destroy();
    }

    public void QueueDespawn(Entity entity)
    {
        _despawnQueue.Add(entity);
    }

    public void Clear()
    {
        _entities.Clear();
        _spawnQueue.Clear();
        _despawnQueue.Clear();
        _loading = false;
    }

    public void QueueLoading(string sceneName)
    {
        _loading = true;
        CurrentScene = sceneName;
    }

    public bool TryFindEntitiesByTag(string tag, out List<Entity> entities)
    {
        entities = new List<Entity>();
        if (_entities.Any(e => e.Tag == tag))
        {
            foreach (var entity in _entities.Where(e => e.Tag == tag))
            {
                entities.Add(entity);
            }
        }
        
        return entities.Count > 0;
    }

    public void Update(float deltaTime)
    {
        if (_loading)
        {
            SceneLoader.LoadScene(this, CurrentScene);
            return;
        }
        
        foreach (var entity in _spawnQueue)
        {
            Spawn(entity);
        }
        _spawnQueue.Clear();
        foreach (var entity in _despawnQueue)
        {
            Despawn(entity);
        }
        _despawnQueue.Clear();
        
        foreach (var entity in _entities)
        {
            entity.Update(this, deltaTime);
        }

        foreach (var entity in _entities.Where(x => x.TryFindComponentsByType<CollidableComponent>(out _)))
        {
            entity.TryFindComponentsByType<CollidableComponent>(out var collidableComponents);
            var collidableComponent = collidableComponents.First();
            collidableComponent.CheckCollision(this);
        }

        foreach (var entity in _entities)
        {
            entity.DispatchEvents(this);
        }
    }

    public void Render(RenderTarget target)
    {
        foreach (var entity in _entities.OrderBy(x => x.Layer))
        {
            entity.Render(target, false);
        }
    }
}