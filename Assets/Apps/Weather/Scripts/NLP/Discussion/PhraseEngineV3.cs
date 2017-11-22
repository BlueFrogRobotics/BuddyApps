using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BuddyApp.Weather
{
	public class PhraseEngineV3
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.PhraseEngineV3"/> class.
		/// </summary>
		public PhraseEngineV3 ()
		{
		}

		/// <summary>
		/// Process sentence.
		/// </summary>
		public delegate string ProcessSentence(string sentence, StopwordProcessor stopwordProcessor = null);


		/// <summary>
		/// The process sentence.
		/// </summary>
		public ProcessSentence processSentence = null;

		/// <summary>
		/// The vocabulary.
		/// </summary>
		private StopwordProcessor stopwordProcessor = null;

		public string GenericCategory {
			set;
			get;
		}



		/// <summary>
		/// The vocabulary.
		/// </summary>
		private Vocabulary vocabulary = null;

		/// <summary>
		/// The sentences representation per category
		/// Key is category name. 
		/// </summary>
		public Dictionary<string, Representation> representations = null;

		/// <summary>
		/// Representation in actypes per category
		/// Key is category name.
		/// </summary>
		public Dictionary<string, Dictionary<string, string>> sentencesActypes = null;

		/// <summary>
		/// The chunk actype per category.
		/// </summary>
		public Dictionary<string, Dictionary<string, Act>> chunkActype = null;
			
		/// <summary>
		/// Gets or sets the phrase engine.
		/// </summary>
		/// <value>The phrase engine.</value>
		public PhraseEngine PhraseEngine {
			set;
			get;
		}

		/// <summary>
		/// Gets or sets the phrase engine v2.
		/// </summary>
		/// <value>The phrase engine v2.</value>
		public PhraseEngineV2 PhraseEngineV2 {
			set;
			get;
		}

		/// <summary>
		/// Gets or sets the vocabulary.
		/// </summary>
		/// <value>The vocabulary.</value>
		public Vocabulary Vocabulary {
			set {
				this.vocabulary = value;
				this.PhraseEngine.setVocabulary (value);
				this.PhraseEngineV2.Vocabulary = value;
			}
			get {
				return vocabulary;
			}
		}

		/// <summary>
		/// Gets or sets the stopword processor.
		/// </summary>
		/// <value>The stopword processor.</value>
		public StopwordProcessor StopwordProcessor {
			set {
				this.stopwordProcessor = value;
				this.PhraseEngine.SetStopwordProcessor(value);
				this.PhraseEngineV2.StopwordProcessor = value;
			}
			get {
				return this.stopwordProcessor;
			}
		}

		/// <summary>
		/// Sentences to semantic.
		/// </summary>
		/// <returns>The to semantic.</returns>
		/// <param name="sentence">Sentence.</param>
		/// <param name="clusterThreshold">Cluster threshold.</param>
		/// <param name="penalityMax">Penality max.</param>
		public string SentenceToSemantic(string sentence, double clusterThreshold, double penalityMax) {
			string correspondingCategory = "";
			string similarSentence = "";
			double similarity = 0; 

			return this.SentenceToSemantic (sentence, clusterThreshold, penalityMax, out correspondingCategory, out similarSentence, out similarity);
		}

		/// <summary>
		/// Sentences to semantic.
		/// La variable penalityMax est utilisée dans le phraseEngineV2 pour déterminer la valeur 
		/// maximale de pénalité à donner aux chemins les plus longs. Cette valeur s'ajuste automatiquement 
		/// à la taille du plus grand chemin. Elle doit être comprise en 0 et 1. Je n'ai pas créé d'exception 
		/// pour gérer ce cas. 
		/// 
		/// </summary>
		/// <returns>The to semantic.</returns>
		/// <param name="sentence">Sentence.</param>
		public string SentenceToSemantic(string sentence, double clusterThreshold, double penalityMax, out string correspondingCategory, out string similarSentence, out double similarity) {
            Debug.Log(" plot twist");
            Debug.Log("WTF !!!! " + sentence);

			// Some cases to care about
			if (penalityMax > 1)
				penalityMax = 1;
			if (clusterThreshold > 1)
				clusterThreshold = 1;
			
			// If our problem need V2 or is generic and only need V1. 
			bool isGeneric = true;

			// Utiliser la V1 pour savoir dans quelle phrase on se situe. 
			//foreach(KeyValuePair<string, Dictionary<string, string>> categoriesRep in this.representations)
			//this.PhraseEngine.GetSimPhrases(sentence, 1);

			similarSentence = "";
			correspondingCategory = "";
			similarity = 0;

            Debug.Log("WTF !!!! " + sentence);

			string sentenceProcessed = "";
			if (this.processSentence != null)
				sentenceProcessed = this.processSentence(sentence, this.StopwordProcessor);
			else
				sentenceProcessed = (string)sentence.Clone();
			
			this.clusterSentence(sentenceProcessed, out correspondingCategory, out similarSentence, out similarity);

			if (similarity < clusterThreshold)
				return "NotUnderstood()";

			//Debug.Log ("Sentence : " + sentenceProcessed + " => " + correspondingCategory + "(" + similarSentence + ", " + similarity + ")");

			isGeneric = correspondingCategory == this.GenericCategory;
			string finalActype = "";
			// If not generic, use V2 else, return corresponding semantic.
			if (!isGeneric) {
				finalActype = this.calculActype(sentenceProcessed, correspondingCategory, penalityMax);
			} else {
				finalActype = this.sentencesActypes[correspondingCategory][similarSentence];
			}
			Debug.Log ("Final Actype : " + finalActype);

			return finalActype;
		}


		/// <summary>
		/// Clusters the sentence.
		/// </summary>
		/// <param name="sentence">Sentence.</param>
		/// <param name="correspondingCategory">Corresponding category.</param>
		/// <param name="similarSentence">Similar sentence.</param>
		/// <param name="similarity">Similarity.</param>
		public void clusterSentence(string sentence, out string correspondingCategory, out string similarSentence, out double similarity) {

			double maxSimilarity = 0;
			correspondingCategory = null;
			similarSentence = null;
			
			// Il faut faire ça pour toutes les catégories.
			foreach (KeyValuePair<string, Dictionary<string, string>> actypesPerCategories in this.sentencesActypes) {
				string category = actypesPerCategories.Key;
				this.PhraseEngine.SentencesActypes = actypesPerCategories.Value;
				ArrayList phrasesRef = this.PhraseEngine.GetSimPhrases(sentence, 1);

                foreach (System.Collections.DictionaryEntry phrase in phrasesRef) {
					if ((double)phrase.Value > maxSimilarity) {
						correspondingCategory = category;
						similarSentence = (string)phrase.Key;
						maxSimilarity = (double)phrase.Value;
					}
				}
			}

			similarity = maxSimilarity;
		}

		/// <summary>
		/// Calculs the actype.
		/// </summary>
		/// <param name="sentence">Sentence.</param>
		public string calculActype(string sentence, string category, double penalityMax = 0.1) {
			this.PhraseEngineV2.ChunkActype = this.chunkActype[category];
			return this.PhraseEngineV2.GetBestActype(sentence, penalityMax);
		}
	}
}

