using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class GoalRow
    {
        private Texture2D _tussockTex;
        private Texture2D _flyTex;
        private IRandomProvider _random;
        private float _flyLifetime;



        private Rectangle _shrubSource = new Rectangle(9, 9, 84, 84);
        private Rectangle _slotSource = new Rectangle(102, 9, 84, 84);

        public List<GoalSlot> Slots { get; private set; } = new List<GoalSlot>();

        private float _flyTimer;
        private int _tileSize;
        private int _laneHeight;

        public GoalRow(Texture2D tussockTex, Texture2D flyTex, IRandomProvider random, int windowWidth, int laneHeight, float flyLifetime)
        {
            _tussockTex = tussockTex;
            _flyTex = flyTex;
            _random = random;
            _laneHeight = laneHeight;
            _tileSize = windowWidth / 8;

            // Create 8 tiles: Shrub, Slot, Shrub, Slot kinda situation
            for (int i = 0; i < 8; i++)
            {
                Slots.Add(new GoalSlot
                {
                    Bounds = new Rectangle(i * _tileSize, 0, _tileSize, _tileSize),
                    IsShrub = (i % 2 == 0), //Even indexes
                    HasFly = false
                });
            }
            SpawnFly();
            _flyLifetime = flyLifetime;
        }

        public void Update(GameTime gameTime)
        {
            // Flystays for 6 seconds then moves to a new slot
            _flyTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_flyTimer <= 0)
            {
                SpawnFly();
            }
        }

        public void SpawnFly()
        {
            // Clear existing
            foreach (var slot in Slots) slot.HasFly = false;

            //Pick a random Slot (odd indexes: 1, 3, 5, 7 are the water pools)
            int slotIndex = (_random.Next(0, 4) * 2) + 1;
            Slots[slotIndex].HasFly = true;
            _flyTimer = _flyLifetime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var slot in Slots)
            {
                Rectangle source = slot.IsShrub ? _shrubSource : _slotSource;
                spriteBatch.Draw(_tussockTex, slot.Bounds, source, Color.White);

                if (slot.HasFly)
                {
                    // Fly is 0.36 ratio of LaneHeight
                    int flySize = (int)(_laneHeight * 0.36f);
                    Rectangle flyBounds = new Rectangle(
                        slot.Bounds.Center.X - flySize / 2,
                        slot.Bounds.Center.Y - flySize / 2,
                        flySize, flySize);
                    spriteBatch.Draw(_flyTex, flyBounds, Color.White);
                }
            }
        }
    }
}
