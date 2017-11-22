using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Weather.Graph
{
	public class Graph
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Graph.Graph"/> class.
		/// </summary>
		public Graph ()
		{
			this.Nodes = new List<Node> ();
			this.Edges = new List<Edge> ();
		}

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		public List<Node> Nodes {
			set;
			get;
		}

		/// <summary>
		/// Gets or sets the edges.
		/// </summary>
		/// <value>The edges.</value>
		public List<Edge> Edges {
			set;
			get;
		}

		/// <summary>
		/// Adds the node.
		/// </summary>
		/// <returns>The node.</returns>
		/// <param name="node">Node.</param>
		public Node AddNode(Node node) {
			this.Nodes.Add (node);
			return node;
		}

		/// <summary>
		/// Adds the edge.
		/// </summary>
		/// <returns>The edge.</returns>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		public Edge AddEdge(Node from, Node to) {
			Edge edge = new Edge ();
			edge.From = from;
			edge.To = to;
			from.OuterEdges.Add (edge);
			to.InnerEdges.Add (edge);
			this.Edges.Add (edge);
			return edge;
		}

		/// <summary>
		/// Gets the shortest way.
		/// Use dijkstra's algorithm.
		/// </summary>w
		/// <returns>The shortest way.</returns>
		/// <param name="begin">Begin.</param>
		public List<Edge> GetShortestPath(Node begin, Node end) {
			List<Node> queue = new List<Node> ();
			Dictionary<Node, double> dist = new Dictionary<Node, double> ();
			Dictionary<Node, Edge> prev = new Dictionary<Node, Edge> ();

			foreach (Node node in this.Nodes) {
				prev.Add (node, null);
				dist.Add (node, 10000000);
				queue.Add (node);
			}

			dist [begin] = 0;
			Node pointer = null;
			while(queue.Count != 0) {
				pointer = queue [0];
				// Get closest distance
				foreach (Node node in queue) {
					if(queue.Exists(x => x.Id == node.Id)) {
						if (dist [node] < dist [pointer])
							pointer = node;
					}
				} 
				queue.Remove (pointer);
		
				// Weight each node that is outer the pointer node. 
				List<Edge> edges = pointer.OuterEdges;
				foreach (Edge edge in edges) {
					double distance = dist [pointer] + edge.Weight;
					if (distance < dist [edge.To]) {
						dist [edge.To] = distance;
						prev [edge.To] = edge;
					}
				}
			}

			// Get result
			List<Edge> resultEdges = new List<Edge>();
			pointer = end;
			while(pointer != begin) {
				resultEdges.Add (prev [pointer]);
				pointer = prev [pointer].From;
			}

			resultEdges.Reverse ();
			return resultEdges; 
		}


		/// <summary>
		/// Gets the shortest way.
		/// Use dijkstra's algorithm.
		/// </summary>w
		/// <returns>The shortest way.</returns>
		/// <param name="begin">Begin.</param>
		public List<Edge> GetLongestPath(Node begin, Node end) {
			List<Node> queue = new List<Node> ();
			Dictionary<Node, double> dist = new Dictionary<Node, double> ();
			Dictionary<Node, Edge> prev = new Dictionary<Node, Edge> ();

			foreach (Node node in this.Nodes) {
				prev.Add (node, null);
				dist.Add (node, -100000);
				queue.Add (node);
			}

			dist [begin] = 10000000;
			Node pointer = null;
			while(queue.Count != 0) {
				pointer = queue [0];
				// Get closest distant node
				foreach (Node node in queue) {
					if(queue.Exists(x => x.Id == node.Id)) {
						if (dist [node] > dist [pointer])
							pointer = node;
					}
				} 
				queue.Remove (pointer);

				// Weight each node that is outer the pointer node. 
				List<Edge> edges = pointer.OuterEdges;
				foreach (Edge edge in edges) {
					double distance = dist [pointer] + edge.Weight;
					if (distance > dist [edge.To]) {
						//Debug.Log ("Est meilleur : " + edge.InfoString + " | " + edge.Info);
						dist [edge.To] = distance;
						prev [edge.To] = edge;
					}
				}
			}

			// Get result
			List<Edge> resultEdges = new List<Edge>();
			pointer = end;
			while(pointer != begin) {
				resultEdges.Add (prev [pointer]);
				pointer = prev [pointer].From;
			}

			resultEdges.Reverse ();
			return resultEdges; 
		}
	}
}

