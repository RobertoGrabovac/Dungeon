using SFML.Graphics;
using SFML.System;
using Color = SFML.Graphics.Color;

namespace Dungeon.Menu
{
    internal class Shield : Button
    {
        public int ExtraLives { get; set; }
        public int Price { get; set; }
        public Shield(Vector2f position, Texture? texture = null) : base(position, new Vector2f(100, 100), "", Color.White, Color.Transparent, texture)
        {

        }
    }
}
