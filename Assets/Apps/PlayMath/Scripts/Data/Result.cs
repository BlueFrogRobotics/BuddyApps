using System;
using UnityEngine;

namespace BuddyApp.PlayMath{
    public class Result : MonoBehaviour{
        public string Equation{ get; set;}
        public string CorrectAnswer{ get; set;}
        public string UserAnswer { get; set;}
        public bool Last{ get; set;}
        private DateTime mElapsed;

        public Result()
        {
        }

        public bool isCorrect()
        {
            return (CorrectAnswer == UserAnswer);
        }
    }
}

