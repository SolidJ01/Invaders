namespace Invaders.Model.Entities;

public interface IUpdatableLogicComponent
{
    public void Update(Scene scene, float deltaTime);
}