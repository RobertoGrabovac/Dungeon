using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon.Menu
{
    internal class IronBoots : Shield
    {
        public int Speed { get; }
        public IronBoots(Vector2f position) : base(position, new Texture(Path.Combine("assets", "boots.png")))
        {
            ExtraLives = 0;
            Speed = 10;
            Price = 20;
        }

        public IronBoots() : this(new Vector2f(0, 0)) { }
    }
}
