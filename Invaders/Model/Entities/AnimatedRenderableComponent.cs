using SFML.Graphics;
using SFML.System;

namespace Invaders.Model.Entities;

public class AnimatedRenderableComponent : RenderableComponent, IUpdatableLogicComponent
{
    private readonly float _interval;
    private readonly List<Vector2i> _textureOffsets;

    private float _timeSinceChange;
    private int _index;
    private Vector2i CurrentOffset => _textureOffsets[_index];
    
    public AnimatedRenderableComponent(string textureName, IntRect baseRect, Entity entity, float interval, List<Vector2i> textureOffsets) : base(textureName, baseRect, entity)
    {
        _interval = interval;
        _textureOffsets = textureOffsets;
        _timeSinceChange = 0;
        _index = 0;
    }

    public void Update(Scene scene, float deltaTime)
    {
        _timeSinceChange += deltaTime;
        if (_timeSinceChange >= _interval)
        {
            _index++;
            if (_index >= _textureOffsets.Count)
                _index = 0;
            
            _sprite.TextureRect = new IntRect(new Vector2i(_textureRect.Left + CurrentOffset.X, _textureRect.Top + CurrentOffset.Y), new Vector2i(_textureRect.Width, _textureRect.Height));
            
            _timeSinceChange = 0;
        }
    }
}