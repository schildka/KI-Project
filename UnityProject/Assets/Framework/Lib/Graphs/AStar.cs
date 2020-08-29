using System;
using System.Collections.Generic;

namespace Graphs
{
	public static class AStar
	{
		public static bool Search<T>(Node<T> startNode, Func<Node<T>, double> heuristic, Func<Node<T>, bool> goalTest, out List<Node<T>> path, out double cost)
		{
			path = new List<Node<T>>();
			cost = float.PositiveInfinity;

			List<NodeWrapper<T>> openList = new List<NodeWrapper<T>>();
			HashSet<Node<T>> closedSet = new HashSet<Node<T>>();

			openList.Add(
				new NodeWrapper<T>
				{
					node = startNode
				}
			);

			// Working through fringe
			while (openList.Count > 0)
			{
				// openList is sorted according to F
				var current = openList[0];

				// At goal?
				if (goalTest(current.node))
				{
					CalculatePathAndCost(current, out path, out cost);
					return true;
				}

				// Remove node and move to closed set
				openList.RemoveAt(0);
				closedSet.Add(current.node);

				// Expand successors
				foreach (var edge in current.node.Edges)
				{
					var succNode = edge.Key;
					var edgeCost = edge.Value;

					// Node was already explored
					if (closedSet.Contains(succNode))
						continue;

					// Path costs so far
					double tentativeG = current.g + edgeCost;

					// Find wrapper in case node is already in openList
					var succWrapper = openList.Find((_w) => { return _w.node == succNode; });

					// Better path already exists
					if (succWrapper != null && tentativeG >= succWrapper.g)
						continue;

					// Node was not expanded before
					if (succWrapper == null)
					{
						succWrapper = new NodeWrapper<T>();
						succWrapper.node = succNode;
						openList.Add(succWrapper);
					}

					// Update node properties
					succWrapper.g = tentativeG;
					succWrapper.h = heuristic(succNode);
					succWrapper.predecessor = current;
				}

				// Sort list (QuickSort --> not the best data structure!)
				openList.Sort();
			}

			return false;
		}

		static void CalculatePathAndCost<T>(NodeWrapper<T> node, out List<Node<T>> path, out double cost)
		{
			path = new List<Node<T>>();
			cost = 0;

			var current = node;
			while (current.predecessor != null)
			{
				path.Add(current.node);
				cost += current.predecessor.node.Edges[current.node];
				current = current.predecessor;
			}

			path.Reverse();
		}


		class NodeWrapper<DataType> : IComparable
		{
			public Node<DataType> node;
			public NodeWrapper<DataType> predecessor;

			public double h;
			public double g;

			public double F
			{
				get
				{
					return h + g;
				}
			}

			public int CompareTo(object obj)
			{
				if (obj == null) return 1;

				var other = obj as NodeWrapper<DataType>;

				if (other != null)
					return F.CompareTo(other.F);

				throw new ArgumentException("Object is not a NodeWrapper");
			}
		}
	}
}
