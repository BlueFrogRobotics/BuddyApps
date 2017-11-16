using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Weather
{
	/// <summary>
	/// Phrase score comparer.
	/// </summary>
	public class IPhraseScoreComparer : IComparer 
	{
		/// <summary>
		/// Compare the specified right and left.
		/// </summary>
		/// <param name="right">Right.</param>
		/// <param name="left">Left.</param>
		int IComparer.Compare(Object right, Object left) {
			DictionaryEntry entry = ((DictionaryEntry)right);
			double r = (double)((DictionaryEntry)right).Value;
			double l = (double)((DictionaryEntry)left).Value;

			// * -1 => décroissant
			// * 1 => croissant
			return (-1)*(Comparer<double>.Default.Compare(r, l));
		}
	}
}

