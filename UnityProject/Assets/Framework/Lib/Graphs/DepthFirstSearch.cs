using System;
using System.Collections.Generic;

namespace Graphs
{
	public static class DepthFirstSearch
	{

		public static bool Search<T>(Node<T> startNode,
									 Func<Node<T>, bool> goalTest,
									 out List<Node<T>> path)
		{
			path = new List<Node<T>>();

			var parents = new Dictionary<Node<T>, Node<T>>();
			var visited = new HashSet<Node<T>>();
			var stack = new Stack<Node<T>>();

			stack.Push(startNode);
			while (stack.Count > 0)
			{
				var currentNode = stack.Pop();
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
						stack.Push(successor);
						parents[successor] = currentNode;
					}
				}
			}

			return false;
		}
	}
}
