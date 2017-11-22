using System;

namespace BuddyApp.Weather
{
	/// <summary>
	/// Phrase engine builder.
	/// Permet de gérer des instances de PhraseEngine 
	/// à partir des modèles.
	/// </summary>
	public class PhraseEngineBuilder
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.PhraseEngineBuilder"/> class.
		/// </summary>
		public PhraseEngineBuilder ()
		{
		}

		/// <summary>
		/// Construction d'une instance de PhraseEngine. 
		/// Il se sert d'un modèle word2vec (Binary à l'occurence)
		/// Des modèles textes existent aussi mais leur cas n'est pas géré ici. 
		/// Il se sert aussi du fichier avec la liste des phrases de référence 
		/// et leur représentation en actype. 
		/// </summary>
		/// <returns>The phrase engine.</returns>
		/// <param name="modelPath">Model path.</param>
		/// <param name="phraseRefPath">Phrase reference path.</param>
		/// <returns>L'instance générée de PhraseEngine</returns>
		public PhraseEngine BuildPhraseEngine(string modelPath, string phraseRefPath, string stopwords = "") {
			return this.BuildPhraseEngine (new Word2VecBinaryReader ().Read (modelPath), phraseRefPath, stopwords);
		}

		/// <summary>
		/// Builds the phrase engine.
		/// </summary>
		/// <returns>The phrase engine.</returns>
		/// <param name="vocabulary">Vocabulary.</param>
		/// <param name="phraseRefPath">Phrase reference path.</param>
		/// <param name="stopwords">Stopwords.</param>
		public PhraseEngine BuildPhraseEngine(Vocabulary vocabulary, string phraseRefPath, string stopwords = "") {
			PhraseEngine phraseEngine = new PhraseEngine(vocabulary);

			if (stopwords != "") {
				StopwordBuilder stopwordBuilder = new StopwordBuilder();
				StopwordProcessor stopwordProcessor = stopwordBuilder.BuildFromTxtFile (stopwords);
				phraseEngine.StopwordProcessor = stopwordProcessor;
			}

			CSVReader csvReader = new CSVReader ();
			csvReader.Read (phraseRefPath);
			string[] line = null;

			while ((line = csvReader.Next ()) != null) {
				if (line.Length == 2) {
					phraseEngine.AddPhraseRef (line[0], line[1]);
				}
			}
			csvReader.Close ();
			return phraseEngine;

		}
	}
}

