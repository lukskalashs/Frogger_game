using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class VehicleLane
    {
        public List<Vehicle> Vehicles { get; private set; } = new List<Vehicle>();

        private List<Texture2D> _vehicleTextures;
        private IRandomProvider _random;

        private float _yPosition;
        private int _direction; // 1 (Left to Right) or -1 (Right to Left)
        private int _windowWidth;
        private int _laneHeight;

        private float _spawnTimer;
        private float _minSpawnInterval;
        private float _maxSpawnInterval;
        private float _minSpeed;
        private float _maxSpeed;

        private float _laneSpeed;



        public VehicleLane(List<Texture2D> textures, IRandomProvider random, float yPosition, int direction, int windowWidth, int laneHeight, float minInterval, float maxInterval, float minSpeed, float maxSpeed)
        {
            _vehicleTextures = textures;
            _random = random;
            _yPosition = yPosition;
            _direction = direction;
            _windowWidth = windowWidth;
            _laneHeight = laneHeight;

            _minSpawnInterval = minInterval;
            _maxSpawnInterval = maxInterval;
            _minSpeed = minSpeed;
            _maxSpeed = maxSpeed;

            SetNextSpawnTime();

            float randomFloat = _random.NextFloat();
            _laneSpeed = minSpeed + (randomFloat * (maxSpeed - minSpeed));
        }

        private void SetNextSpawnTime()
        {
            float randomFloat = _random.NextFloat();
            _spawnTimer = _minSpawnInterval + (randomFloat * (_maxSpawnInterval - _minSpawnInterval));
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _spawnTimer -= deltaTime;

            if (_spawnTimer <= 0)
            {
                SpawnVehicle();
                SetNextSpawnTime();
            }

            for (int i = Vehicles.Count - 1; i >= 0; i--)
            {
                Vehicles[i].Update(gameTime);
                if (Vehicles[i].IsOffScreen)
                {
                    Vehicles.RemoveAt(i);
                }
            }
        }

        private void SpawnVehicle()
        {
            
            int texIndex = _random.Next(0, _vehicleTextures.Count);
            Texture2D tex = _vehicleTextures[texIndex];

            // Size sprites as a ratio of LaneHeight (Car width ratio: 1.25, height: 0.62)
            int width = (int)(_laneHeight * 1.25f);
            int height = (int)(_laneHeight * 0.62f);

            //Spawn just off-screen based on direction
            float spawnX = _direction == 1 ? -(width / 2f) : _windowWidth + (width / 2f);
            Vector2 startPos = new Vector2(spawnX, _yPosition);

            //Random speed within the injected range
            float randomFloat = _random.NextFloat();
            float speed = _minSpeed + (randomFloat * (_maxSpeed - _minSpeed));

            Vehicles.Add(new Vehicle(tex, startPos, width, height, _laneSpeed, _direction, _windowWidth));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var vehicle in Vehicles)
            {
                vehicle.Draw(spriteBatch);
            }
        }
    }
}
