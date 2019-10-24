using System;
using UnityEngine;

namespace BuddyApp.Calcul
{
    public class Result
    {
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

        public Result Clone()
        {
            Result lResult = new Result();
            lResult.mElapsed = this.mElapsed;
            lResult.Equation = this.Equation;
            lResult.CorrectAnswer = this.CorrectAnswer;
            lResult.UserAnswer = this.UserAnswer;
            lResult.Last = this.Last;

            return lResult;
        }
    }
}

