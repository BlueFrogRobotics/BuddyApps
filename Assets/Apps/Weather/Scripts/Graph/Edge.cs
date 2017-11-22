using System;

namespace BuddyApp.Weather
{
	public class Edge
	{
		/// <summary>
		/// The counter.
		/// </summary>
		private static int counter = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Graph.Edge`1"/> class.
		/// </summary>
		public Edge ()
		{
			this.Id = Edge.counter;
			Edge.counter++;
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
		/// Gets or sets the info string.
		/// </summary>
		/// <value>The info string.</value>
		public string InfoString {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the memory string.
		/// </summary>
		/// <value>The memory string.</value>
		public string MemoryString {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets from.
		/// </summary>
		/// <value>From.</value>
		public Node From {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets to.
		/// </summary>
		/// <value>To.</value>
		public Node To {
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

