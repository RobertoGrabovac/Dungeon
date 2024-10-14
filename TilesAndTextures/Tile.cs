using Dungeon.Game;
using Sprite = Dungeon.sprites.Sprite;

namespace Dungeon.TilesAndTextures;

public class Tile : Sprite
{
    public readonly Wall type;

    public Tile(Wall type, Sprite sprite) : base(sprite)
    {
        this.type = type;
    }
    // public Tile(Wall type, Texture tex, Vector2i initial, Vector2i size) : base(tex, initial, size, 1)
    // {
    //     this.type = type;
    // }
    //
    // public Tile(Wall type, string fname, Vector2i initial, Vector2i size) : base(fname, initial, size, 1)
    // {
    //     this.type = type;
    // }
}