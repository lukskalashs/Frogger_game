using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class Vehicle : MovingObject
    {
        //Vehicle represents the actual cars/trucks.

        public Vehicle(Texture2D texture, Vector2 position, int width, int height, float speed, int direction, int windowWidth)
            : base(texture, position, width, height, speed, direction, windowWidth)
        {
        }
    }
}
