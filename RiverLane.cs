using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class RiverLane
    {
        public List<Platform> Platforms { get; set; } = new List<Platform>();

        private Texture2D _platformTexture;
        private List<Rectangle> _sourceRects;
        private List<float> _aspectRatios;
        private IRandomProvider _random;
        private PlatformType _type;
        private List<Rectangle> _logSourceRects;
        private List<float> _logAspectRatios;
        private Texture2D _texture;

        private float _yPosition;
        private int _direction;
        private int _windowWidth;
        private int _laneHeight;

        //Ratio variables for Logs/Turtles
        //private float _widthRatio;
        private float _heightRatio;

        private float _spawnTimer;
        private float _minInterval, _maxInterval;
        private float _laneSpeed;

        public RiverLane(PlatformType type, Texture2D texture, IRandomProvider random, float yPosition, int direction, int windowWidth, int laneHeight, float minInterval, 
                         float maxInterval, float minSpeed, float maxSpeed, List<Rectangle> logRects = null, List<float> logAspects = null)
        {
            _type = type;
            _texture = texture;
            _random = random;
            _yPosition = yPosition;
            _direction = direction;
            _windowWidth = windowWidth;
            _laneHeight = laneHeight;

            _logSourceRects = logRects;
            _logAspectRatios = logAspects;

            _minInterval = minInterval;
            _maxInterval = maxInterval;

            _laneSpeed = minSpeed + (_random.NextFloat() * (maxSpeed - minSpeed));
            SetNextSpawnTime();
        }
        private void SetNextSpawnTime()
        {
            _spawnTimer = _minInterval + (_random.NextFloat() * (_maxInterval - _minInterval));
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _spawnTimer -= deltaTime;

            if (_spawnTimer <= 0)
            {
                SpawnPlatform();
                SetNextSpawnTime();
            }

            for (int i = Platforms.Count - 1; i >= 0; i--)
            {
                Platforms[i].Update(gameTime);
                if (Platforms[i].IsOffScreen)
                {
                    Platforms.RemoveAt(i);
                }




            }
        }

        private void SpawnPlatform()
        {
            if (_type == PlatformType.Log)
            {
                int index = _random.Next(0, _logSourceRects.Count);
                Rectangle sourceRect = _logSourceRects[index];
                float aspect = _logAspectRatios[index];

                int height = (int)(_laneHeight * 0.62f);
                int width = (int)(height * aspect);
                float spawnX = _direction == 1 ? -(width / 2f) : _windowWidth + (width / 2f);

                Platforms.Add(new Platform(_texture, new Vector2(spawnX, _yPosition), width, height, _laneSpeed, _direction, _windowWidth, sourceRect));
            }
            else if (_type == PlatformType.Turtle)
            {
                //Spawn a group of 3 turtles together
                int size = (int)(_laneHeight * 0.62f); //Turtle size ratio
                float spawnX = _direction == 1 ? -(size / 2f) : _windowWidth + (size / 2f);

                for (int i = 0; i < 3; i++)
                {
                    float offsetX = _direction == 1 ? -(i * size) : (i * size);
                    Platforms.Add(new TurtlePlatform(_texture, new Vector2(spawnX + offsetX, _yPosition), size, size, _laneSpeed, _direction, _windowWidth));
                }
            }
            else if (_type == PlatformType.LilyPad)
            {
                int width = (int)(_laneHeight * 0.94f);
                int height = (int)(_laneHeight * 0.55f);
                float spawnX = _direction == 1 ? -(width / 2f) : _windowWidth + (width / 2f);

                //Spawns the LilyPadPlatform so it can animate
                Platforms.Add(new LilyPadPlatform(_texture, new Vector2(spawnX, _yPosition), width, height, _laneSpeed, _direction, _windowWidth));
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var platform in Platforms)
            {
                platform.Draw(spriteBatch);
            }
        }
    }
}

