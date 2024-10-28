using SFML.Graphics;

namespace Invaders.Model.Entities;

public interface IRenderableComponent
{
    public void Render(RenderTarget target);
}