using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class PowerUp
    {
        public PowerUpType Type;
        public Rectangle Bounds;
        public Texture2D Texture;
        public float Lifespan;
    }
}
