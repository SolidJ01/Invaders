namespace Invaders.Model.Entities.Components;

public class GameOverUILogicComponent : EntityComponent, IInstantiatableComponent, IKillableLogicComponent
{
    private UIMenuLogicComponent _uiMenuLogicComponent;
    
    public GameOverUILogicComponent(UIMenuLogicComponent uiMenuLogicComponent, Entity entity) : base(entity)
    {
        _uiMenuLogicComponent = uiMenuLogicComponent;
    }

    public void Instantiate(Scene scene)
    {
        if (scene.TryFindEntitiesByTag("player", out var players))
        {
            var player = players.First();
            if (player.TryFindComponentsByType<HealthComponent>(out var healthComponents))
            {
                var health = healthComponents.First();
                health.Death += OnDeath;
            }
        }
    }

    public void OnDeath(HealthComponent healthComponent)
    {
        _uiMenuLogicComponent.SetActive(true);
        healthComponent.Death -= OnDeath;
    }
}