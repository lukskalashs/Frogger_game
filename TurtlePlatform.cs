using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class TurtlePlatform : Platform
    {
        private int _state = 0; // 0: Surfaced, 1: Shrinking, 2: Submerged, 3: Surfacing
        private float _stateTimer = 0f;
        private float _animTimer = 0f;

        // Starts fully surfaced (Row 4, which is index 3)
        private int _currentFrame = 3;
        private int _texWidth;
        private int _texHeight;

        // Unsafe ONLY when fully shrunk (index 0)
        public override bool IsSafe => _currentFrame > 0;

        public TurtlePlatform(Texture2D texture, Vector2 position, int width, int height, float speed, int direction, int windowWidth)
            : base(texture, position, width, height, speed, direction, windowWidth, new Rectangle(0, 0, texture.Width, texture.Height / 4))
        {
            _texWidth = texture.Width;
            _texHeight = texture.Height / 4; // 1 column x 4 rows
            UpdateSourceRect();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // Move along the river

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _stateTimer += deltaTime;

            switch (_state)
            {
                case 0: // Surfaced (Wait a few seconds before sinking)
                    if (_stateTimer > 3.0f) { _state = 1; _stateTimer = 0; }
                    break;
                case 1: // Shrinking (Play backwards: 3 -> 2 -> 1 -> 0)
                    _animTimer += deltaTime;
                    if (_animTimer > 0.15f)
                    {
                        _animTimer = 0f;
                        _currentFrame--;
                        if (_currentFrame <= 0) { _currentFrame = 0; _state = 2; _stateTimer = 0; }
                        UpdateSourceRect();
                    }
                    break;
                case 2: // Submerged (Wait underwater)
                    if (_stateTimer > 2.0f) { _state = 3; _stateTimer = 0; }
                    break;
                case 3: // Surfacing (Play forwards: 0 -> 1 -> 2 -> 3)
                    _animTimer += deltaTime;
                    if (_animTimer > 0.15f)
                    {
                        _animTimer = 0f;
                        _currentFrame++;
                        if (_currentFrame >= 3) { _currentFrame = 3; _state = 0; _stateTimer = 0; }
                        UpdateSourceRect();
                    }
                    break;
            }
        }

        private void UpdateSourceRect()
        {
            _sourceRect = new Rectangle(0, _currentFrame * _texHeight, _texWidth, _texHeight);
        }
    }
}
