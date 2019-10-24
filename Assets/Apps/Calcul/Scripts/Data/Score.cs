using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Calcul
{
    public class Score
    {

        private int mCorrectAnswers;

        private TimeSpan mTotalAnswerTime; // Use mTotalAnswerTime.ToString("format")

		public List<Result> Results{ get; private set;}

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
            Results.Clear();
        }

        public void AddResult(Result lResult)
        {
            Results.Add(lResult);

            TimeSpan lAnswerTime = TimeSpan.FromSeconds(lResult.ElapsedTime);
            mTotalAnswerTime += lAnswerTime;

            if (lResult.isCorrect())
                mCorrectAnswers++;
        }

        public Result LastResult()
        {
            if (Results.Count > 0)
                return Results[Results.Count - 1];
            return null;
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

