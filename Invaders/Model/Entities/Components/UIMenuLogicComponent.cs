using Invaders.System;
using Invaders.System.Binding;
using SFML.Window;

namespace Invaders.Model.Entities.Components;

public class UIMenuLogicComponent : EntityComponent, IUpdatableLogicComponent, IInstantiatableComponent
{
    private bool _active;

    private readonly Vector2D _position;
    
    private readonly List<MenuItem> _items;
    private readonly List<UIMenuItemRenderableComponent> _renderableItems;

    private readonly string _font;
    private readonly uint _fontSize;
    private readonly UIRenderableComponent.Alignment _alignment;

    private int _index;
    
    private MenuItem ActiveItem => _items[_index];
    private UIMenuItemRenderableComponent ActiveRenderable => _renderableItems[_index];

    // private KeyBinding _previousItemBinding;
    // private KeyBinding _nextItemBinding;
    private AxisBinding _switchItemBinding;
    private KeyBinding _selectBinding;

    private readonly float _switchItemDelay;
    private float _currentSwitchItemDelay;

    private bool _selectReleased;
    
    public UIMenuLogicComponent(List<MenuItem> items, Vector2D position, string font, uint fontSize, Entity entity, UIRenderableComponent.Alignment alignment = UIRenderableComponent.Alignment.Left, bool active = true) : base(entity)
    {
        _active = active;
        _position = position;
        _items = items;
        _font = font;
        _fontSize = fontSize;
        _alignment = alignment;

        _renderableItems = new List<UIMenuItemRenderableComponent>();
        _index = 0;

        // _previousItemBinding = new KeyBinding([Keyboard.Key.W, Keyboard.Key.Up]);
        // _nextItemBinding = new KeyBinding([Keyboard.Key.S, Keyboard.Key.Down]);
        _switchItemBinding = new AxisBinding(new KeyBinding([Keyboard.Key.W, Keyboard.Key.Up]),
            new KeyBinding([Keyboard.Key.S, Keyboard.Key.Down]));
        _selectBinding = new KeyBinding([Keyboard.Key.Space, Keyboard.Key.Numpad0]);
        
        _switchItemDelay = 0.25f;
        _currentSwitchItemDelay = 0;
        
        _selectReleased = false;
        
        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            UIMenuItemRenderableComponent renderable = new UIMenuItemRenderableComponent(item.Title, _font, _fontSize, Entity, _alignment);
            renderable.SetPosition(_position + new Vector2D(0, i * (_fontSize + 25)));
            _renderableItems.Add(renderable);
            Entity.AddComponent(renderable);
        }
    }

    public void Update(Scene scene, float deltaTime)
    {
        if (!_active)
            return;
        
        _currentSwitchItemDelay -= deltaTime;
        if (_switchItemBinding.State != 0 && _currentSwitchItemDelay <= 0)
        {
            _index += (int)Math.Round(_switchItemBinding.State);
            if (_index >= _items.Count)
                _index = 0;
            if (_index < 0)
                _index = _items.Count - 1;

            foreach (var renderable in _renderableItems)
            {
                renderable.SetSelected(false);
            }
            
            ActiveRenderable.SetSelected(true);
            
            _currentSwitchItemDelay = _switchItemDelay;
        }

        if (_selectReleased && _selectBinding.IsPressed)
        {
            ActiveItem.Select(scene);
        }

        if (!_selectReleased && !_selectBinding.IsPressed)
        {
            _selectReleased = true;
        }
    }

    public void Instantiate(Scene scene)
    {
        ActiveRenderable.SetSelected(true);
        SetActive(_active);
    }

    public void SetActive(bool active)
    {
        _active = active;
        foreach (var renderable in _renderableItems)
        {
            renderable.SetVisible(_active);
        }
    }
}