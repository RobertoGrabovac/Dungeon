using Dungeon.sprites;
using Dungeon.TilesAndTextures;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Dungeon.Game;

public class Player : Entity
{
    private bool rightFacing = true;
    private float maxSpeed;
    private float damageMultiplier;
    private float hitPoints;

    public Player(Dungeon.Game.Game game, int health, float damageMultiplier, float hitPoints, float maxSpeed) : base(game, new Texture(TexturePaths.CharacterTilesetPath),
        SpritePositions.Player,
        TexturePaths.CharacterTilesetSize, 1, 0)
    {
        Direction = new Vector2f(1, 0);
        DestroyOnDeath = false;
        baseDamage = 3;
        this.health = health;
        this.damageMultiplier = damageMultiplier;
        this.hitPoints = hitPoints;
        this.maxSpeed = maxSpeed;
        attackRadius = 10;
        //damageRadius = 32;
        damageRadius =
            4; // Ako je ovo preveliko, napadi ne mogu proći kroz uske hodnike pa čak ni malo manje sobe jer se
        // sudaraju sa zidovima i ne uspiju uopće doći blizu neprijatelja/baklje. Alternativno: ne koristiti radijus 
        // nego jednostavno raycast test sa ciljnim hitboxom neprijatelja/baklje...

        hitbox.pos = Position;
        hitbox.half *= 0.8f; // inače je hitbox pretijesan za prolazak kroz hodnike
        hitcircle.center = Position;
        attackCooldown = 0.4f;
}

    public override void TakeDamage(Damage damage)
    {
        if (damage.type == DamageType.Enemy)
        {
            game.SplatterBlood(Position);
            float damageTaken = damage.hitPoints * damageMultiplier;
            health -= damageTaken;
        }
    }

    public override void Update(float dt)
    {
        Vector2f direction = Direction;
        bool moving = false;

        if (attackCounter >= 0)
        {
            SetAnimationFrame(1);
        }
        else
        {
            SetAnimationFrame(0);
        }

        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            var mousePosition = Mouse.GetPosition(game.win);
            var worldMousePosition = game.win.MapPixelToCoords(mousePosition, game.GameView);

            var attackDirection = Utils.normalize(worldMousePosition - Position);
            Attack(new Damage(hitPoints, DamageType.Player, this, damageRadius, attackRadius, attackDirection));
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.W))
        {
            direction = new Vector2f(0, -1);
            direction = Utils.normalize(direction);
            moving = true;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.A))
        {
            direction = new Vector2f(-1, 0) + (moving ? direction : new Vector2f());
            direction = Utils.normalize(direction);
            moving = true;
            if (rightFacing)
            {
                rightFacing = false;
                FlipVertical();
            }
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.S))
        {
            direction = new Vector2f(0, 1) + (moving ? direction : new Vector2f());
            direction = Utils.normalize(direction);
            moving = true;
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.D))
        {
            direction = new Vector2f(1, 0) + (moving ? direction : new Vector2f());
            direction = Utils.normalize(direction);
            moving = true;
            if (!rightFacing)
            {
                rightFacing = true;
                FlipVertical();
            }
        }

        if (!moving)
        {
            Speed = 0;
        }
        else
        {
            Speed = maxSpeed;
            Direction = direction;    
        }
        
        base.Update(dt);
    }
}