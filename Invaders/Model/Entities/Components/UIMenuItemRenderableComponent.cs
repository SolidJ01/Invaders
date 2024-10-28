namespace Invaders.Model.Entities.Components;

public class UIMenuItemRenderableComponent : UIRenderableComponent
{
    private bool _selected;
    private readonly string _baseText;

    public UIMenuItemRenderableComponent(string baseText, string font, uint fontSize, Entity entity, Alignment alignment = Alignment.Left) : base(font, fontSize, entity, alignment)
    {
        _selected = false;
        _baseText = baseText;
    }

    public override void Instantiate(Scene scene)
    {
        base.Instantiate(scene);
        SetText(_baseText);
    }

    public void SetSelected(bool selected)
    {
        _selected = selected;
        SetText(_selected ? $"*{_baseText}*" : _baseText);
    }
}