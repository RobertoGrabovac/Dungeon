using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;

namespace Dungeon.Menu
{
    internal class Sword : Button
    {
        public int Damage { get; set; }
        public int Price { get; set; }
        public Sword(Vector2f position, Texture? texture = null) : base(position, new Vector2f(100, 100), "", Color.White, Color.Transparent, texture) { }
    }
}
