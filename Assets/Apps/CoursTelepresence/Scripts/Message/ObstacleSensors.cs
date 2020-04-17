using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.CoursTelepresence
{
    public class ObstacleSensors
    {

        public float obstacleRight;
        public float obstacleCenter;
        public float obstacleLeft;

        public ObstacleSensors(float iObstacleRight, float iObstacleCenter, float iObstacleLeft)
        {
            obstacleRight = iObstacleRight;
            obstacleCenter = iObstacleCenter;
            obstacleLeft = iObstacleLeft;
        }

    }
}
