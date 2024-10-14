using Dungeon.sprites;
using SFML.Graphics;
using Sprite = Dungeon.sprites.Sprite;

namespace Dungeon.Game;

public class Torch : Entity
{
    public bool isLit = false;

    public Torch(Dungeon.Game.Game game, Sprite other) : base(game, other)
    {
        lightOff();
    }

    public void lightOn()
    {
        game.Score += 100;
        isLit = true;
        sprite.TextureRect = new IntRect(textureBeginX, textureBeginY, textureWidth, textureHeight);
    }

    public void lightOff()
    {
        isLit = false;
        sprite.TextureRect = new IntRect(textureBeginX + textureWidth, textureBeginY, textureWidth, textureHeight);
    }

    public override void TakeDamage(Damage damage)
    {
        if (damage.type == DamageType.Player && !isLit)
        {
            lightOn();
            game.TorchesLit++;
        }
    }
}
