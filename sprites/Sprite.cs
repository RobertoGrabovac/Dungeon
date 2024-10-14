using Dungeon.sprites.hitboxes;
using SFML.Graphics;
using SFML.System;

namespace Dungeon.sprites;

/**
 * Klasa koja predstavlja animirani sprite. Pretpostavljamo da postoji samo jedna "animacija" tj. niz sličica koje
 * je potrebno prikazivati u ciklusu, gdje svaka sličica mora biti prikazana frameLength puta.
 * Te se sličice učitavaju iz slike koja ih sadrži raspoređene uzastopno od neke
 * početne lokacije donjeg lijevog ruba, s poznatim i fiksnim širinama i visinama svake u nizu.
 */
public class Sprite : Transformable, Drawable, ICollidable
{
    public static readonly float sqrt2o2 = (float)Math.Sqrt(2) * 0.5f;
    
    public Texture texture;
    protected SFML.Graphics.Sprite sprite = new SFML.Graphics.Sprite();
    public AABB hitbox { get; set; }
    public Circle hitcircle { get; set; }
    public Vector2f Direction { get; set; }
    public float Speed { get; set; }
    
    protected int textureBeginX;
    protected int textureBeginY;
    public readonly int textureWidth, textureHeight;

    protected int currentFrame = 0;
    protected readonly int nFrames;
    protected readonly float frameLength;
    protected float frameCounter = 0;
    public bool Visible { get; set; } = true;

    public new Vector2f Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            hitbox.pos = value;
            hitcircle.center = value;
        }
    }

    public Sprite(Sprite other)
    {
        texture = other.texture;
        sprite = other.sprite;
        nFrames = other.nFrames;
        frameLength = other.frameLength;
        hitbox = other.hitbox;
        hitcircle = other.hitcircle;
        textureBeginX = other.textureBeginX;
        textureBeginY = other.textureBeginY;
        textureWidth = other.textureWidth;
        textureHeight = other.textureHeight;
        PrepareCollisionData();
    }
    
    public Sprite(Texture texture, Vector2i textureBegin, Vector2i textureSize, int nFrames = 1, float frameLength = 0)
    {
        this.texture = texture;
        this.nFrames = nFrames;
        this.frameLength = frameLength;
        this.textureWidth = textureSize.X;
        this.textureHeight = textureSize.Y;
        this.textureBeginX = textureBegin.X * textureSize.X;
        this.textureBeginY = textureBegin.Y * textureSize.Y;
        sprite.Texture = texture;
        sprite.TextureRect = new IntRect(textureBeginX, textureBeginY, textureWidth, textureHeight);
        sprite.Origin = new Vector2f(textureWidth * 0.5f, textureHeight * 0.5f);
        PrepareCollisionData();
    }

    private void PrepareCollisionData()
    {
        hitbox = new AABB(this, Position, new Vector2f(textureWidth * 0.5f, textureHeight * 0.5f));
        float max = Math.Max(textureWidth, textureHeight);
        hitcircle = new Circle(sqrt2o2 * max, Position);
    }

    public void FlipHorizontal()
    {
        Scale = new Vector2f(Scale.X, -Scale.Y);
    }

    public void FlipVertical()
    {
        Scale = new Vector2f(-Scale.X, Scale.Y);
    }

    public void Animate(float dt)
    {
        if (frameLength == 0) return;
        if (nFrames <= 1) return;

        frameCounter -= dt;
        if (frameCounter <= 0)
        {
            currentFrame += 1;
            currentFrame %= nFrames;
            SetAnimationFrame(currentFrame);
            frameCounter = frameLength;
        }
    }
    protected void SetAnimationFrame(int frame)
    {
        sprite.TextureRect = new IntRect(textureBeginX + textureWidth * frame, textureBeginY, textureWidth, textureHeight);
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        //animate();
        if (!Visible) return;
        states.Transform *= Transform;
        target.Draw(sprite, states);
    }
}