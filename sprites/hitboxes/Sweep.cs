using SFML.System;

namespace Dungeon.sprites.hitboxes;

/**
 * Sadrži rezultate provjere sudara pomicajućeg AABB-a vs. nepomicajućeg (u trenutnom okviru). Atribut pos
 * predstavlja konačnu dostižnu poziciju u kojoj nema interpenetracija (ako sudara nema, to je onda jednostavno
 * originalna pozicija pomicajućeg objekta + njegov željeni pomak).
 */
public class Sweep
{
    public Hit? hit = null;
    public Vector2f pos = new();
    public float time = 1;
}