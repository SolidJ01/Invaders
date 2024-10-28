namespace Invaders.Model.Entities.Components;

public interface IScoreableLogicComponent
{
    public void OnScoreGained(int score);
}