using Dungeon.sprites;
using SFML.Graphics;
using SFML.System;

namespace Dungeon.Game;

public class Blood : Entity
{
    private static readonly Texture bloodTexture = new Texture(Path.Combine("assets", "blood.png"));
    private bool aboutToDestroy;
    
    public Blood(Game game, Vector2f location) : base(game, bloodTexture, new Vector2i(),
        new Vector2i(64, 64), 4, 0.04f)
    {
        Position = location;
        Collidable = false;
        Scale = new Vector2f(0.5f, 0.5f);
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (currentFrame == nFrames - 1)
        {
            aboutToDestroy = true;
        }
        else if (aboutToDestroy)
        {
            game.DestroyEntity(this);
        }
    }
}