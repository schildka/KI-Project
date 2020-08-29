using System;

namespace TreeSearch
{
    public static class DepthFirstSearch
    {
        public static bool Search<T>(Node<T> startNode, Func<Node<T>, bool> goalTest, out Node<T> result)
        {
            result = null;
            return false;
        }
    }

}
