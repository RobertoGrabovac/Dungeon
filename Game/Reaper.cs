using Dungeon.sprites;
using Sprite = Dungeon.sprites.Sprite;

namespace Dungeon.Game;

public class Reaper : Enemy
{
    public Reaper(Game game, Sprite sprite, float maxSpeed, float hitPoints, float health) : 
        base(game, sprite, maxSpeed, health, hitPoints)
    {
    }
}