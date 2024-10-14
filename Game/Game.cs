using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Reflection;
using Dungeon.Menu;
using Dungeon.sprites;
using Dungeon.sprites.hitboxes;
using Dungeon.TilesAndTextures;
using SFML.Graphics;
using SFML.System;

namespace Dungeon.Game;

public class Game : Drawable
{
    public readonly View camera = new View();
    public RenderWindow win;

    private Tile?[] worldTiles;
    private List<Entity> worldEntities; // chests, torch, novcici, etc.
    private List<Entity> entities; // drugi entities - na ovaj nacin se chestovi mogu iscrtati prije neprijatelja
    private List<Entity> killList = new(); // ovo se koristi kako bi se na kraju framea uništili određeni entiteti
    // (neprijatelji); to se ne može raditi u samoj update petlji jer time mijenjamo samu kolekciju kroz koju iteriramo
    private List<Entity> insertList = new(); // dualno stavci iznad
    public Player player;

    private const int dungeonWidth = 36;
    private const int dungeonHeight = 24;

    private int score = 0;
    public int Score
    {
        get => score;
        set
        {
            score = value;
            if (score > HighScore)
                HighScore = score;
        }
    }

    public int Level { get; private set; } = 0;
    public int Gold { get; set; } = 0;
    
    public int TorchCount { get; private set; }
    public int HighScore { get; private set; } = 0;
    
    public int TorchesLit { get; set; }

    public HUD hud;
    private const int gameScale = 5;
    private const int hudScale = 4;
    public View GameView { get; private set; }
    public View HudView { get; private set; }
    private float playerHitPoints, playerSpeed;
    private int lives;
    private Difficulty difficulty;
    
    public Game(RenderWindow win, int highScore, float playerHitPoints, float playerSpeed, int lives, Difficulty difficulty)
    {
        HighScore = highScore;
        this.playerHitPoints = playerHitPoints;
        this.playerSpeed = playerSpeed;
        this.lives = lives;
        this.difficulty = difficulty;
        
        this.win = win;
        hud = new HUD(this);
        GameView = new();
        HudView = new(new Vector2f(win.Size.X, win.Size.Y) / (hudScale * 2),
            new Vector2f(win.Size.X, win.Size.Y) / hudScale);
        SetGameScale(win.Size.X, win.Size.Y);
        
        win.Resized += (sender, eventArgs) =>
        {
            SetGameScale(eventArgs.Width, eventArgs.Height);
        };
        
        GenerateLevel();
    }

    private void SetGameScale(uint width, uint height)
    {
        GameView.Size = new Vector2f(width, height) / gameScale;
        GameView.Center = new Vector2f(width, height) / 2;
    }

    private void InstantiatePlayer()
    {
        float damageMultiplier = difficulty switch
        {
            Difficulty.EASY => 1.0f,
            Difficulty.MEDIUM => 1.5f,
            Difficulty.HARD => 3.5f,
            _ => throw new ArgumentOutOfRangeException()
        };

        player = new Player(this, lives, damageMultiplier, playerHitPoints, playerSpeed);
    }

    private void GenerateLevel()
    {
        Level += 1;
        InstantiatePlayer();
        var generator = new DungeonGenerator(this, dungeonWidth, dungeonHeight, difficulty);

        worldTiles = generator.buildWorldSprites();
        (entities, worldEntities) = generator.buildEntities();

        TorchesLit = 0;
        TorchCount = worldEntities.OfType<Torch>().Count();
    }

    /**
     * Vraća siječe li zraka neki zid.
     */
    public bool IntersectsWalls(Vector2f origin, Vector2f direction)
    {
        return worldTiles.Where(tile => tile is { type: Wall.WALL })
            .Any(tile => tile?.hitbox.intersectSegment(origin, direction) != null);
    }

