namespace Invaders.Model.Entities.Components;

public delegate void ScoreGainedEvent(int score);

public class ScoreComponent : EntityComponent, IInstantiatableComponent
{
    public event ScoreGainedEvent ScoreGained;
    
    private int _score;
    public int Score => _score;

    public ScoreComponent(Entity entity) : base(entity)
    {
        _score = 0;
    }

    public void GainScore(int score)
    {
        _score += score;
        ScoreGained?.Invoke(_score);
    }

    public void Instantiate(Scene scene)
    {
        if (Entity.TryFindComponentsByType<IScoreableLogicComponent>(out var scoreSubscribers))
        {
            foreach (var subscriber in scoreSubscribers)
            {
                ScoreGained += subscriber.OnScoreGained;
            }
        }
    }
}