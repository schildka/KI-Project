using System;

namespace TreeSearch
{

    public static class DepthLimitedSearch
    {
        public static bool Search<T>(Node<T> startNode, int maxDepth, Func<Node<T>, bool> goalTest, out Node<T> result)
        {
            result = null;
            return false;
        }

    }
}
