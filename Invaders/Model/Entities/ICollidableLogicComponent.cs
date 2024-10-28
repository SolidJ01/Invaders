namespace Invaders.Model.Entities;

public interface ICollidableLogicComponent
{
    public void OnCollision(CollidableComponent other, Scene scene);
}