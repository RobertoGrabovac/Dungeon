using SFML.System;

namespace Dungeon;

public static class Utils
{
    public static float dotProduct(Vector2f a, Vector2f b)
    {
        return a.X * b.X + a.Y * b.Y;
    }
    
    public static Vector2f normalize(Vector2f v)
    {
        Vector2f n = new Vector2f(v.X, v.Y);
        float sum = n.X * n.X + n.Y * n.Y;
        if (sum > 0)
        {
            float len = (float)Math.Sqrt(sum);
            float inv = 1.0f / len;
            n.X *= inv;
            n.Y *= inv;
        }
        else
        {
            n.X = 0;
            n.Y = 0;
        }

        return n;
    }

    public static float normSquared(Vector2f v)
    {
        return v.X * v.X + v.Y * v.Y;
    }

    public static Vector2i GetRandomTexture(Vector2i[] Options)
    {
        Random random = new Random();
        return Options[random.Next(Options.Length)];
    }

    public static float length(Vector2f source)
    {
        return (float) Math.Sqrt(source.X * source.X + source.Y * source.Y);
    }

    //Returns a given vector with its length normalized to 1
    public static Vector2f normalise(Vector2f source)
    {
        float len = length(source);
        if (len != 0) source /= len;
        return source;
    }
}