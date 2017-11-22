using System;
using System.Collections;

namespace BuddyApp.Weather
{
	public interface IPhraseEngine
	{
		/// <summary>
		/// Gets the sim actypes.
		/// </summary>
		/// <returns>The sim actypes. ArrayList with keys of string (actype) and float(coeff of similarity)</returns>
		/// <param name="phrase">Phrase.</param>
		ArrayList getSimActypes (string phrase);
	}
}

