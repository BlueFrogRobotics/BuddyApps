using System;

namespace BuddyApp.Weather
{
	
	/// <summary>
	/// Actype representation.
	/// </summary>
	public struct ActypeRepresentation {
		
		/// <summary>
		/// The phrase.
		/// </summary>
		public string phrase;

		/// <summary>
		/// The actype.
		/// </summary>
		public string actype;

		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.ActypeRepresentation"/> struct.
		/// </summary>
		/// <param name="phrase">Phrase.</param>
		/// <param name="actype">Actype.</param>
		public ActypeRepresentation(string phrase, string actype) {
			this.phrase = phrase;
			this.actype = actype;
		}
	}

}

