using SFML.System;

namespace Dungeon.sprites.hitboxes;

/**
 * Predstavlja sudar koji se dogodio između dva AABB-a i sadrži sve potrebne informacije o njemu kako bi se sudar
 * mogao razriješiti. Između ostalog, tu je vektor normale stranice AABB-a u koju se provjeravajući objekt sudario,
 * kao i vrijeme sudara (što ustvari predstavlja realan broj t između 0 i 1 uključivo iz jednadžbe zrake željenog pomaka).  
 */
public class Hit
{
    public AABB collider;
    public Vector2f pos;
    public Vector2f delta;
    public Vector2f normal;
    public float time;

    public Hit(AABB collider)
    {
        this.collider = collider;
        this.pos = new Vector2f();
        this.delta = new Vector2f();
        this.normal = new Vector2f();
        this.time = 0;
    }
}