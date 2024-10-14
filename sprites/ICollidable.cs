using Dungeon.sprites.hitboxes;
using SFML.System;

namespace Dungeon.sprites;

public interface ICollidable
{
    public AABB hitbox { get; set; }
    public Circle hitcircle { get; set; }
    public Vector2f Position { get; set; }
    public Vector2f Direction { get; set; }
    public float Speed { get; set; }
}