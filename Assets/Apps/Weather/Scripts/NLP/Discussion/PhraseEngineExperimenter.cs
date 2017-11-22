using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace BuddyApp.Weather
{

	/// <summary>
	/// Phrase engine experimenter.
	/// </summary>
	public class PhraseEngineExperimenter
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.PhraseEngineExperimenter"/> class.
		/// </summary>
		public PhraseEngineExperimenter ()
		{
		}

		/// <summary>
		/// Experiment the specified phraseEngine, inputFIle and resultFile.
		/// </summary>
		/// <param name="phraseEngine">Phrase engine.</param>
		/// <param name="inputFIle">Input file csv with phrase;out wanted;.</param>
		/// <param name="resultFile">Result file csv with phrase;out wanted; out; simValue; out2; simValue.</param>
		public void Experiment(PhraseEngine phraseEngine, string inputFile, string resultFile) {
			CSVReader csvReader = new CSVReader ();
			csvReader.Read (inputFile);
			string[] line = null;
			var w = new StreamWriter (resultFile);
			int nbPhraseWithoutRepresentation = 0;
			int nbTotal = 0; 
			int nbGood = 0; 

			csvReader.Next ();
			w.WriteLine ("Phrase Attendue;Phrase entree;Sortie Attendue;Actype 1;Similarite 1;Actype 2;Similarite 2");
			while ((line = csvReader.Next ()) != null) {
				if (line.Length == 3) {
					string phraseAttendue = line [0]; 
					string phraseEntree = line [1]; 
					string sortieAttendue = line [2];
					try {
						ArrayList phrases = phraseEngine.GetSimPhrases (phraseEntree);
						System.Object[] phrasesEntries = phrases.ToArray ();
						string actypeOut1 = phraseEngine.GetActypeOfAPhrase ((string)((DictionaryEntry)phrasesEntries [0]).Key);
						double simValue1 = (double)((DictionaryEntry)phrasesEntries [0]).Value;
						string actypeOut2 = phraseEngine.GetActypeOfAPhrase ((string)((DictionaryEntry)phrasesEntries [1]).Key);
						double simValue2 = (double)((DictionaryEntry)phrasesEntries [1]).Value;
						string lineWrite = string.Format("{0};{1};{2};{3};{4};{5};{6}", phraseAttendue, phraseEntree, sortieAttendue, actypeOut1, simValue1, actypeOut2, simValue2);
						w.WriteLine(lineWrite);
						w.Flush();
						Debug.Log("Phrase Transformee : " + phraseEngine.ProcessPhrase(phraseEntree));
						if(actypeOut1.CompareTo(sortieAttendue) == 0) {	
							nbGood ++;
						}
						nbTotal ++;
					} catch(PhraseHasNoRepresentation exception) {
						nbPhraseWithoutRepresentation ++;
						Debug.Log ("No rep : " + exception.ProcessPhrase);
					}
				}
			}
			w.Close ();
			csvReader.Close ();
			Debug.Log ("Nombre de phrases sans representation : " + nbPhraseWithoutRepresentation);
			Debug.Log ("Nombre de phrases avec representation : " + nbTotal);
			Debug.Log ("Nombre de phrases bien representees : " + nbGood);
			Debug.Log ("Error Rate : " + (((float)nbTotal - (float)nbGood) / (float)nbTotal) * 100 + " %");
		}

		/// <summary>
		/// Tests the actypes are the same.
		/// </summary>
		/// <returns><c>true</c>, if actypes are the same was tested, <c>false</c> otherwise.</returns>
		/// <param name="actype1">Actype1.</param>
		/// <param name="actype2">Actype2.</param>
		public bool TestActypesAreTheSame(string actype1, string actype2) {
			if (actype1.CompareTo (actype2) == 0)
				return true;
			return false;
		}
	}
}

