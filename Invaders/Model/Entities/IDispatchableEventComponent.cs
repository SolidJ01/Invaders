namespace Invaders.Model.Entities;

public interface IDispatchableEventComponent
{
    public void DispatchEvents(Scene scene);
}