    /**
     * Vraća entitete s kojima se entitet ent potencijalno sudario unutar delta pomaka. Provjeravamo sudare u više
     * iteracija jer je moguće da jedan sudar ostavi još vremena za gibanje unutar kojeg se može dogoditi drugi itd.
     */
    public Entity?[] TestCollision(ICollidable ent, Vector2f delta, float dt, bool includePlayer = false)
    {
        float tRemaining = 1.0f;
        var collidees = new Entity?[4];
        int nCollisions = 0;
        
        for (int i = 0; i < 4 && tRemaining > 0; ++i)
        {
            var adjustedExtents = ent.hitbox.half + delta * 2;
            var testbox = new Circle(Math.Max(adjustedExtents.X, adjustedExtents.Y), ent.Position);
            var adjustedCircle = new Circle();
            
            var nearest = new Sweep
            {
                time = 1,
                pos = ent.Position + delta
            };
            foreach (var tile in worldTiles)
            {
                if (tile == null) continue;
                if (tile.type != Wall.FLOOR && testbox.intersects(tile.hitcircle))
                {
                    var sweep = tile.hitbox.sweepAABB(ent.hitbox, delta);
                    if (sweep.time < nearest.time)
                        nearest = sweep;
                }
            }

            int nIterations = includePlayer ? 3 : 2;
            for (int j = 0; j < nIterations; ++j) // raditi ovo preko Union i sl. alocira daleko previše memorije na SOH
            {
                List<Entity> entlist = j switch
                {
                    0 => worldEntities,
                    1 => entities,
                    _ => new List<Entity>(1) { player }
                };
                
                // sada još moramo provjeriti potencijalne sudare s ostalim entitetima na mapi
                foreach (var other in entlist) // broadphase test s ostalim entitetima
                {
                    adjustedCircle.radius = other.hitcircle.radius + other.Speed * dt*tRemaining * 2;
                    adjustedCircle.center = other.Position;
                    if (ent != other && other is { Visible: true, Collidable: true } && testbox.intersects(adjustedCircle))
                    {
                        var adjustedDelta = delta - other.ComputeDelta(dt*tRemaining); // other gledamo u okviru gibanja trenutnog entiteta (other ostaje statičan)
                        var sweep = other.hitbox.sweepAABB(ent.hitbox, adjustedDelta);
                        if (sweep.time < nearest.time)
                            nearest = sweep;
                    }
                }
            }

            ent.Position = nearest.pos;
            if (nearest.hit != null)
            {
                if (nearest.hit.time == 0) // HACK: nekako smo se našli "unutar" AABB-a s kojim se sudaramo i treba polako izaći van
                {
                    ent.Position += nearest.hit.normal * 0.4f;
                }
                var normal = nearest.hit.normal;
                delta -= Utils.dotProduct(delta, normal) * normal;
                ent.Direction += normal;
                if (nearest.hit.collider.owner is Entity e)
                    collidees[nCollisions++] = e;
            }

            tRemaining -= nearest.time;
        }

        return collidees;
    }

    public void SplatterBlood(Vector2f location)
    {
        insertList.Add(new Blood(this, location));
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        GameView.Center = camera.Center;
        win.SetView(GameView);
        
        foreach (var sprite in worldTiles)
        {
            sprite?.Draw(target, states);
        }

        foreach (var entity in worldEntities.Union(entities))
        {
            entity.Draw(target, states);
        }

        player.Draw(target, states);
        
        // draw HUD
        win.SetView(HudView);
        hud.Draw(target, states);
    }

    /**
     * Vraća je li igra gotova.
     */
    public bool Update(float dt)
    {
        ExecuteKillList();
        ExecuteInsertList();

        player.Update(dt);
        foreach (var entity in entities.Union(worldEntities))
        {
            entity.Update(dt);
        }
        
        camera.Center = player.Position;

        CheckTorches();

        if (player.health <= 0)
        {
            return true; // igrač je umro, izađi na meni            
        }

        return false;
    }

    public void DestroyEntity(Entity ent)
    {
        killList.Add(ent);
    }

    private void ExecuteKillList()
    {
        foreach (var ent in killList)
        {
            if (worldEntities.Contains(ent))
                worldEntities.Remove(ent);
            else
                entities.Remove(ent);
        }

        killList.Clear();
    }

    private void ExecuteInsertList()
    {
        foreach (var ent in insertList)
        {
            entities.Add(ent);
        }
        
        insertList.Clear();
    }

    private void CheckTorches()
    {
        if (TorchCount == TorchesLit)
        {
            GenerateLevel(); // na sljedeći level
        }
    }
}