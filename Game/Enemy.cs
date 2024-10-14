using Dungeon.sprites;
using Dungeon.sprites.hitboxes;
using SFML.System;

namespace Dungeon.Game;

public class Enemy : Entity
{
    protected float DetectionRadius;
    protected float MaxSpeed;
    protected float MinPlayerDistance;
    protected float HitPoints;
    private Circle DetectionCircle = new Circle();
    private bool rightFacing = true;

    public Enemy(Game game, Sprite sprite, float maxSpeed, float health = 3, float hitPoints = 0.4f,
        float detectionRadius = 40,
        float minPlayerDistance = 20) : base(game, sprite)
    {
        DetectionRadius = detectionRadius;
        MaxSpeed = maxSpeed;
        MinPlayerDistance = minPlayerDistance;
        HitPoints = hitPoints;
        this.health = health;
        hitbox.half *= 0.9f;
        attackCooldown = 2f;
        score = 40;
    }

    /**
     * Vraća poziciju igrača ukoliko je on unutar raspona detekcije neprijatelja.
     */
    protected Vector2f? detectPlayer()
    {
        DetectionCircle.center = Position;
        DetectionCircle.radius = DetectionRadius;
        if (DetectionCircle.intersects(game.player.hitcircle))
            return game.player.Position;
        return null;
    }

    public override void Update(float dt)
    {
        if (attackCounter >= 0)
        {
            SetAnimationFrame(1);
        }
        else
        {
            SetAnimationFrame(0);
        }

        var playerPos = detectPlayer();
        var lineOfSight = playerPos != null ? (Vector2f)(playerPos - Position) : new Vector2f();
        if (playerPos != null && !game.IntersectsWalls(Position, lineOfSight))
        {
            if (Utils.normSquared(lineOfSight) > MinPlayerDistance * MinPlayerDistance)
            {
                var sweep = game.player.hitbox.sweepAABB(hitbox, ComputeDelta(dt) - game.player.ComputeDelta(dt));
                if (sweep.hit != null)
                {
                    Position = sweep.pos + sweep.hit.normal;
                    Speed = 0;
                }
                else
                {
                    Speed = MaxSpeed;
                    Direction = Utils.normalise(lineOfSight);
                }
            }
            else // bliski napad
            {
                Speed = 0;
                Attack(new Damage(HitPoints, DamageType.Enemy, this, 5, 30, Direction));
            }
        }
        else // igrač nije "vidljiv" neprijatelju, pa on stane na mjestu
        {
            Speed = 0;
        }

        switch (rightFacing)
        {
            case true when Direction.X < 0:
                rightFacing = false;
                FlipVertical();
                break;
            case false when Direction.X > 0:
                rightFacing = true;
                FlipVertical();
                break;
        }
        
        base.Update(dt);
    }

    public override void TakeDamage(Damage damage)
    {
        game.SplatterBlood(Position);
        health -= damage.hitPoints;
        base.TakeDamage(damage);
    }
}