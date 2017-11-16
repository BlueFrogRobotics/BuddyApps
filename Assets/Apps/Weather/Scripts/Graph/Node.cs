using System;
using System.Collections.Generic;

namespace BuddyApp.Weather
{
	public class Node
	{
		/// <summary>
		/// Initializes the <see cref="Graph.Node"/> class.
		/// </summary>
		private static int counter = 0;


		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyCSharp.Node`1"/> class.
		/// </summary>
		public Node ()
		{
			this.Id = counter;
			counter++;
			this.OuterEdges = new List<Edge> ();
			this.InnerEdges = new List<Edge> ();
		}

		/// <summary>
		/// Gets or sets the info.
		/// </summary>
		/// <value>The info.</value>
		public double Info {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the weight.
		/// </summary>
		/// <value>The weight.</value>
		public double Weight {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the outer edges.
		/// </summary>
		/// <value>The outer edges.</value>
		public List<Edge> OuterEdges {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the inner edges.
		/// </summary>
		/// <value>The inner edges.</value>
		public List<Edge> InnerEdges {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int Id {
			get;
			set;
		}
	}
}

