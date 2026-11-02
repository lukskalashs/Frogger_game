using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class BackgroundManager
    {
        private Texture2D _roadTex;
        private Texture2D _waterTex;
        private int _windowWidth;
        private int _laneHeight;

        // Source rectangles sliced from Road.png
        private Rectangle _bottomGrassSource = new Rectangle(0, 851, 1000, 77); // Approx 858-928
        private Rectangle _roadSource = new Rectangle(0, 547, 1000, 308);       //4 lanes * 77 = 308
        private Rectangle _medianGrassSource = new Rectangle(0, 320, 1000, 77); //Approx 320-540

        public BackgroundManager(Texture2D roadTex, Texture2D waterTex, int windowWidth, int laneHeight)
        {
            _roadTex = roadTex;
            _waterTex = waterTex;
            _windowWidth = windowWidth;
            _laneHeight = laneHeight;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw Bottom Start Grass (Row 1)
            int startGrassY = 895 - _laneHeight;
            Rectangle startGrassDest = new Rectangle(0, startGrassY, _windowWidth, _laneHeight);
            spriteBatch.Draw(_roadTex, startGrassDest, _bottomGrassSource, Color.White);

            //Draw the 4 Traffic Lanes (Rows 2 to 5)
            int roadHeight = _laneHeight * 4;
            int roadY = 895 - (_laneHeight * 5);
            Rectangle roadDest = new Rectangle(0, roadY, _windowWidth, roadHeight);
            spriteBatch.Draw(_roadTex, roadDest, _roadSource, Color.White);

            //Draw Median Grass (Row 6)
            int medianY = 895 - (_laneHeight * 6);
            Rectangle medianDest = new Rectangle(0, medianY, _windowWidth, _laneHeight);
            spriteBatch.Draw(_roadTex, medianDest, _medianGrassSource, Color.White);

            // 4. Draw Water for the River (Rows 7 to 10)
            // The water texture can be stretched without distortion
            int waterHeight = _laneHeight * 4;
            int waterY = 895 - (_laneHeight * 10); // Starts right under the goal row (125px)
            Rectangle waterDest = new Rectangle(0, waterY, _windowWidth, waterHeight);
            spriteBatch.Draw(_waterTex, waterDest, null, Color.White);
        }
    }
}
