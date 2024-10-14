using SFML.Graphics;
using SFML.System;

namespace Dungeon.Menu
{
    internal class IronChestplate : Shield
    {
        public IronChestplate(Vector2f position) : base(position, new Texture(Path.Combine("assets", "chestplate.png")))
        {
            ExtraLives = 2;
            Price = 50;
        }
        public IronChestplate() : this(new Vector2f(0, 0)) { }
    }
}
