using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace BuddyApp.Weather
{
	public class PhraseEngineV2
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.PhraseEngineV2"/> class.
		/// </summary>
		public PhraseEngineV2 ()
		{
			this.ChunkActype = new Dictionary<string, Act> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.PhraseEngineV2"/> class.
		/// </summary>
		/// <param name="vocabulary">Vocabulary.</param>
		public PhraseEngineV2(Vocabulary vocabulary) {
			this.Vocabulary = vocabulary;
			this.ChunkActype = new Dictionary<string, Act> ();
			this.ChunkRepresentation = new Dictionary<string, Representation> ();
		}

		/// <summary>
		/// Gets or sets the vocabulary.
		/// </summary>
		/// <value>The vocabulary.</value>
		public Vocabulary Vocabulary {
			set;
			get;
		}

		/// <summary>
		/// Gets or sets the stopword processor.
		/// </summary>
		/// <value>The stopword processor.</value>
		public StopwordProcessor StopwordProcessor {
			set;
			get;
		}
	
		/// <summary>
		/// Gets or sets the chunk actype.
		/// </summary>
		/// <value>The chunk actypes.</value>
		public Dictionary<string, Act> ChunkActype {
			set;
			get;
		}
	
		/// <summary>
		/// Gets or sets the chunk representation.
		/// </summary>
		/// <value>The chunk representation.</value>
		public Dictionary<string, Representation> ChunkRepresentation {
			set;
			get;
		}

		/// <summary>
		/// Gets the actype.
		/// </summary>
		/// <returns>The actype.</returns>
		/// <param name="sentence">Sentence.</param>
		public Graph.Graph constructGraph(string sentence, double maxPenality = 0.0) {
			Graph.Graph graph = new Graph.Graph ();
			string phrase = this.ProcessPhrase (sentence);
			string[] phraseSplited = phrase.Split(' ');
			Node[] nodes = new Node[phraseSplited.Length+1];

			// Init nodes 
			for (int i = 0; i < phraseSplited.Length+1; i++) {
				nodes[i] = graph.AddNode (new Node ());
			}

			int nbMaxEdgePerPath = phraseSplited.Length;
			// Construct the graph 
			for (int i = 0; i < phraseSplited.Length; i++) {
				string chunk = phraseSplited [i];
				for (int j = i; j < phraseSplited.Length; j++) {
					KeyValuePair<string, double> value = MostSimilarToRef (chunk);
					//Debug.Log ("Sim : " + chunk + " | " + value.Key + " => " + value.Value);
					double penality = 0.0;
					Edge edge = graph.AddEdge (nodes [i], nodes [j + 1]);
					if (nbMaxEdgePerPath > 1) {
						penality = maxPenality / (nbMaxEdgePerPath - 1) * (nbMaxEdgePerPath - j + i - 1);
						edge.Info = value.Value - penality;
						edge.Weight = (value.Value - penality) * chunk.Split (' ').Length / nbMaxEdgePerPath; //(nodes.Length - (nodes [j + 1].Id - nodes [i].Id)); // With normalization
					} else {
						edge.Info = value.Value;
						edge.Weight = edge.Info;
					}
					edge.InfoString = value.Key;
					edge.MemoryString = chunk;
					if(j < phraseSplited.Length - 1)
						chunk += " " + phraseSplited [j + 1];
				}
			}
			return graph;
		}


		/// <summary>
		/// Mosts the similar to reference.
		/// </summary>
		/// <returns>The similar to reference.</returns>
		/// <param name="chunk">Chunk.</param>
		public KeyValuePair<string, double> MostSimilarToRef(string chunk) {
			// On calcule la table des scores
			Representation rep = this.Vocabulary.GetSummRepresentationOrNullForPhrase (this.ProcessPhrase(chunk), true);
			if (rep == null) {
				throw new PhraseHasNoRepresentation (chunk, this.ProcessPhrase(chunk));
			}
			double closestDistance = -1;
			string closestChunk = "";

			foreach (KeyValuePair<string, Act> chunkAct in this.ChunkActype) {
				Representation chunkRep = this.ChunkRepresentation [chunkAct.Key];
				DistanceTo distance = rep.GetCosineDistanceTo (chunkRep);
				if (distance.DistanceValue > closestDistance) {
					closestChunk = chunkAct.Key;	
					closestDistance = distance.DistanceValue;
				}
			}

			//Debug.Log ("CLOSEST CHUNK : " + closestChunk);

			return new KeyValuePair<string, double>(closestChunk, closestDistance);
		}

		/// <summary>
		/// Gets the best actype.
		/// </summary>
		/// <returns>The best actype.</returns>
		/// <param name="phrase">Phrase.</param>
		public string GetBestActype(string sentence, double penalityMax = 0.1) {
			string processedSentence = this.ProcessPhrase (sentence);
			
			Graph.Graph graph = this.constructGraph (processedSentence, penalityMax);

			foreach (Edge edge in graph.Edges) {
				//Debug.Log (edge.MemoryString + " => " + edge.InfoString + " : " + edge.Weight + " et " + edge.Info);
			}

			List<Edge> edges = graph.GetLongestPath (
				graph.Nodes.ToArray()[0], 
				graph.Nodes.ToArray()[graph.Nodes.ToArray().Length-1]
			);

			String edgeDescriptor = "Description de l'arc : ";
			foreach (Edge edge in edges) {
				edgeDescriptor += edge.MemoryString + " : " + edge.InfoString.Split (' ').Length.ToString() + " & " + edge.Info + " | ";
			}

			//Debug.Log (edgeDescriptor);

			// Est la première fonction ? 
			Boolean isFirstFunc = true;
			// On avait finit sur un attribut à l'étape précédente ? 
			Boolean finishedOnAttr = false;
			// Full actype qu'on retourne à la fin.
			string fullActype = "";
			foreach (Edge edge in edges) {
				Act act = this.ChunkActype [edge.InfoString];
				//Debug.Log ("Actype : " + actype.ToString());

				// Si c'est un attribut 
				if (act.IsAttributeOnly ()) {
					foreach (KeyValuePair<string, string> attr in act.Slots) {
						if (finishedOnAttr)
							fullActype += ",";
						fullActype += attr.Key + "=" + attr.Value;
						finishedOnAttr = true;
					}
				} 
				// Si c'est une fonction
				else {
					if (!isFirstFunc)
						fullActype += ");";
					fullActype += act.Actype + "(";
					finishedOnAttr = false;
					isFirstFunc = false;
					foreach (KeyValuePair<string, string> attr in act.Slots) {
						if (finishedOnAttr)
							fullActype += ",";
						fullActype += attr.Key + "=" + attr.Value;
						finishedOnAttr = true;
					}
				}
			}
			fullActype += ")";
			return fullActype;
		}
			
		/// <summary>
		/// Processes the phrase.
		/// Hypothese that only one hyphen ou one single quote 
		/// which could make word incomprehensible.
		/// </summary>
		/// <returns>The phrase.</returns>
		/// <param name="phrase">Phrase.</param>
		public string ProcessPhrase(string phrase) {
			string newPhrase = (string)phrase.Clone ();
			newPhrase = newPhrase.Replace (" ?", "");
			newPhrase = newPhrase.Replace ("?", "");
			string [] splitedSentence = newPhrase.Split (' ');
			for (int i = 0; i < splitedSentence.Length; i++) {
				if (!this.Vocabulary.ContainsWord (splitedSentence [i])) {
					string [] splitHyphen = splitedSentence [i].Split ('-');
					string [] splitSingleQuote = splitedSentence [i].Split ('\'');
					string [] splitSingleQuoteSpecial = splitedSentence [i].Split ('’');
					//Debug.Log ("splitHyphen : " + splitHyphen.Length + " & splitSingleQuote : " + splitSingleQuote.Length);
					if (splitHyphen.Length > 1) {
						splitedSentence [i] = String.Join (" ", splitHyphen);
					} else if(splitSingleQuote.Length > 1) {
						splitedSentence [i] = String.Join (" ", splitSingleQuote);
					} else if(splitSingleQuoteSpecial.Length > 1) {
						splitedSentence [i] = String.Join (" ", splitSingleQuoteSpecial);
					}
				}
			}
			newPhrase = String.Join (" ", splitedSentence);
			if (this.StopwordProcessor != null)
				newPhrase = StopwordProcessor.Process (newPhrase);
			return newPhrase;
		}

		/// <summary>
		/// Adds the chunk reference.
		/// </summary>
		/// <param name="chunk">Chunk.</param>
		/// <param name="actype">Actype</param>
		public void AddChunkRef(string chunk, Act actype) {
			Representation rep = this.Vocabulary.GetSummRepresentationOrNullForPhrase (this.ProcessPhrase(chunk));
			if (rep == null) {
				Debug.Log ("Phrase sans representation : " + this.ProcessPhrase (chunk));
				throw new PhraseHasNoRepresentation (chunk, this.ProcessPhrase(chunk));
			}

			if(chunk == "" || chunk == " ") { 
				// On a certainement eu un mauvais cas de stopword
				Debug.Log("Empty chunk : " + actype.ToString());
			}
			else if (this.ChunkActype.ContainsKey (chunk)) {
				Debug.Log ("chunk already exists :" + chunk + " " + actype.ToString() + " | " + this.ChunkActype [chunk].ToString ()); 
			} else {
				this.ChunkActype.Add (chunk, actype);
				this.ChunkRepresentation.Add (chunk, rep);
			}
		}
	}
}

