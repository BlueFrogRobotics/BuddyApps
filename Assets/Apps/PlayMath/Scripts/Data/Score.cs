using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
    public class Score : MonoBehaviour {

        private int mCorrectAnswers;

        private DateTime mDateTime; // Use mDateTime.ToString("MM/dd/yyyy HH:mm")
        private TimeSpan mTotalAnswerTime; // Use mTotalAnswerTime.ToString("format")

        public List<Result> Results{ get; }

        public int CorrectAnswers{ get{ return mCorrectAnswers; } }
        public int BadAnswers{ get { return (mGameParams.Sequence - mCorrectAnswers); } }

        private GameParameters mGameParams;

        public Score()
        {
            Results = new List<Result>();
        }

        public void ResetScore()
        {
            mGameParams = User.Instance.GameParameters;
            mCorrectAnswers = 0;
            mTotalAnswerTime = TimeSpan.Zero;
            mDateTime = DateTime.Now;
            Results.Clear();
        }

        public void AddResult(Result lResult)
        {
            Results.Add(lResult.Clone());

            TimeSpan lAnswerTime = TimeSpan.FromSeconds(lResult.ElapsedTime);
            mTotalAnswerTime += lAnswerTime;

            if (lResult.isCorrect())
                mCorrectAnswers++;
        }

        public double SuccessPercent()
        {
            return ( (double) mCorrectAnswers/mGameParams.Sequence ); 
        }

        public bool IsPerfect()
        {
            return (mCorrectAnswers == mGameParams.Sequence);
        }

		public ScoreSummary ToScoreSummary() 
		{
			ScoreSummary lScoreSummary = new ScoreSummary();

			lScoreSummary.BadAnswers = this.BadAnswers;
			lScoreSummary.CorrectAnswers = this.CorrectAnswers;
			lScoreSummary.Difficulty = this.mGameParams.Difficulty;
			lScoreSummary.TotalAnswerTime = this.mTotalAnswerTime;

			return lScoreSummary;
		}
    }
}

