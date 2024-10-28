using SFML.Graphics;

namespace Invaders.System;

public class AssetManager
{
    private const string BasePath = "Assets";
    private readonly Dictionary<string, Texture> _textures;
    private readonly Dictionary<string, Font> _fonts;

    public AssetManager()
    {
        _textures = new Dictionary<string, Texture>();
        _fonts = new Dictionary<string, Font>();
    }

    public Texture LoadTexture(string path)
    {
        if (!_textures.ContainsKey(path))
        {
            _textures.Add(path, new Texture($"{BasePath}/{path}.png"));
        }
        return _textures[path];
    }

    public Font LoadFont(string path)
    {
        if (!_fonts.ContainsKey(path))
        {
            _fonts.Add(path, new Font($"{BasePath}/{path}.ttf"));
        }
        return _fonts[path];
    }
}