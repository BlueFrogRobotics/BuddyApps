using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Xml.Serialization;
using Buddy;

namespace BuddyApp.PlayMath{
	[Serializable]
	public class ScoreSummaryList {

		private const int NBR_LEVELS = 5;

		private const int SCORES_FOR_ONE_LEVEL_COUNT_MAX = 4;

		private const string FILE_TO_SERIALIZE = "score_summary_list";

		public List<ScoreSummary>[] ScoresByLevels { get; set; }

		public ScoreSummaryList() {
			this.ScoresByLevels = new List<ScoreSummary>[NBR_LEVELS];
			for (int i = 0; i < NBR_LEVELS; i++) {
				this.ScoresByLevels[i] = new List<ScoreSummary>();
			}
		}

		/// <summary>
		/// Try to store a new score. The score will be stored, only if it's one of the best
		/// </summary>
		/// <param name="score">Score to store</param>
		public void NewScore(Score score) {
			ScoreSummary lScoreSummary = score.ToScoreSummary();

			List<ScoreSummary> lScores = ScoresByLevels[lScoreSummary.Difficulty - 1];

			// find the index score
			int i = 0;
			while (i < lScores.Count) {
				if (lScoreSummary.BetterThan(lScores[i])) {
					break;
				}

				i++;
			}

			if (i >= SCORES_FOR_ONE_LEVEL_COUNT_MAX) {
				// score no too good to be stored
				return;
			}

			lScores.Insert(i, lScoreSummary);

			if (lScores.Count == (SCORES_FOR_ONE_LEVEL_COUNT_MAX + 1)) {
				// the last score is > SCORES_COUNT_MAX
				lScores.RemoveAt(lScores.Count - 1);
			}
		}

		public override string ToString() {
			StringBuilder lSb = new StringBuilder();

			for (int i = 0; i < NBR_LEVELS; i++) {
				lSb.Append("[");
				bool lFirst = true;

				List<ScoreSummary> lScores = this.ScoresByLevels[i];
				for (int j = 0; j < lScores.Count; j++) {
					if (lFirst) {
						lFirst = false;
					} 
					else {
						lSb.Append(",");
					}

					lSb.Append(lScores[j].ToString());
				}

				lSb.Append("]");
			}

			return lSb.ToString();
		}

		public static void SaveDefault(ScoreSummaryList scoreSummaryList) {
			Utils.SerializeXML<ScoreSummaryList>(scoreSummaryList, PathToSerialize());
		}

		public static ScoreSummaryList LoadDefault() {
			ScoreSummaryList lScores = Utils.UnserializeXML<ScoreSummaryList>(PathToSerialize());

			if (lScores == null) {
				lScores = new ScoreSummaryList();
			}			

			return lScores;
		}

		private static string PathToSerialize() {
			return BYOS.Instance.Resources.GetPathToRaw(FILE_TO_SERIALIZE);
		}
	}
}
