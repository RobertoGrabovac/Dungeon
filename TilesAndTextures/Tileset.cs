using Dungeon.Game;
using SFML.Graphics;
using SFML.System;

namespace Dungeon.TilesAndTextures;

/**
 * Ova klasa olakšava stvaranje mapa koje koriste različite tilesetove: nužno je samo postaviti odgovarajuće parametre
 * i .png s pixelima.
 */
public class Tileset
{
    public readonly string ImageFilename;
    public readonly Vector2i TileDims;
    public readonly Texture texture;

    /**
     * LUT koji preslikava tip tilea u odgovarajuću koordinatu gornje-lijevog ruba.
     */
    public readonly Dictionary<Wall, Vector2i> wallMapper;

    public Tileset(string filename, Vector2i dims, Dictionary<Wall, Vector2i> walls)
    {
        ImageFilename = filename;
        TileDims = dims;
        wallMapper = walls;
        texture = new Texture(ImageFilename);
    }
}