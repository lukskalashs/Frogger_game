using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class Platform : MovingObject
    {
        protected Rectangle _sourceRect;
        // Allows the Frog to know how fast to move when riding

        // Logs and Lily Pads are always safe. Turtles have to override this.
        public virtual bool IsSafe => true;
        public float CurrentVelocity => Speed * Direction;


        public Platform(Texture2D texture, Vector2 position, int width, int height, float speed, int direction, int windowWidth, Rectangle sourceRect)
             : base(texture, position, width, height, speed, direction, windowWidth)
        {
            _sourceRect = sourceRect;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                // Draw only the specific log from the sheet using the SourceRect

                Vector2 origin = new Vector2(_sourceRect.Width / 2f, _sourceRect.Height / 2f);
                Vector2 scale = new Vector2((float)Width / _sourceRect.Width, (float)Height / _sourceRect.Height);
                spriteBatch.Draw(Texture, Position, _sourceRect, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
