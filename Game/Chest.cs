using Dungeon.sprites;
using SFML.System;

namespace Dungeon.Game;

public class Chest : Entity
{
    private bool opened = false;
    public int CoinAmount { get; private set; }

    public Chest(Game game, Sprite sprite, Vector2f position, int coinAmount = 10) : base(game, sprite)
    {
        Position = position;
        CoinAmount = coinAmount;
        DestroyOnDeath = true;
        health = 1;
    }

    public override void TakeDamage(Damage damage)
    {
        if (damage.type == DamageType.Player && !opened)
        {
            opened = true;
            Collidable = false;
            game.Gold += CoinAmount;
            game.Score += CoinAmount;

            health -= damage.hitPoints;
            base.TakeDamage(damage);
        }
    }
}