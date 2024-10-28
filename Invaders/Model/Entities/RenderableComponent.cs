using SFML.Graphics;
using SFML.System;

namespace Invaders.Model.Entities;

public class RenderableComponent : EntityComponent, IInstantiatableComponent, IRenderableComponent
{
    private readonly string _textureName;
    protected readonly Sprite _sprite;
    protected readonly IntRect _textureRect;

    public Vector2f Position
    {
        get => _sprite.Position;
        set => _sprite.Position = value;
    }

    public RenderableComponent(string textureName, IntRect baseRect, Entity entity) : base(entity)
    {
        _textureName = textureName;
        _sprite = new Sprite();
        _textureRect = baseRect;
    }

    public void Instantiate(Scene scene)
    {
        _sprite.Texture = scene.AssetManager.LoadTexture(_textureName);
        _sprite.TextureRect = _textureRect;
        _sprite.Origin = new Vector2f(_textureRect.Width / 2f, _textureRect.Height / 2f);
    }
    
    public void Render(RenderTarget target)
    {
        target.Draw(_sprite);
    }

    public void SetColor(Color color)
    {
        _sprite.Color = color;
    }
}