using Dungeon.sprites;
using Sprite = Dungeon.sprites.Sprite;

namespace Dungeon.Game;

public class Ghost : Enemy
{
    public Ghost(Game game, Sprite sprite, float maxSpeed, float hitPoints, float health) : base(game, sprite, maxSpeed,
        health, hitPoints)
    {
    }
}