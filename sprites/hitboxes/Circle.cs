using SFML.System;

namespace Dungeon.sprites.hitboxes;

/**
 * Ova klasa služi samo tzv. broadphase testiranju sudara: brže je provjeriti koji su sve mogući kandidati za sudar
 * ako se koriste njihove opisane kružnice jer je kod za testiranje presjeka kružnica kraći i ne sadrži grananja, za
 * razliku od intersectAABB rutine za hitboxove.
 */
public class Circle
{
    public float radius { get; set; }
    public Vector2f center { get; set; }

    public Circle(float radius=0, Vector2f center=new Vector2f())
    {
        this.radius = radius;
        this.center = center;
    }

    public bool intersects(Circle other)
    {
        float radsum = radius + other.radius;
        float distance = Utils.normSquared(center - other.center);
        
        return (distance < radsum * radsum);
    }
}