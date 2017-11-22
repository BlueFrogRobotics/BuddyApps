using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BuddyApp.Weather
{
	/// <summary>
	/// Phrase engine.
	/// Gère les phrases de référence pour comparer avec la phrase donnée.
	/// Elle ne gère pas l'allocation des données notamment concernant le Word2Vec.
	/// </summary>
	public class PhraseEngine
	{
		/// <summary>
		/// The stopword processor.
		/// </summary>
		private StopwordProcessor stopwordProcessor = null;

		/// <summary>
		/// The sentences representation.
		/// </summary>
		private Dictionary<string, Representation> sentencesRepresentation = null;

		/// <summary>
		/// Representation in actypes
		/// </summary>
		private Dictionary<string, string> sentencesActypes = null;

		/// <summary>
		/// The vocabulary from word2vec.
		/// </summary>
		private Vocabulary vocabulary = null;

		/// <summary>
		/// Gets or sets the sentences representation.
		/// </summary>
		/// <value>The sentences representation.</value>
		public Dictionary<string, Representation> SentencesRepresentation {
			get {
				return this.sentencesRepresentation;
			}
			set {
				this.sentencesRepresentation = value;
			}
		}

		/// <summary>
		/// Gets or sets the sentences actypes.
		/// </summary>
		/// <value>The sentences actypes.</value>
		public Dictionary<string, string> SentencesActypes {
			get {
				return this.sentencesActypes;
			}
			set {
				this.sentencesActypes = value;
			}
		}



		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.PhraseEngine"/> class.
		/// Cela charge le modèle.
		/// </summary>
		/// <param name="vocabulary">Vocabulary.</param>
		public PhraseEngine (Vocabulary vocabulary)
		{
			this.sentencesRepresentation = new Dictionary<string, Representation> ();
			this.sentencesActypes = new Dictionary<string, string> ();
			this.vocabulary = vocabulary;
		}

		/// <summary>
		/// Gets or sets the stopword processor.
		/// </summary>
		/// <value>The stopword processor.</value>
		public StopwordProcessor StopwordProcessor {
			set {
				this.stopwordProcessor = value;
			}
			get {
				return this.stopwordProcessor;
			}
		}

		/// <summary>
		/// Sets the vocabulary.
		/// Permet de changer de modèle si nécessaire.
		/// </summary>
		/// <param name="vocabulary">Vocabulary.</param>
		public void setVocabulary(Vocabulary vocabulary) {
			this.vocabulary = vocabulary;
		}
			

		/// <summary>
		/// Ajoute la phrase de référence au dictionnaire. 
		/// Il faut définir son actype aussi. (deuxième dictionnaire)
		/// </summary>
		/// <param name="phrase">Phrase.</param>
		public void AddPhraseRef(string phrase, string actype) {
			if (!this.sentencesRepresentation.ContainsKey(phrase)) {
				Representation rep = this.vocabulary.GetSummRepresentationOrNullForPhrase (this.ProcessPhrase(phrase));
				this.sentencesRepresentation.Add (phrase, rep);
				this.sentencesActypes.Add (phrase, actype);
			}
		}

        /// <summary>
        /// Permet de récupérer les phrases de référence les plus similaires à la 
        /// phrase donnée en entrée. 
        /// On peut préciser le nombre de phrases les plus similaires 
        /// que l'on souhaite avoir en retour. 
        /// </summary>
        /// <param name="phrase">Phrase.</param>
        /// <param name="nb">Nb.</param>
        /// <returns>Phrases les plus similaires</returns>

        public ArrayList GetSimPhrases(string phrase, int nb = 5) {
			// On calcule la table des scores
			Hashtable scores = new Hashtable();
			Representation rep = this.vocabulary.GetSummRepresentationOrNullForPhrase(this.ProcessPhrase(phrase), true);
			if (rep == null) {
				throw new PhraseHasNoRepresentation(phrase, this.ProcessPhrase(phrase));
			}
			int nbMax = 0;
			foreach (KeyValuePair<string, string> sentence in this.sentencesActypes) {
				if (this.sentencesRepresentation.ContainsKey(sentence.Key)) {
                    if (this.sentencesRepresentation[sentence.Key] != null)
                    {
                        DistanceTo distance = rep.GetCosineDistanceTo(this.sentencesRepresentation[sentence.Key]);
                        scores.Add(sentence.Key, distance.DistanceValue);
					    nbMax++;
                    }
				}
			}

            // On trie cette table et on récupère le nombre de phrases souhaitées
            IComparer scoreComparer = new IPhraseScoreComparer();
			ArrayList al = new ArrayList();
			al.AddRange(scores);
			al.Sort(scoreComparer);
			return al.GetRange(0, Math.Min(nb, nbMax));
		}

		/// <summary>
		/// Gets the actype of a phrase.
		/// </summary>
		/// <returns>The actype of a phrase.</returns>
		/// <param name="phrase">Phrase.</param>
		public string GetActypeOfAPhrase(string phrase) {
			if (this.sentencesActypes.ContainsKey (phrase))
				return this.sentencesActypes [phrase];
			else
				return null;
		}

		/// <summary>
		/// Sets the stopword processor.
		/// </summary>
		/// <param name="stopwordProcessor">Stopword processor.</param>
		public void SetStopwordProcessor(StopwordProcessor stopwordProcessor) {
			this.stopwordProcessor = stopwordProcessor;
		}

		/// <summary>
		/// Processes the phrase.
		/// Use StopwordProcessor if defined. 
		/// Next step : Use Lemmatiser if defined
		/// Next step : Use other processors if defined
		/// </summary>
		/// <returns>The phrase processed.</returns>
		/// <param name="phrase">Phrase.</param>
		public string ProcessPhrase(string phrase) {
			string newPhrase = (string)phrase.Clone ();
			newPhrase = newPhrase.Replace (" ?", "");
			newPhrase = newPhrase.Replace ("?", "");
			if (this.stopwordProcessor != null)
				newPhrase = stopwordProcessor.Process (newPhrase);
			return newPhrase;
		}
	}
}

