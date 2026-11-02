using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Frogger
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState _currentState = GameState.Splash;

        //Systeming
        private InputManager _inputManager;
        private IRandomProvider _randomProvider;
        private GameConfig _config;
        private BackgroundManager _background;
        private PowerUpManager _powerUpManager;

        // Entities
        private Frog _player;
        private List<VehicleLane> _trafficLanes = new List<VehicleLane>();
        private List<RiverLane> _riverLanes = new List<RiverLane>();


        // Textures
        private Texture2D _frogTex;
        private Texture2D _carLeftTex, _carRightTex, _truckLeftTex, _truckRightTex, _frogDeathTex, _logTex, _turtleTex, _lilypadTex, _tussockTex, _flyTex, _roadTex, _waterTex, _timeFreezeTex, _superJumpTex;

        private Vector2 _frogStartPosition;

        private GoalRow _goalRow;
        private int _score = 0;

        private SpriteFont _font;
        private int _lives = 3;
        private int _fliesEaten = 0;
        private int _highestRowReached = 1; //Start grass is Row 1

        private int PointsPerForwardHop { get; set; } = 10;
        private int PointsPerFly { get; set; } = 50;

        private float _timeFreezeTimer = 0f;
        private float _superJumpTimer = 0f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _config = new GameConfig();


            //Set exact window size derived from config rules
            // 10 lanes * 77px + 125px goal row = 895px total height
            _graphics.PreferredBackBufferWidth = _config.WindowWidth;
            _graphics.PreferredBackBufferHeight = 895;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _randomProvider = new DefaultRandomProvider();
            _inputManager = new InputManager();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here


            _font = Content.Load<SpriteFont>("File");

            _frogTex = Content.Load<Texture2D>("Frog");

            
            _carLeftTex = Content.Load<Texture2D>("car-truck1_move_left(red)");
            _carRightTex = Content.Load<Texture2D>("car-truck1_move_right(red)");
            _truckLeftTex = Content.Load<Texture2D>("truck5_left(yellow bus)");
            _truckRightTex = Content.Load<Texture2D>("truck5_right(yellow bus)");
            _frogDeathTex = Content.Load<Texture2D>("Frog_dies");
            _logTex = Content.Load<Texture2D>("Log Light");
            _turtleTex = Content.Load<Texture2D>("Turtle_shrink_in");
            _lilypadTex = Content.Load<Texture2D>("Lily pad");
            _tussockTex = Content.Load<Texture2D>("a tussock (or a hummock)");
            _flyTex = Content.Load<Texture2D>("fly");
            _roadTex = Content.Load<Texture2D>("Road");
            _waterTex = Content.Load<Texture2D>("Water(River)");
            _timeFreezeTex = Content.Load<Texture2D>("Fly Time Freeze(powerUp)");
            _superJumpTex = Content.Load<Texture2D>("super jump(PowerUp)");

            var leftFacingVehicles = new List<Texture2D> { _carLeftTex, _truckLeftTex };
            var rightFacingVehicles = new List<Texture2D> { _carRightTex, _truckRightTex };

            //Calculate starting position (Row 1: Bottom Start Grass)
            _frogStartPosition = new Vector2(
                _config.WindowWidth / 2f,
                895 - (_config.LaneHeight / 2f)
            );

            // Initialize Player (passing 100ms per frame for the animation)
            _player = new Frog(_frogTex, _frogDeathTex, _frogStartPosition, _config.LaneHeight, _config.WindowWidth, 895, _config.FrogHopDuration, 100);

            // Initialize Traffic Lanes (Rows 2 to 5)
            //Lane 1 (Row 2, closest to start): Moves Right (1)
            float lane1Y = 895 - _config.LaneHeight - (_config.LaneHeight / 2f);
            _trafficLanes.Add(new VehicleLane(rightFacingVehicles, _randomProvider, lane1Y, 1, _config.WindowWidth, _config.LaneHeight, 2.0f, 4.5f, 80f, 160f));

            float lane2Y = 895 - (2 * _config.LaneHeight) - (_config.LaneHeight / 2f);
            _trafficLanes.Add(new VehicleLane(leftFacingVehicles, _randomProvider, lane2Y, -1, _config.WindowWidth, _config.LaneHeight, 2.5f, 5.0f, 100f, 200f));

            float lane3Y = 895 - (3 * _config.LaneHeight) - (_config.LaneHeight / 2f);
            _trafficLanes.Add(new VehicleLane(rightFacingVehicles, _randomProvider, lane3Y, 1, _config.WindowWidth, _config.LaneHeight, 1.5f, 3.5f, 90f, 110f));

            float lane4Y = 895 - (4 * _config.LaneHeight) - (_config.LaneHeight / 2f);
            _trafficLanes.Add(new VehicleLane(leftFacingVehicles, _randomProvider, lane4Y, -1, _config.WindowWidth, _config.LaneHeight, 2.0f, 4.0f, 120f, 160f));


            var logRects = new List<Rectangle> {
                new Rectangle(12, 46, 428, 66), new Rectangle(15, 182, 295, 66), //Logs
                new Rectangle(18, 286, 124, 66), new Rectangle(193, 286, 65, 66)
            };

            var logAspects = new List<float> { 6.5f, 4.5f, 1.9f, 1.0f }; // Corresponding aspect ratios

            // Stream 1: Logs moving Right
            float river1Y = 895 - (6 * _config.LaneHeight) - (_config.LaneHeight / 2f);
            _riverLanes.Add(new RiverLane(PlatformType.Log, _logTex, _randomProvider, river1Y, 1, _config.WindowWidth, _config.LaneHeight, 2.5f, 5.0f, 70f, 110f, logRects, logAspects));

            // Stream 2: Turtles moving Left
            float river2Y = 895 - (7 * _config.LaneHeight) - (_config.LaneHeight / 2f);
            _riverLanes.Add(new RiverLane(PlatformType.Turtle, _turtleTex, _randomProvider, river2Y, -1, _config.WindowWidth, _config.LaneHeight, 3.5f, 6.0f, 80f, 120f));

            // Stream 3: Logs moving Right
            float river3Y = 895 - (8 * _config.LaneHeight) - (_config.LaneHeight / 2f);
            _riverLanes.Add(new RiverLane(PlatformType.Log, _logTex, _randomProvider, river3Y, 1, _config.WindowWidth, _config.LaneHeight, 2.0f, 4.5f, 100f, 140f, logRects, logAspects));

            // Stream 4: Lily pads moving Left
            float river4Y = 895 - (9 * _config.LaneHeight) - (_config.LaneHeight / 2f);
            _riverLanes.Add(new RiverLane(PlatformType.LilyPad, _lilypadTex, _randomProvider, river4Y, 1, _config.WindowWidth, _config.LaneHeight, 3.0f, 5.5f, 90f, 130f));

            //To win
            // Passing 60second(60f) at the end sets the fly to stay for a full minute
            _goalRow = new GoalRow(_tussockTex, _flyTex, _randomProvider, _config.WindowWidth, _config.LaneHeight, 10f);
            _background = new BackgroundManager(_roadTex, _waterTex, _config.WindowWidth, _config.LaneHeight);
            _powerUpManager = new PowerUpManager(_timeFreezeTex, _superJumpTex, _randomProvider, _config.WindowWidth, _config.LaneHeight,
                                                 _config.PowerUpMinSpawnInterval, _config.PowerUpMaxSpawnInterval, _config.PowerUpLifespan);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            KeyboardState kb = Keyboard.GetState();

            //STATE:SPLASH SCREEN OR GAME OVER
            if (_currentState == GameState.Splash || _currentState == GameState.GameOver)
            {
                if (kb.IsKeyDown(Keys.Space))
                {
                    // Reset game stats and start
                    _score = 0;
                    _lives = _config.StartingLives;
                    _fliesEaten = 0;
                    _highestRowReached = 1;
                    _player = new Frog(_frogTex, _frogDeathTex, _frogStartPosition, _config.LaneHeight, _config.WindowWidth, 895, _config.FrogHopDuration, 100);
                    _currentState = GameState.Playing;
                }
                return;
            }

            // STATE: PLAYING
            //(Only if alive)
            if (!_player.IsDead)
            {
                Vector2 inputDirection = _inputManager.GetMovementDirection();
                _player.InitiateHop(inputDirection);

                // Track Forward Progress for Points
                int currentRow = (int)((895 - _player.Position.Y) / _config.LaneHeight) + 1;
                if (currentRow > _highestRowReached && currentRow <= 11)
                {
                    _score += PointsPerForwardHop; // +10 points
                    _highestRowReached = currentRow;
                }
            }

            _player.Update(gameTime);

            // Check if ready to respawn AFTER death animation
            if (_player.IsReadyToRespawn)
            {
                _lives--;
                if (_lives <= 0)
                {
                    _currentState = GameState.GameOver; 
                }
                else
                {
                    _player = new Frog(_frogTex, _frogDeathTex, _frogStartPosition, _config.LaneHeight, _config.WindowWidth, 895, _config.FrogHopDuration, 100);
                    _highestRowReached = 1; //Reset row tracker for the new life
                }
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Manage Power-Up Timers
            if (_timeFreezeTimer > 0) _timeFreezeTimer -= dt;
            if (_superJumpTimer > 0)
            {
                _superJumpTimer -= dt;
                _player.JumpMultiplier = 2; // Jump 2 lanes
            }
            else
            {
                _player.JumpMultiplier = 1; // Normality
            }

            // Update PowerUps & Check the frogPickups
            _powerUpManager.Update(gameTime);
            for (int i = _powerUpManager.ActivePowerUps.Count - 1; i >= 0; i--)
            {
                if (_player.BoundingBox.Intersects(_powerUpManager.ActivePowerUps[i].Bounds))
                {
                    if (_powerUpManager.ActivePowerUps[i].Type == PowerUpType.TimeFreeze)
                        _timeFreezeTimer = 6f; //Freeze traffic for 6 seconds
                    else
                        _superJumpTimer = 4f; //Super Jump lasts 8 seconds

                    _powerUpManager.ActivePowerUps.RemoveAt(i);
                    _score += 25; // Bonus points for picking up! JUST FOR FUN!
                }
            }


            //Update Traffic & Check for the <<Collisions>>
            foreach (var lane in _trafficLanes)
            {
                if (_timeFreezeTimer <= 0)
                {
                    lane.Update(gameTime);
                }
                foreach (var vehicle in lane.Vehicles)
                {
                    Rectangle frogBox = _player.BoundingBox;
                    frogBox.Inflate(-10, -10);
                    Rectangle vehicleBox = vehicle.BoundingBox;
                    vehicleBox.Inflate(-5, -5);

                    if (!_player.IsDead && frogBox.Intersects(vehicleBox))
                    {
                        _player.Die();
                        break;
                    }
                }
            }

            //River Logic and Riding check
            float medianTopBoundary = 895 - (6 * _config.LaneHeight);
            bool inRiverZone = _player.Position.Y < medianTopBoundary && _player.Position.Y > (895 - 10 * _config.LaneHeight);
            bool isRiding = false;

            foreach (var lane in _riverLanes)
            {
                if (_timeFreezeTimer <= 0)
                {
                    lane.Update(gameTime);
                }
                foreach (var platform in lane.Platforms)
                {
                    if (Math.Abs(_player.Position.Y - platform.Position.Y) < 5)
                    {
                        if (_player.Position.X >= platform.Position.X - platform.Width / 2f &&
                            _player.Position.X <= platform.Position.X + platform.Width / 2f)
                        {
                            if (platform.IsSafe)
                            {
                                isRiding = true;
                                if (!_player.IsHopping)
                                {
                                    //Change to use 0 velocity when frozen:
                                    float currentVel = _timeFreezeTimer > 0 ? 0f : platform.CurrentVelocity;
                                    _player.Ride(currentVel, (float)gameTime.ElapsedGameTime.TotalSeconds);
                                    if (_player.JustLanded && platform is LilyPadPlatform lilyPad)
                                    {
                                        lilyPad.TriggerBob();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (inRiverZone && !isRiding && !_player.IsHopping && !_player.IsDead)
            {
                _player.Die();
            }

            //Goal Row Update & Check
            _goalRow.Update(gameTime);

            if (_player.Position.Y <= 125 && !_player.IsHopping && !_player.IsDead)
            {
                foreach (var slot in _goalRow.Slots)
                {
                    if (_player.Position.X >= slot.Bounds.Left && _player.Position.X <= slot.Bounds.Right)
                    {
                        if (slot.IsShrub || !slot.HasFly)
                        {
                            _player.Die(); //Landed on shrub or empty slot
                        }
                        else
                        {
                            _fliesEaten++;
                            _score += PointsPerFly; //+50 points
                            _goalRow.SpawnFly();
                            _highestRowReached = 1;
                            _player = new Frog(_frogTex, _frogDeathTex, _frogStartPosition, _config.LaneHeight, _config.WindowWidth, 895, _config.FrogHopDuration, 100);
                        }
                        break;
                    }
                }
            }





            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(40, 40, 40));

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            if (_currentState == GameState.Playing)
            {
                //Draw Game World
                _background.Draw(_spriteBatch);
                _goalRow.Draw(_spriteBatch);
                foreach (var lane in _trafficLanes) lane.Draw(_spriteBatch);
                foreach (var lane in _riverLanes) lane.Draw(_spriteBatch);
                _powerUpManager.Draw(_spriteBatch);
                _player.Draw(_spriteBatch);

                //Draw HUD Overlay
                if (_font != null)
                {
                    _spriteBatch.DrawString(_font, $"Score: {_score}", new Vector2(10, 10), Color.White);
                    _spriteBatch.DrawString(_font, $"Lives: {_lives}", new Vector2(10, 40), Color.White);
                    _spriteBatch.DrawString(_font, $"Flies Eaten: {_fliesEaten}", new Vector2(10, 70), Color.White);
                }
            }
            else if (_currentState == GameState.Splash)
            {
                if (_font != null)
                {
                    //Draw Splash Screen
                    Vector2 titleSize = _font.MeasureString(_config.SplashTitle);
                    _spriteBatch.DrawString(_font, _config.SplashTitle, new Vector2((_config.WindowWidth - titleSize.X) / 2, 300), Color.Yellow);

                    Vector2 instSize = _font.MeasureString(_config.SplashInstructions);
                    _spriteBatch.DrawString(_font, _config.SplashInstructions, new Vector2((_config.WindowWidth - instSize.X) / 2, 400), Color.White);
                    Vector2 warnSize = _font.MeasureString(_config.SplashPowerUpWarning);
                    _spriteBatch.DrawString(_font, _config.SplashPowerUpWarning, new Vector2((_config.WindowWidth - warnSize.X) / 2, 460), Color.Orange);
                }
            }
            else if (_currentState == GameState.GameOver)
            {
                if (_font != null)
                {
                    Vector2 overSize = _font.MeasureString(_config.GameOverText);
                    _spriteBatch.DrawString(_font, _config.GameOverText, new Vector2((_config.WindowWidth - overSize.X) / 2, 250), Color.Red);

                    string stats = $"Final Score: {_score}\nFlies Eaten: {_fliesEaten}";
                    Vector2 statsSize = _font.MeasureString(stats);
                    _spriteBatch.DrawString(_font, stats, new Vector2((_config.WindowWidth - statsSize.X) / 2, 350), Color.White);

                    Vector2 restartSize = _font.MeasureString(_config.RestartText);
                    _spriteBatch.DrawString(_font, _config.RestartText, new Vector2((_config.WindowWidth - restartSize.X) / 2, 500), Color.Yellow);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);

          
        }
    }
}
