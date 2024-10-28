using Invaders.System;
using SFML.Graphics;

namespace Invaders.Model.Entities;

public class UIRenderableComponent : EntityComponent,  IRenderableComponent, IInstantiatableComponent
{
    private bool _visible;
    public enum Alignment { Left, Center, Right }
    
    private readonly string _font;
    private readonly uint _fontSize;
    private readonly Alignment _alignment;
    private Text _text;

    private Vector2D _position;

    public UIRenderableComponent(string font, uint fontSize, Entity entity, Alignment alignment = Alignment.Left, bool visible = true) : base(entity)
    {
        _visible = visible;
        _font = font;
        _fontSize = fontSize;
        _alignment = alignment;
        _text = new Text();
    }

    public virtual void Instantiate(Scene scene)
    {
        _text.Font = scene.AssetManager.LoadFont(_font);
        _text.CharacterSize = _fontSize;
        _text.FillColor = new Color(71, 242, 83);
        if (_position is not null)
            SetPosition(_position);
    }

    public void SetText(string text)
    {
        _text.DisplayedString = text;
        SetPosition(_position);
    }

    public void SetPosition(Vector2D position)
    {
        _position = position;
        
        switch (_alignment)
        {
            case Alignment.Left:
                _text.Position = _position;
                break;
            case Alignment.Center:
                _text.Position = _position - new Vector2D(_text.GetGlobalBounds().Width / 2, 0);
                break;
            case Alignment.Right:
                _text.Position = _position - new Vector2D(_text.GetGlobalBounds().Width, 0);
                break;
        }
    }

    public void SetVisible(bool visible)
    {
        _visible = visible;
    }

    public void Render(RenderTarget target)
    {
        if (_visible)
            target.Draw(_text);
    }
}