using Dungeon.Menu;
using Dungeon.sprites;
using Dungeon.TilesAndTextures;
using SFML.System;
using Sprite = Dungeon.sprites.Sprite;

namespace Dungeon.Game;

public enum Wall
{
    HOLE = 0,
    WALL = 1,
    FLOOR = 2,
}

public class DungeonGenerator
{
    private int width;
    private int height;
    private int maxSubdungeonSize = 14;
    private int minRoomSize = 5;
    public Wall[,] walls;
    public List<Torch> torches;
    public List<Entity> worldEntities; // chests, torch, novcici, etc.
    public List<Entity> entities; // enemies, etc. - na ovaj nacin se chestovi mogu iscrtati prije neprijatelja

    public TextureLoader dungeonSpritesLoader = new TextureLoader(TexturePaths.DungeonTilesetPath, TexturePaths.DungeonTilesetSize);
    public TextureLoader entitySpritesLoader = new TextureLoader(TexturePaths.CharacterTilesetPath, TexturePaths.CharacterTilesetSize);

    private Difficulty difficulty;
    private Vector2f playerPosition;
    public Game game;

    Random random = new Random();

    public DungeonGenerator(Game game, int width, int height, Difficulty difficulty)
    {
        this.difficulty = difficulty;
        this.game = game;
        this.width = width;
        this.height = height;
        walls = new Wall[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                walls[i, j] = Wall.HOLE;
            }
        }
        
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        torches = new List<Torch>();
        worldEntities = new List<Entity>();
        entities = new List<Entity>();
        SplitRecursive(0, 0, width - 1, height - 1);
        game.player.Position = playerPosition;
    }

    public (List<Entity>, List<Entity>) buildEntities()
    {
        List<Entity> worldEntities = new List<Entity>(this.worldEntities);
        worldEntities.AddRange(torches);
        return (entities, worldEntities);
    }

    private void SplitRecursive(int x1, int y1, int x2, int y2)
    {
        int xCenter = (x1 + x2) / 2;
        int yCenter = (y1 + y2) / 2;

        int dungeonWidth = x2 - x1 + 1;
        int dungeonHeight = y2 - y1 + 1;

        if (dungeonWidth < maxSubdungeonSize && dungeonHeight < maxSubdungeonSize)
        {
            int roomX1 = random.Next(x1, Math.Max(xCenter - minRoomSize / 2, x1));
            int roomY1 = random.Next(y1, Math.Max(yCenter - minRoomSize / 2, y1));
            int roomX2 = random.Next(Math.Min(x2, xCenter + (minRoomSize + 1) / 2 - 1), x2);
            int roomY2 = random.Next(Math.Min(y2, yCenter + (minRoomSize + 1) / 2 - 1), y2);

            CreateRoom(roomX1, roomY1, roomX2, roomY2);
            return;
        }

        bool horizontalSplit = random.Next(2) == 0;
        if (dungeonWidth < maxSubdungeonSize) horizontalSplit = true;
        if (dungeonHeight < maxSubdungeonSize) horizontalSplit = false;

        if (horizontalSplit)
        {
            int splitPosition = random.Next(y1 + maxSubdungeonSize / 2 - 1, y2 - maxSubdungeonSize / 2 + 1);

            SplitRecursive(x1, y1, x2, splitPosition);
            SplitRecursive(x1, splitPosition, x2, y2);

            CreateHallway(xCenter, splitPosition, false);
        }
        else
        {
            int splitPosition = random.Next(x1 + maxSubdungeonSize / 2 - 1, x2 - maxSubdungeonSize / 2 + 1);

            SplitRecursive(x1, y1, splitPosition, y2);
            SplitRecursive(splitPosition, y1, x2, y2);

            CreateHallway(splitPosition, yCenter, true);
        }
    }

    private void CreateRoom(int x1, int y1, int x2, int y2)
    {
        var alignment = new Vector2f(1.5f, 1.5f);
        int xCenter = (x1 + x2) / 2;
        int yCenter = (y1 + y2) / 2;
        playerPosition = getSpritePosition(yCenter + 1, xCenter + 1); // Igrac se uvijek nalazi u zadnjoj generiranoj sobi
        // Baklja se uvijek generira u prvoj sobi, a nekad i u ostalima

        if (torches.Count == 0 || random.NextDouble() < 0.8 / (torches.Count + 1))
        {
            var torch = new Torch(game, dungeonSpritesLoader.GetSpriteForCell(SpritePositions.TorchLit));
            torch.Position = getSpritePosition(yCenter, xCenter);
            torches.Add(torch);
        }
        
        if (random.NextDouble() < 0.8) // nasumično ubacivanje skeletona
        {
            var skellySprite = new Sprite(entitySpritesLoader.texture,
                SpritePositions.Skeleton,
                TexturePaths.CharacterTilesetSize);
            float hp;
            float health;
            float speed;
            switch (difficulty)
            {
                case Difficulty.EASY:
                    hp = 0.4f;
                    health = 3;
                    speed = 20;
                    break;
                case Difficulty.MEDIUM:
                    hp = 0.8f;
                    health = 11;
                    speed = 23;
                    break;
                case Difficulty.HARD:
                    hp = 1;
                    health = 20;
                    speed = 30;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var skeleton = new Skeleton(game, skellySprite, speed, hp, health);
            skeleton.Position = getSpritePosition(yCenter - 1, xCenter + 1) + alignment;
            entities.Add(skeleton);
        }

        if (random.NextDouble() < 0.6) // ghost
        {
            var ghostSprite = new Sprite(entitySpritesLoader.texture,
                SpritePositions.Ghost,
                TexturePaths.CharacterTilesetSize);
            float hp;
            float health;
            float speed;
            switch (difficulty)
            {
                case Difficulty.EASY:
                    hp = 0.4f;
                    health = 3;
                    speed = 10;
                    break;
                case Difficulty.MEDIUM:
                    hp = 1;
                    health = 5;
                    speed = 12;
                    break;
                case Difficulty.HARD:
                    hp = 5;
                    health = 20;
                    speed = 15;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var ghost = new Ghost(game, ghostSprite, speed, hp, health);
            ghost.Position = getSpritePosition(yCenter + 1, xCenter - 1) + alignment;
            entities.Add(ghost);
        }

        if (random.NextDouble() < 0.3) // reaper(?)
        {
            var reapSprite = new Sprite(entitySpritesLoader.texture,
                SpritePositions.Reaper,
                TexturePaths.CharacterTilesetSize);
            float hp;
            float health;
            float speed;
            switch (difficulty)
            {
                case Difficulty.EASY:
                    hp = 0.8f;
                    health = 3;
                    speed = 7;
                    break;
                case Difficulty.MEDIUM:
                    hp = 1.2f;
                    health = 5;
                    speed = 8;
                    break;
                case Difficulty.HARD:
                    hp = 2.2f;
                    health = 20;
                    speed = 13;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var reaper = new Reaper(game, reapSprite, speed, hp, health);
            reaper.Position = getSpritePosition(yCenter + 1, xCenter + 2) + alignment;
            entities.Add(reaper);
        }
        
        for (int i = x1 + 1; i < x2; i++)
        {
            for (int j = y1 + 1; j < y2; j++)
            {
                walls[j, i] = Wall.FLOOR;
            }
        }
        for (int i = x1; i <= x2; i++)
        {
            walls[y1, i] = Wall.WALL;
            walls[y2, i] = Wall.WALL;
        }
        for (int i = y1; i <= y2; i++)
        {
            walls[i, x1] = Wall.WALL;
            walls[i, x2] = Wall.WALL;
        }
    }

    private void PlaceHorizontal(int x, int y)
    {
        walls[y + 1, x] = Wall.WALL;
        walls[y, x] = Wall.FLOOR;
        walls[y - 1, x] = Wall.WALL;
    }

    private void PlaceVertical(int x, int y)
    {
        walls[y, x - 1] = Wall.WALL;
        walls[y, x] = Wall.FLOOR;
        walls[y, x + 1] = Wall.WALL;
    }

    private void CreateHallway(int x, int y, bool isHorizontal)
    {
        if (isHorizontal)
        {
            for (int i = x; i < width; i++)
            {
                if (walls[y, i] == Wall.FLOOR) break;
                if (random.NextDouble() < 0.4f)
                { // nasumično smještanje škrinja; koriste se drukčiji spriteovi u horizontalnim/vertikalnim hodnicima
                    var chestSprite = new Sprite(dungeonSpritesLoader.texture,
                        new Vector2i(4, 8),
                        TexturePaths.DungeonTilesetSize, 1);
                    var chest = new Chest(game, chestSprite, getSpritePosition(y, i));
                    worldEntities.Add(chest);
                }
                PlaceHorizontal(i, y);
            }
            for (int i = x - 1; i >= 0; i--)
            {
                if (walls[y, i] == Wall.FLOOR) break;
                if (random.NextDouble() < 0.3f)
                {
                    var chestSprite = new Sprite(dungeonSpritesLoader.texture,
                        new Vector2i(3, 8),
                        TexturePaths.DungeonTilesetSize, 1);
                    var chest = new Chest(game, chestSprite, getSpritePosition(y, i));
                    worldEntities.Add(chest);
                }
                PlaceHorizontal(i, y);
            }
        }
        else
        {
            for (int i = y; i < height; i++)
            {
                if (walls[i, x] == Wall.FLOOR) break;
                PlaceVertical(x, i);
            }
            for (int i = y - 1; i >= 0; i--)
            {
                if (walls[i, x] == Wall.FLOOR) break;
                PlaceVertical(x, i);
            }
        }
    }

    private Vector2f getSpritePosition(int y, int x)
    {
        return new Vector2f(dungeonSpritesLoader.textureSize.Y * y, dungeonSpritesLoader.textureSize.X * x);
    }

    private Wall getWall(int y, int x)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return Wall.HOLE;
        }
        return walls[y, x];
    }

    public Tile?[] buildWorldSprites()
    {
        Tile?[] worldTiles = new Tile?[walls.Length];
        for (int i = 0; i < walls.GetLength(0); ++i)
        {
            for (int j = 0; j < walls.GetLength(1); ++j)
            {
                Tile? sprite;
                Sprite texture;

                if (getWall(i, j) == Wall.FLOOR)
                {
                    if (getWall(i - 1, j) == Wall.WALL)
                    {
                        if (getWall(i, j - 1) == Wall.WALL)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorTopLeft);
                        }
                        else if (getWall(i, j + 1) == Wall.WALL)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorBottomLeft);
                        }
                        else
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorLeft);
                        }
                    }
                    else if (getWall(i + 1, j) == Wall.WALL)
                    {
                        if (getWall(i, j - 1) == Wall.WALL)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorTopRight);
                        }
                        else if (getWall(i, j + 1) == Wall.WALL)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorBottomRight);
                        }
                        else
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorRight);
                        }
                    }
                    else if (getWall(i, j + 1) == Wall.WALL)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorBottom);
                    }
                    else if (getWall(i, j - 1) == Wall.WALL)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorTop);
                    }
                    else
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(FloorPositions.FloorCenter);
                    }
                    sprite = new Tile(Wall.FLOOR, texture);
                }
                else if (getWall(i, j) == Wall.WALL)
                {
                    if (getWall(i, j + 1) == Wall.FLOOR)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallTop);
                    }
                    else if (getWall(i, j - 1) == Wall.FLOOR)
                    {
                        if (getWall(i - 1, j) == Wall.FLOOR && getWall(i + 1, j) == Wall.FLOOR)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallLeftRight);
                        }
                        else if (getWall(i + 1, j) == Wall.FLOOR)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallTopRight);
                        }
                        else if (getWall(i - 1, j) == Wall.FLOOR)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallTopLeft);
                        }
                        else
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallBottom);
                        }
                    }
                    else if (getWall(i - 1, j) == Wall.FLOOR && getWall(i + 1, j) == Wall.FLOOR)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallLeftRight);
                    }
                    else if (getWall(i + 1, j) == Wall.FLOOR)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallLeft);
                    }
                    else if (getWall(i - 1, j) == Wall.FLOOR)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallRight);
                    }
                    else if ((getWall(i, j - 1) == Wall.WALL || getWall(i, j + 1) == Wall.WALL) && (getWall(i + 1, j) == Wall.WALL || getWall(i - 1, j) == Wall.WALL))
                    {
                        int leftWall = getWall(i, j - 1) == Wall.WALL ? 1 : 0;
                        int rightWall = getWall(i, j + 1) == Wall.WALL ? 1 : 0;
                        int topWall = getWall(i - 1, j) == Wall.WALL ? 1 : 0;
                        int bottomWall = getWall(i + 1, j) == Wall.WALL ? 1 : 0;
                        if (getWall(i + 1, j - 1) == Wall.FLOOR)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallBottomLeft);
                        }
                        else if (getWall(i - 1, j - 1) == Wall.FLOOR)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallBottomRight);
                        }
                        else if (getWall(i + 1, j + 1) == Wall.FLOOR)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallLeft);
                        }
                        else if (getWall(i - 1, j + 1) == Wall.FLOOR)
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallRight);
                        }
                        else
                        {
                            texture = dungeonSpritesLoader.GetSpriteForCell(SpritePositions.DoorHorizontal);
                        }
                    }
                    else if (getWall(i + 1, j) == Wall.WALL)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallLeft);
                    }
                    else if (getWall(i - 1, j) == Wall.WALL)
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(WallPositions.WallRight);
                    }
                    else
                    {
                        texture = dungeonSpritesLoader.GetSpriteForCell(SpritePositions.DoorHorizontal);
                    }
                    sprite = new Tile(Wall.WALL, texture);
                }
                else
                {
                    sprite = null;
                }

                if (sprite != null)
                {
                    sprite.Position = getSpritePosition(i, j);
                    sprite.hitbox.pos = sprite.Position;
                    sprite.hitcircle.center = sprite.Position;
                }

                worldTiles[i * walls.GetLength(1) + j] = sprite;
            }
        }
        return worldTiles;
    }
}