using SFML.System;

namespace Dungeon.sprites.hitboxes;

/**
 * Kod za robustnu detekciju sudara; port od
 * https://noonat.github.io/intersect/#sweep-tests
 * Ova klasa implementira Axis-Aligned Bounding Box tj. pravokutnik čije su stranice paralelne koordinatnim osima.
 * Budući da u pravilu ne rotiramo spriteove u igri, ovo je dovoljno za preciznu detekciju kolizije, a može biti
 * korisno i za razne druge provjere stvari koje ovise o relativnom položaju entiteta.
 */
public class AABB
{
    public Vector2f pos;
    public Vector2f half;
    public readonly object owner;

    public AABB(object owner, Vector2f pos, Vector2f half)
    {
        this.pos = pos;
        this.half = half;
        this.owner = owner;
    }

    public Hit? intersectPoint(Vector2f point)
    {
        float dx = point.X - pos.X;
        float px = half.X - Math.Abs(dx);
        if (px <= 0)
            return null;

        float dy = point.Y - pos.Y;
        float py = half.Y - Math.Abs(dy);
        if (py <= 0)
            return null;

        var hit = new Hit(this);
        if (px < py)
        {
            float sx = Math.Sign(dx);
            hit.delta.X = px * sx;
            hit.normal.X = sx;
            hit.pos.X = pos.X + half.X * sx;
            hit.pos.Y = point.Y;
        }
        else
        {
            float sy = Math.Sign(dy);
            hit.delta.Y = py * sy;
            hit.normal.Y = sy;
            hit.pos.X = point.X;
            hit.pos.Y = pos.Y + half.Y * sy;
        }

        return hit;
    }

    public Hit? intersectSegment(Vector2f pos, Vector2f delta, float paddingX = 0, float paddingY = 0)
    {
        float scaleX = 1.0f / delta.X;
        float scaleY = 1.0f / delta.Y;
        float signX = Math.Sign(scaleX);
        float signY = Math.Sign(scaleY);
        float nearTimeX = (this.pos.X - signX * (half.X + paddingX) - pos.X) * scaleX;
        float nearTimeY = (this.pos.Y - signY * (half.Y + paddingY) - pos.Y) * scaleY;
        float farTimeX = (this.pos.X + signX * (half.X + paddingX) - pos.X) * scaleX;
        float farTimeY = (this.pos.Y + signY * (half.Y + paddingY) - pos.Y) * scaleY;
        if (nearTimeX > farTimeY || nearTimeY > farTimeX)
            return null;

        float nearTime = nearTimeX > nearTimeY ? nearTimeX : nearTimeY;
        float farTime = farTimeX < farTimeY ? farTimeX : farTimeY;
        if (nearTime >= 1 || farTime <= 0)
            return null;

        var hit = new Hit(this)
        {
            time = Math.Clamp(nearTime, 0, 1)
        };
        if (nearTimeX > nearTimeY)
        {
            hit.normal.X = -signX;
            hit.normal.Y = 0;
        }
        else
        {
            hit.normal.X = 0;
            hit.normal.Y = -signY;
        }

        hit.delta.X = (1.0f - hit.time) * -delta.X;
        hit.delta.Y = (1.0f - hit.time) * -delta.Y;
        hit.pos.X = pos.X + delta.X * hit.time;
        hit.pos.Y = pos.Y * delta.Y * hit.time;
        return hit;
    }

    public Hit? intersectAABB(AABB box)
    {
        float dx = box.pos.X - pos.X;
        float px = box.half.X + half.X - Math.Abs(dx);
        if (px <= 0)
            return null;

        float dy = box.pos.Y - pos.Y;
        float py = box.half.Y + half.Y - Math.Abs(dy);
        if (py <= 0)
            return null;

        var hit = new Hit(this);
        if (px < py)
        {
            float sx = Math.Sign(dx);
            hit.delta.X = px * sx;
            hit.normal.X = sx;
            hit.pos.X = pos.X + half.X * sx;
            hit.pos.Y = box.pos.Y;
        }
        else
        {
            float sy = Math.Sign(dy);
            hit.delta.Y = py * sy;
            hit.normal.Y = sy;
            hit.pos.X = box.pos.X;
            hit.pos.Y = pos.Y + half.Y * sy;
        }

        return hit;
    }

    public Sweep sweepAABB(AABB box, Vector2f delta)
    {
        const float EPSILON = 1e-1f;
        
        var sweep = new Sweep();
        if (delta is { X: 0, Y: 0 })
        {
            sweep.pos = box.pos;
            sweep.hit = intersectAABB(box);
            sweep.time = sweep.hit != null ? (sweep.hit.time = 0) : 1;
            return sweep;
        }

        sweep.hit = intersectSegment(box.pos, delta, box.half.X, box.half.Y);
        if (sweep.hit != null)
        {
            sweep.time = Math.Clamp(sweep.hit.time - EPSILON, 0, 1);
            sweep.pos = box.pos + delta * sweep.time;
            //sweep.pos = pos + delta * sweep.time + sweep.hit.normal;
            var direction = delta;
            direction = Utils.normalize(direction);
            sweep.hit.pos.X = Math.Clamp(sweep.hit.pos.X + direction.X * box.half.X, pos.X - half.X, pos.X + half.X);
            sweep.hit.pos.Y = Math.Clamp(sweep.hit.pos.Y + direction.Y * box.half.Y, pos.Y - half.Y, pos.Y + half.Y);
        }
        else
        {
            sweep.pos = box.pos + delta;
            sweep.time = 1;
        }

        return sweep;
    }
}