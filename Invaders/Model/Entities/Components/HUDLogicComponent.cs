using Invaders.System;

namespace Invaders.Model.Entities.Components;

public class HUDLogicComponent : EntityComponent, IInstantiatableComponent, IDamageableLogicComponent, IKillableLogicComponent, IScoreableLogicComponent
{
    private readonly UIRenderableComponent _healthElement;
    private readonly UIRenderableComponent _scoreElement;
    
    public HUDLogicComponent(UIRenderableComponent healthElement, UIRenderableComponent scoreElement, Entity entity) : base(entity)
    {
        _healthElement = healthElement;
        _scoreElement = scoreElement;
    }

    public void Instantiate(Scene scene)
    {
        if (scene.TryFindEntitiesByTag("player", out var players))
        {
            var player = players.First();
            if (player.TryFindComponentsByType<HealthComponent>(out var healthComponents))
            {
                var health = healthComponents.First();
                health.HealthLost += OnHealthLost;
                health.Death += OnDeath;
                SetHealthString(health.Health);
            }

            if (player.TryFindComponentsByType<ScoreComponent>(out var scoreComponents))
            {
                var score = scoreComponents.First();
                score.ScoreGained += OnScoreGained;
                SetScoreString(score.Score);
            }
        }
    }

    public void OnHealthLost(float health)
    {
        SetHealthString(health);
    }

    public void OnDeath(HealthComponent healthComponent)
    {
        healthComponent.HealthLost -= OnHealthLost;
        healthComponent.Death -= OnDeath;
    }

    public void OnScoreGained(int score)
    {
        SetScoreString(score);
    }

    private void SetHealthString(float health)
    {
        _healthElement.SetText($"Structural Integrity: {MathF.Round(health, 0)}%");
    }

    private void SetScoreString(int score)
    {
        _scoreElement.SetText($"{score} pts");
    }
}