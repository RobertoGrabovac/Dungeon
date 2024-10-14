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
    internal class StoneSword : Sword
    {
        public StoneSword(Vector2f position) : base(position, new Texture(Path.Combine("assets", "stoneSword.png")))
        {
            Damage = 2;
            Price = 50;
        }
        public StoneSword() : this(new Vector2f(0, 0)) { }
    }
}
