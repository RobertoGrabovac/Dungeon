using SFML.Graphics;
using SFML.System;

namespace Dungeon.sprites;

/**
 * Klasa koja predstavlja entitete tj. objekte u igri. Oni se prikazuju kao spriteovi i mogu se pomicati.
 */
public class Entity : Sprite
{
    protected Game.Game game; // referenca na stanje igre koje nam omogućuje testiranje sudara s drugim objektima i sl.
    public bool DestroyOnDeath { get; protected init; } = true;
    public bool Collidable { get; set; } = true;
    public float health = 3;
    public int baseDamage = 1;
    public float attackRadius = 50;
    public float damageRadius = 2;

    protected float attackCooldown = 0.5f;
    protected float attackCounter = 0;

    protected int score = 0;

    public Entity(Game.Game game, Texture texture, Vector2i textureBegin, Vector2i textureSize, int nFrames, float frameLength)
        : base(texture, textureBegin, textureSize, nFrames, frameLength)
    {
        this.game = game;
    }

    public Entity(Game.Game game, Sprite other)
        : base(other)
    {
        this.game = game;
    }

    public void Attack(Damage damage)
    {
        if (attackCounter >= 0) return;
        attackCounter = attackCooldown;

        var collisions = game.TestCollision(damage, damage.attackRay, 0, true);
        // odgovorit ćemo samo na 1 moguću koliziju jer damage se ne može "odbijati" tj. ima samo jednu šansu udara nečeg
        if (collisions[0] == null || collisions[0] == this) return;
        collisions[0]?.TakeDamage(damage);
    }

    public virtual void TakeDamage(Damage damage)
    {
        if (health <= 0)
        {
            if (DestroyOnDeath)
            {
                game.Score += score;
                game.DestroyEntity(this);
            }
        }
    }

    public virtual void Update(float dt)
    {
        Animate(dt);
        if (Collidable)
            Move(dt); // objekti s kojima se ne može sudarati su u pravilu nepomični i ne bi trebali provjeravati kolizije i sl.

        if (attackCounter >= 0)
        {
            attackCounter -= dt;
        }
    }

    public virtual void Move(float dt)
    {
        Vector2f delta = ComputeDelta(dt);
        game.TestCollision(this, delta, dt);
    }

    public virtual Vector2f ComputeDelta(float dt)
    {
        return Direction * Speed * dt;
    }

    public void Rotate(float angle) // kut je u stupnjevima
    {
        Transform.Rotate(angle); // ovo je nužno jer želimo(?) zarotirati i sprite
        var v = new Vector2f(1.0f, 0.0f);
        var rotation = new Transform();
        rotation.Rotate(angle);
        Direction = rotation.TransformPoint(v);
    }
}