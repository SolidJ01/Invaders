namespace Invaders.Model.Entities.Components;

public interface IDamageableLogicComponent
{
    public void OnHealthLost(float health);
}