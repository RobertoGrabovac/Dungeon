using Dungeon.sprites;
using SFML.System;
using Sprite = Dungeon.sprites.Sprite;

namespace Dungeon.Game;

public class Skeleton : Enemy
{
    public Skeleton(Game game, Sprite sprite, float maxSpeed, float hitPoints, float health) : base(game, sprite, maxSpeed, health, hitPoints)
    {
    }
}