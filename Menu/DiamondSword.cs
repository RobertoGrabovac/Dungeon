using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dungeon.sprites;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;

namespace Dungeon.Menu
{
    internal class DiamondSword : Sword
    {
        public DiamondSword(Vector2f position) : base(position, new Texture(Path.Combine("assets", "diamondSword.png")))
        {
            Damage = 3;
            Price = 100;
        }
        public DiamondSword() : this(new Vector2f(0, 0)) { }
    }
}

