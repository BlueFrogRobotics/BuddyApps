using System;
using System.Collections.Generic;

namespace BuddyApp.Weather
{
	public class StopwordBuilder
	{
		/// <summary>
		/// Un simple constructeur
		/// </summary>
		public StopwordBuilder ()
		{
		}

		/// <summary>
		/// Créer le StopwordProcessor à partir d'un fichier texte.
		/// Chaque ligne est un mot qui alimente le dictionnaire du 
		/// StopwordProcessor.
		/// </summary>
		/// <returns>The stopword processor</returns>
		/// <param name="filePath">File path of stopwords</param>
		public StopwordProcessor BuildFromTxtFile(string filePath) {
			StopwordProcessor stopwordProcessor = new StopwordProcessor();
			string [] words = System.IO.File.ReadAllLines(filePath);
			foreach (string word in words) {
				stopwordProcessor.AddStopword (word);
			}
			return stopwordProcessor;
		}
	}
}

