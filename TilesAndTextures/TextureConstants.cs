using SFML.System;

namespace Dungeon.TilesAndTextures;

public static class TexturePaths
{
    public static readonly string DungeonTilesetPath = Path.Combine("assets", "Dungeon_Tileset.png");
    public static readonly Vector2i DungeonTilesetSize = new Vector2i(16, 16);

    public static readonly string CharacterTilesetPath = Path.Combine("assets", "Dungeon_Character.png");
    public static readonly Vector2i CharacterTilesetSize = new Vector2i(16, 16);
}

public static class SpritePositions
{
    public static readonly Vector2i Player = new Vector2i(3, 2);

    public static readonly Vector2i Hole = new Vector2i(8, 7);
    public static readonly Vector2i DoorHorizontal = new Vector2i(7, 3);
    public static readonly Vector2i DoorVertical = new Vector2i(7, 4);

    public static readonly Vector2i Skeleton = new Vector2i(4, 3);
    public static readonly Vector2i Ghost = new Vector2i(2, 3);
    public static readonly Vector2i Reaper = new Vector2i(1, 2);

    public static readonly Vector2i SilverChestSmall = new Vector2i(1, 8);
    public static readonly Vector2i SilverChestBig = new Vector2i(2, 8);
    public static readonly Vector2i WoddenChestSmall = new Vector2i(0, 8);
    public static readonly Vector2i WoddenChestBig = new Vector2i(3, 8);

    public static readonly Vector2i WallTorch = new Vector2i(1, 9);
    public static readonly Vector2i TorchLit = new Vector2i(3, 9);
    public static readonly Vector2i TorchUnlit = new Vector2i(4, 9);

    public static readonly Vector2i Coin = new Vector2i(6, 8);

    public static readonly Vector2i HealthPotionSmall = new Vector2i(8, 9);
    public static readonly Vector2i HealthPotionBig = new Vector2i(9, 8);

    public static readonly Vector2i Key = new Vector2i(9, 9);
}

public static class FloorPositions
{
    public static Vector2i FloorCenter => Utils.GetRandomTexture(FloorCenterOptions);
    public static Vector2i FloorTop => Utils.GetRandomTexture(FloorTopOptions);
    public static Vector2i FloorBottom => Utils.GetRandomTexture(FloorBottomOptions);
    public static readonly Vector2i FloorLeft = new Vector2i(1, 2);
    public static readonly Vector2i FloorRight = new Vector2i(4, 2);
    public static readonly Vector2i FloorTopLeft = new Vector2i(1, 1);
    public static readonly Vector2i FloorTopRight = new Vector2i(4, 1);
    public static readonly Vector2i FloorBottomLeft = new Vector2i(1, 3);
    public static readonly Vector2i FloorBottomRight = new Vector2i(4, 3);

    private static readonly Vector2i[] FloorCenterOptions = new Vector2i[] {
        new Vector2i(2, 2),
        new Vector2i(3, 2),
    };
    private static readonly Vector2i[] FloorTopOptions = new Vector2i[] {
        new Vector2i(2, 1),
        new Vector2i(3, 1),
    };
    private static readonly Vector2i[] FloorBottomOptions = new Vector2i[] {
        new Vector2i(2, 3),
        new Vector2i(3, 3),
    };
}

public static class WallPositions
{
    public static Vector2i WallTop => Utils.GetRandomTexture(WallTopOptions);

    public static Vector2i WallBottom => Utils.GetRandomTexture(WallBottomOptions);

    public static Vector2i WallLeft => Utils.GetRandomTexture(WallLeftOptions);

    public static Vector2i WallRight => Utils.GetRandomTexture(WallRightOptions);
    public static Vector2i WallLeftRight => Utils.GetRandomTexture(WallLeftRightOptions);

    public static readonly Vector2i WallBottomLeft = new Vector2i(0, 4);
    public static readonly Vector2i WallBottomRight = new Vector2i(5, 4);
    public static readonly Vector2i WallTopLeft = new Vector2i(0, 5);
    public static readonly Vector2i WallTopRight = new Vector2i(3, 5);


    private static readonly Vector2i[] WallTopOptions = new Vector2i[] {
        new Vector2i(1, 0),
        new Vector2i(2, 0),
        new Vector2i(3, 0),
    };

    private static readonly Vector2i[] WallBottomOptions = new Vector2i[] {
        new Vector2i(1, 4),
        new Vector2i(2, 4),
        new Vector2i(3, 4),
    };

    private static readonly Vector2i[] WallLeftOptions = new Vector2i[] {
        new Vector2i(0, 0),
        new Vector2i(0, 1),
        new Vector2i(0, 2),
    };

    private static readonly Vector2i[] WallRightOptions = new Vector2i[] {
        new Vector2i(5, 0),
        new Vector2i(5, 1),
        new Vector2i(5, 2),
    };

    private static readonly Vector2i[] WallLeftRightOptions = new Vector2i[] {
        new Vector2i(6, 0),
        new Vector2i(6, 1),
        new Vector2i(6, 2),
    };

}

