using Dungeon.sprites.hitboxes;
using SFML.System;

namespace Dungeon.sprites;

public enum DamageType
{
    Enemy = 0,
    Player = 1,
}


public class Damage : ICollidable
{
    public float hitPoints;
    public DamageType type;
    public readonly Entity origin;
    public readonly float distance;
    public Vector2f direction;
    public readonly Vector2f attackRay;
    public AABB hitbox { get; set; }
    public Circle hitcircle { get; set; }
    public Vector2f Position { get; set; }

    public Vector2f Direction
    {
        get => direction;
        set => direction = value;
    }

    public float Speed { get; set; }

    public Damage(float hitPoints, DamageType type, Entity origin, float radius, float distance, Vector2f direction)
    {
        this.hitPoints = hitPoints;
        this.type = type;
        this.origin = origin;
        this.distance = distance;
        this.direction = direction;
        this.attackRay = direction * distance;
        this.Position = origin.Position + direction * 2*Math.Max(origin.hitbox.half.X+1, origin.hitbox.half.Y+1);
        this.hitbox = new AABB(origin, Position, new Vector2f(radius, radius));
        this.hitcircle = new Circle(radius * 2 * Sprite.sqrt2o2, Position);

    }
}