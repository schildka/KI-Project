using System;
using System.Collections.Generic;

namespace Graphs
{
	public static class DepthLimitedSearch
	{

		public static bool Search<T>(Node<T> startNode,
									 int maxDepth, Func<Node<T>, bool> goalTest,
									 out List<Node<T>> path)
		{
			path = new List<Node<T>>();

			var parents = new Dictionary<Node<T>, Node<T>>();
			var visited = new HashSet<Node<T>>();
			var stack = new Stack<Node<T>>();
			stack.Push(startNode);

			int level = 0;

			while (stack.Count > 0)
			{
				var currentNode = stack.Pop();
				visited.Add(currentNode);

				// Cutoff
				if (level >= maxDepth) continue;

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

				foreach (var child in currentNode.Neighbors)
				{
					if (!visited.Contains(child))
					{
						stack.Push(child);
						parents[child] = currentNode;
					}
				}

				level++;
			}

			return false;
		}
	}
}
