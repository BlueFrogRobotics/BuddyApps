using Buddy;

using System.IO;

namespace BuddyApp.RedLightGreenLightGame {

    public class LevelData
    {

        public LevelData()
        {
            Target = new TargetData();
        }

        public int Level { get; set; }

        public bool Swing { get; set; }

        public bool EyesClose { get; set; }

        public float WaitingTime { get; set; }

        public float StartDetectionTime { get; set; }

        public TargetData Target { get; set; }

        //public bool Movement { get; set; }

        //public float Speed { get; set; }

        //public float Size { get; set; }

        public bool Blink { get; set; }

        public float SensibilityThreshold { get; set; }

        public float MotionTolerance { get; set; }

    }

}