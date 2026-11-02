using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class SpriteAnimator
    {
        private Texture2D _texture;
        private int _totalCols;
        private int _totalRows;
        private int _currentCol;
        private int _currentRow;
        private double _animationTimer;
        private int _msPerFrame;

        public SpriteAnimator(Texture2D texture, int totalCols, int totalRows, int msPerFrame)
        {
            _texture = texture;
            _totalCols = totalCols;
            _totalRows = totalRows;
            _msPerFrame = msPerFrame;
        }

        public void SetState(int row)
        {
            if (_currentRow != row)
            {
                _currentRow = row;
                _currentCol = 0;
                _animationTimer = 0;
            }
        }

        public void Update(GameTime gameTime, bool isMoving)
        {
            // Col 2 (index 1) is the resting/landed frame
            if (!isMoving)
            {
                _currentCol = 1;
                return;
            }

            _animationTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_animationTimer >= _msPerFrame)
            {
                _currentCol = (_currentCol + 1) % _totalCols;
                _animationTimer = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            int width = _texture.Width / _totalCols;
            int height = _texture.Height / _totalRows;
            Rectangle sourceRect = new Rectangle(_currentCol * width, _currentRow * height, width, height);

            Vector2 origin = new Vector2(width / 2f, height / 2f);
            spriteBatch.Draw(_texture, position, sourceRect, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}

