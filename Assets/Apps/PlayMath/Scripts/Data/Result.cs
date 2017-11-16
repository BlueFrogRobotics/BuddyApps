using System;
using UnityEngine;

namespace BuddyApp.PlayMath{
    public class Result : MonoBehaviour{

        private TimeSpan mElapsed;

        public string Equation{ get; set;}
        public string CorrectAnswer{ get; set;}
        public string UserAnswer { get; set;}
        public bool Last{ get; set;}

        public double ElapsedTime {
            get{ return mElapsed.TotalSeconds; }
            set{ mElapsed = TimeSpan.FromSeconds(value); }
        }

        public Result()
        {
        }

        public bool isCorrect()
        {
            return (CorrectAnswer == UserAnswer);
        }
    }
}

