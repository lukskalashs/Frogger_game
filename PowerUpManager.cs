using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class PowerUpManager
    {
        private Texture2D _timeFreezeTex;
        private Texture2D _superJumpTex;
        private IRandomProvider _random;
        private int _medianY;
        private int _size;
        private int _windowWidth;
        private float _minSpawnInterval;
        private float _maxSpawnInterval;
        private float _lifespan;
        public List<PowerUp> ActivePowerUps { get; private set; } = new List<PowerUp>();
        private float _spawnTimer = 5f;

        public PowerUpManager(Texture2D timeFreezeTex, Texture2D superJumpTex, IRandomProvider random, int windowWidth, int laneHeight, float minSpawnInterval, float maxSpawnInterval, float lifespan)
        {
            _timeFreezeTex = timeFreezeTex;
            _superJumpTex = superJumpTex;
            _random = random;
            _windowWidth = windowWidth;
            _size = (int)(laneHeight * 0.6f);
            _medianY = 895 - (6 * laneHeight) + (laneHeight / 2) - (_size / 2);

            // Store injected values
            _minSpawnInterval = minSpawnInterval;
            _maxSpawnInterval = maxSpawnInterval;
            _lifespan = lifespan;

            _spawnTimer = _minSpawnInterval + (_random.NextFloat() * (_maxSpawnInterval - _minSpawnInterval));
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _spawnTimer -= dt;

            //Spawn a new power-up every 10 to 20 seconds
            if (_spawnTimer <= 0 && ActivePowerUps.Count < 2)
            {
                PowerUpType type = _random.NextFloat() > 0.5f ? PowerUpType.TimeFreeze : PowerUpType.SuperJump;
                Texture2D tex = type == PowerUpType.TimeFreeze ? _timeFreezeTex : _superJumpTex;
                int x = _random.Next(50, _windowWidth - 50);

                // Use the injected _lifespan
                ActivePowerUps.Add(new PowerUp { Type = type, Texture = tex, Bounds = new Rectangle(x, _medianY, _size, _size), Lifespan = _lifespan });

                // Use the injected intervals
                _spawnTimer = _minSpawnInterval + (_random.NextFloat() * (_maxSpawnInterval - _minSpawnInterval));
            }

            // Remove old ones
            for (int i = ActivePowerUps.Count - 1; i >= 0; i--)
            {
                ActivePowerUps[i].Lifespan -= dt;
                if (ActivePowerUps[i].Lifespan <= 0) ActivePowerUps.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var p in ActivePowerUps) spriteBatch.Draw(p.Texture, p.Bounds, Color.White);
        }
    }
}
