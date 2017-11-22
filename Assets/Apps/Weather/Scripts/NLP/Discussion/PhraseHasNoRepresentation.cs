using System;

namespace BuddyApp.Weather
{
	public class PhraseHasNoRepresentation : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.PhraseHasNoRepresentation"/> class.
		/// </summary>
		/// <param name="phrase">Phrase.</param>
		public PhraseHasNoRepresentation (string phrase, string processPhrase) : base("la phrase \"" + phrase + "\" n'a pas de représentation.")
		{
			this.ProcessPhrase = processPhrase;	
		}

		public string ProcessPhrase {
			get;
			set;
		}
	}
}

