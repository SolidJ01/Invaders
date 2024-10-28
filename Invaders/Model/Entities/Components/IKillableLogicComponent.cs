namespace Invaders.Model.Entities.Components;

public interface IKillableLogicComponent
{
    public void OnDeath(HealthComponent healthComponent);
}