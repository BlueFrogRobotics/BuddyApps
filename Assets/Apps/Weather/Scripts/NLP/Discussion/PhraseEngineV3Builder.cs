using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BuddyApp.Weather
{
	public class PhraseEngineV3Builder
	{
		/// <summary>
		/// The categories.
		/// </summary>
		public List<String> categories = null;


		/// <summary>
		/// Initializes a new instance of the <see cref="AppBuddyApp.WordEmbedding.NLP.Discussion.PhraseEngineV3Builder"/> class.
		/// </summary>
		public PhraseEngineV3Builder ()
		{
			
			categories = new List<string> ();
		}


		/// <summary>
		/// Builds the phrase engine.
		/// </summary>
		/// <returns>The phrase engine.</returns>
		/// <param name="vocabulary">Vocabulary.</param>
		/// <param name="categoryPath">Category path.</param>
		/// <param name="genericPath">Generic path.</param>
		/// <param name="processSentence">Process sentence.</param>
		/// <param name="stopwords">Stopwords.</param>
		public PhraseEngineV3 BuildPhraseEngine(Vocabulary vocabulary, string categoryPath, string genericCategory, List<String> listCategories, PhraseEngineV3.ProcessSentence processSentence, string stopwords = "") {
			this.categories = listCategories;

			PhraseEngine phraseEngineV1 = new PhraseEngine (vocabulary);
			PhraseEngineV2 phraseEngineV2 = new PhraseEngineV2 ();
			PhraseEngineV3 phraseEngineV3 = new PhraseEngineV3 ();

			phraseEngineV3.PhraseEngine = phraseEngineV1;
			phraseEngineV3.PhraseEngineV2 = phraseEngineV2;
			phraseEngineV3.Vocabulary = vocabulary;
			phraseEngineV3.sentencesActypes = new Dictionary<string, Dictionary<string, string>> ();
			phraseEngineV3.representations = new Dictionary<string, Representation> ();
			phraseEngineV3.chunkActype = new Dictionary<string, Dictionary<string, Act>> ();
			phraseEngineV3.processSentence = processSentence;
			phraseEngineV3.GenericCategory = genericCategory;
			phraseEngineV1.SentencesRepresentation = phraseEngineV3.representations;
			phraseEngineV2.ChunkRepresentation = phraseEngineV3.representations;



			if (stopwords != "") {
				StopwordBuilder stopwordBuilder = new StopwordBuilder ();
				StopwordProcessor stopwordProcessor = stopwordBuilder.BuildFromTxtFile (stopwords);
				phraseEngineV3.StopwordProcessor = stopwordProcessor;
				phraseEngineV1.StopwordProcessor = stopwordProcessor;
			} else {
				phraseEngineV3.StopwordProcessor = null;
				phraseEngineV1.StopwordProcessor = null;
			} 

			// Looking for each category
			foreach (string category in this.categories) {
				// Sentences, Actypes then Representation
				phraseEngineV3.sentencesActypes.Add(category, this.LoadSentencesActypes (categoryPath + category + "_sentences.csv"));

				foreach (KeyValuePair<string, Dictionary<string, string>> category_sentencesActypes in phraseEngineV3.sentencesActypes) {
					foreach (KeyValuePair<string, string> sentenceActype in category_sentencesActypes.Value) {
						if (!phraseEngineV3.representations.ContainsKey (sentenceActype.Key)) {
							phraseEngineV3.representations.Add(sentenceActype.Key, this.CalculSentenceRepresentation (vocabulary, sentenceActype.Key, processSentence, phraseEngineV3.StopwordProcessor));
						}
					}
				}

				// Chunks, Actypes then Representation
				if (category == genericCategory)
					continue;
				phraseEngineV3.chunkActype.Add(category, this.LoadChunksActypes (categoryPath + category + "_chunks.csv", processSentence));
				foreach (KeyValuePair<string, Dictionary<string, Act>> category_chunksActypes in phraseEngineV3.chunkActype) {
					foreach (KeyValuePair<string, Act> chunkActype in category_chunksActypes.Value) {
						if (!phraseEngineV3.representations.ContainsKey (chunkActype.Key)) {
							phraseEngineV3.representations.Add(chunkActype.Key, this.CalculSentenceRepresentation (vocabulary, chunkActype.Key, processSentence));
						}
					}
				}
			}

			phraseEngineV3.Vocabulary = vocabulary;
			return phraseEngineV3;
		}

		/// <summary>
		/// Loads the sentences actypes corresponding to a given category. TODO
		/// </summary>
		/// <returns>The sentences actypes.</returns>
		/// <param name="category">Category.</param>
		public Dictionary<string, string> LoadSentencesActypes(string sentenceRefPath) {
			Dictionary<string, string> sentencesActypes = new Dictionary<string, string> ();
			CSVReader csvReader = new CSVReader ();
			csvReader.Read (sentenceRefPath);
			string[] line = null;
			Dictionary<string, string> phrasesActype = null;
			while ((line = csvReader.Next ()) != null) {
				if (line.Length == 2) {
					if (!sentencesActypes.ContainsKey (line[0])) {
						sentencesActypes.Add (line [0], line [1]);
					}
				}
			}
			return sentencesActypes;
		}

		/// <summary>
		/// Loads the sentences representation corresponding to a given category. TODO
		/// </summary>
		/// <returns>The sentences representation.</returns>
		/// <param name="category">Category.</param>
		public Representation CalculSentenceRepresentation(Vocabulary vocabulary, string sentence, PhraseEngineV3.ProcessSentence processSentence, StopwordProcessor stopwordProcessor = null) {
			return vocabulary.GetSummRepresentationOrNullForPhrase (processSentence(sentence, stopwordProcessor));
		}


		public Dictionary<string, Act> LoadChunksActypes(string chunkRefPath, PhraseEngineV3.ProcessSentence processSentence) {
			Dictionary<string, Act> chunksActype = new Dictionary<string, Act>();
			CSVReader csvReader = new CSVReader ();
			csvReader.Read (chunkRefPath);
			string[] line = null;


			while ((line = csvReader.Next ()) != null) {
				string [] actLine = new string[line.Length - 1];
				Array.Copy (line, 1, actLine, 0, line.Length-1);
				if (line.Length >= 2) {
					Act act = new Act ();
					act.setAct (actLine);
					string chunk = processSentence (line [0]);

					if (chunk == "" || chunk == " ") { 
						// On a certainement eu un mauvais cas de stopword
						Debug.Log ("Empty chunk : " + act.ToString ());
					} else if (chunksActype.ContainsKey (chunk)) {
						Debug.Log ("chunk already exists :" + chunk + " " + act.ToString () + " | " + chunksActype [chunk].ToString ()); 
					} else {
						chunksActype.Add (chunk, act);
					}
					//Dictionary<string, Representation> chunkRepresentation = null;
				}
			}

			csvReader.Close ();
			return chunksActype;
		}
	}
}


/// Comment : 
/// 
/// J'ai une liste de catégories 
/// On ne fait pas de map, on simplifie la chose. 
/// On cherche les deux fichiers csv de référence, chunk et sentence.
/// On les traite et on les ajoute à la liste. 
/// PhraseEngineV3 aura donc un dictionnaire : categoryName => referenceStructure. 
/// Voilà, bonne chance. 
/// 
/// Plus tard, permettre la configuration des listes par xml ou json : 
/// Il suffira de mettre la liste des catégories et automatiquement il ira chercher les fichiers
/// correspondant. 
/// 

