using System;
using UnityEngine;

namespace BuddyApp.Weather
{
	/// <summary>
	/// Phrase engine builder.
	/// Permet de gérer des instances de PhraseEngine 
	/// à partir des modèles.
	/// </summary>
	public class PhraseEngineV2Builder
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.PhraseEngineBuilder"/> class.
		/// </summary>
		public PhraseEngineV2Builder ()
		{
		}

		/// <summary>
		/// Builds the phrase engine.
		/// </summary>
		/// <returns>The phrase engine.</returns>
		/// <param name="modelPath">Model path.</param>
		/// <param name="wordRefPath">Word reference path.</param>
		/// <param name="stopwords">Stopwords.</param>
		public PhraseEngineV2 BuildPhraseEngine(string modelPath, string chunkRefPath, string stopwords = "") {
			return this.BuildPhraseEngine (new Word2VecBinaryReader ().Read (modelPath), chunkRefPath, stopwords);
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
		public PhraseEngineV2 BuildPhraseEngine(Vocabulary vocabulary, string chunkRefPath, string stopwords = "") {
			PhraseEngineV2 phraseEngine = new PhraseEngineV2(vocabulary);

			if (stopwords != "") {
				StopwordBuilder stopwordBuilder = new StopwordBuilder();
				StopwordProcessor stopwordProcessor = stopwordBuilder.BuildFromTxtFile (stopwords);
				phraseEngine.StopwordProcessor = stopwordProcessor;
			}

			CSVReader csvReader = new CSVReader ();
			csvReader.Read (chunkRefPath);
			string[] line = null;


			while ((line = csvReader.Next ()) != null) {
				string [] actLine = new string[line.Length - 1];
				Array.Copy (line, 1, actLine, 0, line.Length-1);
				if (line.Length >= 2) {
					Act act = new Act ();
					act.setAct (actLine);
					phraseEngine.AddChunkRef (phraseEngine.ProcessPhrase(line[0]), act);
				}
			}

			csvReader.Close ();
			return phraseEngine;
		}

	}
}

