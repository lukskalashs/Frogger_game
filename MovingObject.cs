using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public abstract class MovingObject : GameObject
    {
        //MovingObject handles velocity and off-screen cleanup

        protected float Speed;
        protected int Direction; // 1 for right, -1 for Lefts
        public bool IsOffScreen { get; private set; }

        private int _windowWidth;

        public MovingObject(Texture2D texture, Vector2 position, int width, int height, float speed, int direction, int windowWidth)
            : base(texture, position, width, height)
        {
            Speed = speed;
            Direction = direction;
            _windowWidth = windowWidth;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move in a straight line at a constant speed
            Position = new Vector2(Position.X + (Speed * Direction * deltaTime), Position.Y);

            // Check if fully off-screen so the lane manager can delete it
            if ((Direction == 1 && Position.X - Width / 2f > _windowWidth) ||
                (Direction == -1 && Position.X + Width / 2f < 0))
            {
                IsOffScreen = true;
            }
        }
    }
}
