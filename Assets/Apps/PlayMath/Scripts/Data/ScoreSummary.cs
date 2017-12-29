﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

namespace BuddyApp.PlayMath{
    public class ScoreSummary {
		public int CorrectAnswers { get; set; }
		public int BadAnswers { get; set;}
        public int Difficulty { get; set; }

        [XmlIgnore]
        public TimeSpan TotalAnswerTime { get; set; }

        // Pretend property for serialization
        [XmlElement("TotalAnswerTime")]
        public long TotalAnswerTimeTicks
        {
            get { return TotalAnswerTime.Ticks; }
            set { TotalAnswerTime = new TimeSpan(value); }
        }

        private double SuccessPercent() {
			return ( (double) this.CorrectAnswers / (this.CorrectAnswers + this.BadAnswers) ); 
		}

		/// <summary>
		/// Compare 2 objects
		/// </summary>
		/// <returns><c>true</c>, if this > scoreSummary, <c>false</c> otherwise.</returns>
		/// <param name="scoreSummary">Score summary to compare</param>
		public bool BetterThan(ScoreSummary scoreSummary) {
			double lDiff = this.SuccessPercent() - scoreSummary.SuccessPercent();

			if (lDiff > 0) {
				return true;
			} else if (lDiff == 0) {
				return (this.TotalAnswerTime.CompareTo(scoreSummary.TotalAnswerTime) < 0);
			} else {
				return false;
			}
		}

		public override string ToString () {
			return string.Format ("ScoreSummary: CorrectAnswers={0}, BadAnswers={1}, TotalAnswerTime={2}, Difficulty={3}"
				, CorrectAnswers, BadAnswers, TotalAnswerTime, Difficulty);
		}
	}
}