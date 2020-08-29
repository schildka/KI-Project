using System;

namespace TreeSearch
{

    /// <summary>
    /// 
    /// Deliverables
    /// 
    /// 1. An implementation of `class Node<DataType>` in `CodeProject/src/Node.cs` [1 Pts]
    /// 2. A test routine including a visitor that prints the tree structure in `CodeProject/src/TreeSearch.cs` [1 Pts]
    /// 3. Breadth-First Search using the node implementation in `CodeProject/src/BreadthFirstSearch.cs` [1 Pts]
    /// 4. Depth-First Search using the node implementation in `CodeProject/src/DepthFirstSearch.cs` [1 Pts]
    /// 5. Depth-Limited Search using the node implementation in `CodeProject/src/DepthLimitedSearch.cs` [1 Pts]
    /// 
    /// </summary>
    public class TreeSearch
    {

        static Node<string> GenerateTree()
        {
            Node<string> root = null;

            // Add children and branches here...

            return root;
        }

        public static void PrintNode(Node<string> node)
        {
            // Print the node and it's children
        }

        static void Main(string[] args)
        {
            var tree = GenerateTree();

            // Traversal.PrintNode needs to be implemented...
            tree.Traverse(PrintNode);

            // Implement a goal test...
            Func<Node<string>, bool> goalTest = (n) => { return false; };

            Node<string> result = null;

            Console.WriteLine("Testing BFS...");
            if (!BreadthFirstSearch.Search(tree, goalTest, out result))
                throw new Exception();

            Console.WriteLine("Testing DFS...");
            if (!DepthFirstSearch.Search(tree, goalTest, out result))
                throw new Exception();

            Console.WriteLine("Testing DLS...");
            int depthLimit = 3;
            if (!DepthLimitedSearch.Search(tree, depthLimit, goalTest, out result))
                throw new Exception();

            Console.WriteLine("Success.");
        }

    }

}
