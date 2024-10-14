using SFML.Graphics;
using SFML.System;
using Sprite = Dungeon.sprites.Sprite;

namespace Dungeon.TilesAndTextures;

/**
 * Ova klasa olaksava ucitavanje tekstura u grid slikama
 */

public class TextureLoader
{
    public readonly Vector2i textureSize;
    public readonly Texture texture;

    public TextureLoader(string filename, Vector2i textureSize)
    {
        this.textureSize = textureSize;
        texture = new Texture(filename);
    }

    public Sprite GetSpriteForCell(Vector2i textureLocation)
    {
        return new Sprite(texture, textureLocation, textureSize, 1);
    }
}