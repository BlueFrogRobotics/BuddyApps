using System;
using System.Collections;
using System.Collections.Generic;


namespace BuddyApp.Weather
{
	/// <summary>
	/// L'objectif de cette classe est de gérer une liste de stopwords 
	/// et de pouvoir appliquer des transformations sur des phrases
	/// en supprimant les stopwords qu'elle contient.
	/// </summary>
	public class StopwordProcessor
	{

		/// <summary>
		/// The dictionary of stopwords.
		/// </summary>
		public Dictionary<string, bool> stopwords;

		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Tools.StopwordProcessor"/> class.
		/// Créer la classe avec une liste de stopwords vide.
		/// </summary>
		public StopwordProcessor() {
			this.stopwords = new Dictionary<string, bool> ();
		}

		/// <summary>
		/// Process the specified phrase.
		/// </summary>
		/// <param name="phrase">Phrase.</param>
		public string Process(string phrase) {
			string[] phraseSplit = phrase.Split (' ');
			return String.Join (" ", this.Process (phraseSplit));
		}

		/// <summary>
		/// Process the specified phrase.
		/// </summary>
		/// <param name="phrase">Phrase.</param>
		/// <returns>Phrase without stopwords</returns>
		public String [] Process(string [] phrase) {
			List<string> newPhrase = new List<string> (); 

			foreach (string word in phrase) {
				if (!this.stopwords.ContainsKey (word)) {
					newPhrase.Add (word);
				}
			}

			return newPhrase.ToArray();
		}

		/// <summary>
		/// Gets the position where we have to delete stopwords.
		/// </summary>
		/// <returns>The delete position.</returns>
		/// <param name="phrase">Phrase.</param>
		public List<int> GetDeletePosition(string [] phrase) {
			List<int> deletePositions = new List<int>();
			for(int i = 0; i < phrase.Length; i++) {
				if (this.stopwords.ContainsKey (phrase[i])) {
					deletePositions.Add(i);
				}
			}
			return deletePositions;
		}

		/// <summary>
		/// Ajoute un mot à la liste de stopwords.
		/// </summary>
		/// <param name="stopword">Stopword.</param>
		public void AddStopword(string stopword) {
			if(!this.stopwords.ContainsKey(stopword))
				this.stopwords.Add(stopword, true);
		}

		/// <summary>
		/// Determines whether this instance has the specified stopword.
		/// </summary>
		/// <returns><c>true</c> if this instance has the specified stopword; otherwise, <c>false</c>.</returns>
		/// <param name="word">Word.</param>
		public bool HasStopword(string word) {
			return this.stopwords.ContainsKey (word);
		}
	}
}