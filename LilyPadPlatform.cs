using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class LilyPadPlatform : Platform
    {
        private int _state = 0; // 0 = Idle, 1 = BobDown, 2 = BobUp
        private int _currentCol = 0;
        private float _animTimer = 0f;
        private int _texWidth;
        private int _texHeight;

        public LilyPadPlatform(Texture2D texture, Vector2 position, int width, int height, float speed, int direction, int windowWidth)
            : base(texture, position, width, height, speed, direction, windowWidth, new Rectangle(0, 0, texture.Width / 6, texture.Height / 3))
        {
            _texWidth = texture.Width / 6;  // 6 columns
            _texHeight = texture.Height / 3; // 3 rows
            UpdateSourceRect();
        }

        public void TriggerBob()
        {
            // Only trigger the animation if it is currently floating idle
            if (_state == 0)
            {
                _state = 1; // Start bobbing down (Row 3)
                _currentCol = 0;
                _animTimer = 0f;
                UpdateSourceRect();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_state != 0) //If an animation is playing
            {
                _animTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_animTimer > 0.08f) //Quick frame rate for the splash/ripple
                {
                    _animTimer = 0f;
                    _currentCol++;

                    if (_currentCol >= 6) //Reached the end of the row
                    {
                        _currentCol = 0;
                        if (_state == 1)
                        {
                            _state = 2; // Tr/nsition to Bob Up (Row 2)
                        }
                        else if (_state == 2)
                        {
                            _state = 0; //eturn to Idle (Row 1)
                        }
                    }
                    UpdateSourceRect();
                }
            }
        }

        private void UpdateSourceRect()
        {
            int row = 0;
            if (_state == 1) row = 2; // Row 3 (Index 2) is Bob Down
            else if (_state == 2) row = 1; // Row 2 (Index 1) is Bob Up

            _sourceRect = new Rectangle(_currentCol * _texWidth, row * _texHeight, _texWidth, _texHeight);
        }
    }
}
