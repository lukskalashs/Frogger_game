using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class Frog
    {
        private SpriteAnimator _animator;
        public Vector2 Position { get; private set; }
        private Vector2 _targetPosition;
        private Texture2D _deathTexture;

        private bool _isHopping = false;
        private float _hopTimer = 0f;

        private int _laneHeight;
        private int _windowWidth;
        private int _windowHeight;
        private float _hopDuration;


        //Death State bariables
        public bool IsDead { get; private set; }
        public bool IsReadyToRespawn { get; private set; }
        private float _deathTimer = 0f;

        public bool IsHopping => _isHopping;
        public bool JustLanded { get; private set; }

        //PooweUp State Variables
        public int JumpMultiplier { get; set; } = 1;


        public Rectangle BoundingBox
        {
            get
            {
                int size = (int)(_laneHeight * 0.65f);
                return new Rectangle((int)(Position.X - size / 2f), (int)(Position.Y - size / 2f), size, size);
            }
        }

        public Frog(Texture2D texture, Texture2D deathTexture, Vector2 startPosition, int laneHeight, int windowWidth, int windowHeight, float hopDuration, int msPerFrame)
        {
            _deathTexture = deathTexture;
            Position = startPosition;
            _targetPosition = startPosition;

            _laneHeight = laneHeight;
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _hopDuration = hopDuration;

            _animator = new SpriteAnimator(texture, 3, 8, msPerFrame);
            _animator.SetState(7);
        }

        public void Die()
        {
            if (IsDead) return;
            IsDead = true;
            _isHopping = false; //Stop movement immediately
        }


        public void InitiateHop(Vector2 direction)
        {
            if (_isHopping || direction == Vector2.Zero) return;

            _isHopping = true;
            _hopTimer = 0f;

            // Map direction to the correct animation row (0-indexed)
            if (direction.Y < 0) _animator.SetState(3); // Row 4: Hop up
            else if (direction.Y > 0) _animator.SetState(0); // Row 1: Hop down
            else if (direction.X < 0) _animator.SetState(1); // Row 2: Hop left
            else if (direction.X > 0) _animator.SetState(2); // Row 3: Hop right

            _targetPosition = new Vector2(
            Position.X + (direction.X * _laneHeight * JumpMultiplier), // Multiply by JumpMultiplier! SuperPower Jump!
            Position.Y + (direction.Y * _laneHeight * JumpMultiplier)
        );

            // Clamp bounds so the frog cannot leave the screen
            _targetPosition.X = MathHelper.Clamp(_targetPosition.X, _laneHeight / 2f, _windowWidth - (_laneHeight / 2f));
            _targetPosition.Y = MathHelper.Clamp(_targetPosition.Y, _laneHeight / 2f, _windowHeight - (_laneHeight / 2f));
        }

        public void Update(GameTime gameTime)
        {
            JustLanded = false;


            // Handle death timer and animation timeline
            if (IsDead)
            {
                _deathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

              
                if (_deathTimer >= 1.25f)
                {
                    IsReadyToRespawn = true;
                }
                return;
            }

            if (_isHopping)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _hopTimer += deltaTime;

                float progress = MathHelper.Clamp(_hopTimer / _hopDuration, 0f, 1f);
                Position = Vector2.Lerp(Position, _targetPosition, progress);

                if (progress >= 1f)
                {
                    _isHopping = false;
                    Position = _targetPosition;
                    JustLanded = true; //Triggers exactly when the jump finishes!!!!!
                }
            }

            _animator.Update(gameTime, _isHopping);
            // Handle death timer instead of movement
            if (IsDead)
            {
                _deathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_deathTimer >= 1.5f) // Display skeleton for 1.5 seconds
                {
                    IsReadyToRespawn = true;
                }
                return;
            }

            if (_isHopping)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _hopTimer += deltaTime;

                float progress = MathHelper.Clamp(_hopTimer / _hopDuration, 0f, 1f);
                Position = Vector2.Lerp(Position, _targetPosition, progress);

                if (progress >= 1f)
                {
                    _isHopping = false;
                    Position = _targetPosition;
                }
            }

            _animator.Update(gameTime, _isHopping);
        }

        public void Ride(float velocityX, float deltaTime)
        {
            if (IsDead || _isHopping) return;

            // Move the frog at the exact speed of the log it is standing on
            Position = new Vector2(Position.X + (velocityX * deltaTime), Position.Y);
            _targetPosition = Position; //  in sync

            // Frog dies if it is carried off-screen while riding a platform
            if (Position.X < 0 || Position.X > _windowWidth)
            {
                Die();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsDead && _deathTexture != null)
            {
                // scaling 580 by 411 out of the center whole image
                Rectangle sourceRect = new Rectangle(49, 20, 580, 411);
                Vector2 origin = new Vector2(sourceRect.Width / 2f, sourceRect.Height / 2f);

                //fit exactly one lane width (77px)
                float baseScale = 77f / sourceRect.Width;
                float currentScale = baseScale;
                float alpha = 1f;

               
                if (_deathTimer < 0.15f)
                {
                    //Pop-i
                    float progress = _deathTimer / 0.15f;
                    currentScale = baseScale * MathHelper.Lerp(0.5f, 1.2f, progress);
                }
                else if (_deathTimer < 0.25f)
                {
                    // Settle
                    float progress = (_deathTimer - 0.15f) / 0.10f;
                    currentScale = baseScale * MathHelper.Lerp(1.2f, 1.0f, progress);
                }
                else if (_deathTimer >= 0.95f)
                {
                    // Fade out
                    float progress = (_deathTimer - 0.95f) / 0.30f;
                    alpha = MathHelper.Lerp(1f, 0f, progress);
                }

                spriteBatch.Draw(_deathTexture, Position, sourceRect, Color.White * alpha, 0f, origin, currentScale, SpriteEffects.None, 0f);
            }
            else
            {
                _animator.Draw(spriteBatch, Position);
            }
        }
    }

}
