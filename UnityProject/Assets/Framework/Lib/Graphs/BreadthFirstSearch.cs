using System;
using System.Collections.Generic;

namespace Graphs
{
	public static class BreadthFirstSearch
	{

		public static bool Search<T>(Node<T> startNode,
									 Func<Node<T>, bool> goalTest,
									 out List<Node<T>> path)
		{
			path = new List<Node<T>>();

			var parents = new Dictionary<Node<T>, Node<T>>();
			var visited = new HashSet<Node<T>>();
			var queue = new Queue<Node<T>>();

			queue.Enqueue(startNode);
			while (queue.Count > 0)
			{
				var currentNode = queue.Dequeue();
				visited.Add(currentNode);

				if (goalTest(currentNode))
				{
					path.Add(currentNode);
					while (parents.ContainsKey(currentNode))
					{
						currentNode = parents[currentNode];
						path.Add(currentNode);
					}
					path.Reverse();
					return true;
				}

				foreach (var successor in currentNode.Neighbors)
				{
					if (!visited.Contains(successor))
					{
						queue.Enqueue(successor);
						parents[successor] = currentNode;
					}
				}
			}

			return false;
		}
	}
}
