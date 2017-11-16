using System;
using System.Collections.Generic;
using UnityEngine;


namespace BuddyApp.Weather
{
	/// <summary>
	/// Actype. Attention, cette classe n'a pas de lien avec ActypeRepresentation
	/// </summary>
	public class Act
	{
		private string name = ""; 
		private Dictionary<string, string> attr;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public String Actype {
			set{
				this.name = value;
			}
			get {
				return this.name;
			}
		}

		/// <summary>
		/// Gets or sets the attr.
		/// </summary>
		/// <value>The attr.</value>
		public Dictionary<string, string> Slots {
			set{
				this.attr = value;
			}
			get {
				return this.attr;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BuddyApp.WordEmbedding.NLP.Discussion.Actype"/> class.
		/// Attention : Cette classe n'a pas de lien avec ActypeRepresentation
		/// </summary>
		public Act ()
		{
			this.attr = new Dictionary<string, string> ();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="BuddyApp.WordEmbedding.NLP.Discussion.Actype"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BuddyApp.WordEmbedding.NLP.Discussion.Actype"/>.</returns>
		public string ToString() {
			string act = name;
			if(this.Actype != "") 
				act += "(";
			foreach (KeyValuePair<string, string> entry in this.attr) {
				act += entry.Key + "=" + entry.Value;
			}
			if(this.name != "")
				act += ")";
			return act;
		}

		/// <summary>
		/// Sets the actype.
		/// </summary>
		/// <returns>The actype.</returns>
		/// <param name="actype">Actype.</param>
		public Act setAct(string[] act) {
			for (int i = 0; i < act.Length; i++) {
				if (act [i].Contains ("=")) {
					if (act [i] != "") {
						string[] attrSplit = act [i].Split ('=');
						this.attr.Add (attrSplit [0], attrSplit [1]);
					}
				} else if (act [i] != " ") {
					if (act [i] != "") {
						this.name = act [0];
					}
				}
			}
			return this;
		}

		/// <summary>
		/// Determines whether this instance is attribute only.
		/// </summary>
		/// <returns><c>true</c> if this instance is attribute only; otherwise, <c>false</c>.</returns>
		public Boolean IsAttributeOnly() {
			return this.name == ""; 
		}
	}
}

