using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class GameConfig
    {
        public int WindowWidth { get; set; } = 1000;
        public int LaneHeight { get; set; } = 77;
        public float FrogHopDuration { get; set; } = 0.12f;
        public int StartingLives { get; set; } = 3;

        public float PowerUpMinSpawnInterval { get; set; } = 10f;
        public float PowerUpMaxSpawnInterval { get; set; } = 20f;
        public float PowerUpLifespan { get; set; } = 8f; 
        public float TimeFreezeDuration { get; set; } = 4f;
        public float SuperJumpDuration { get; set; } = 8f;


        //Scores and UI

        public string SplashPowerUpWarning { get; set; } = "\"With greater super powers come great responsibility...\"";
        public string SplashTitle { get; set; } = "FROGGER";
        public string SplashInstructions { get; set; } = "Use WASD/Arrows to move.\nPress SPACE to Start!";
        public string GameOverText { get; set; } = "GAME OVER";
        public string RestartText { get; set; } = "Press SPACE to Play Again";
    }
}
