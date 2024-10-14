using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon.Menu
{
    internal class IronHelmet : Shield
    {
        public IronHelmet(Vector2f position) : base(position, new Texture(Path.Combine("assets", "helmet.png")))
        {
            ExtraLives = 1;
            Price = 30;
        }
        public IronHelmet() : this(new Vector2f(0, 0)) { }
    }
}
