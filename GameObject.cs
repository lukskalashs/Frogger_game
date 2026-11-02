using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        // Automatically generates a collision hitbox based on the center position
        public Rectangle BoundingBox => new Rectangle(
            (int)(Position.X - Width / 2f),
            (int)(Position.Y - Height / 2f),
            Width, Height);

        public GameObject(Texture2D texture, Vector2 position, int width, int height)
        {
            Texture = texture;
            Position = position;
            Width = width;
            Height = height;
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);


                // Dynamically scale the texture to the injected Width/Height to avoid hard-coded sprittig sizes
                Vector2 scale = new Vector2((float)Width / Texture.Width, (float)Height / Texture.Height);

                spriteBatch.Draw(Texture, Position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
