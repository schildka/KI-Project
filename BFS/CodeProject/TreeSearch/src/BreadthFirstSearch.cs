using System;
using System.CodeDom;
using System.Collections.Generic;

namespace TreeSearch
{
    public static class BreadthFirstSearch
    {
        static Node<int> target;

        static public void Main(String[] args)
        {
            try
            {
                Node<int> root = new Node<int>(0);
                root.Nachfolger.Add(new Node<int>(1));
                root.Nachfolger.Add(new Node<int>(2));
                root.Nachfolger.Add(new Node<int>(3));
                root.Nachfolger.Add(new Node<int>(4));
                root.Nachfolger[0].Nachfolger.Add(new Node<int>(5));
                root.Nachfolger[0].Nachfolger.Add(new Node<int>(6));
                root.Nachfolger[0].Nachfolger.Add(new Node<int>(7));
                root.Nachfolger[2].Nachfolger.Add(new Node<int>(8));
                root.Nachfolger[2].Nachfolger.Add(new Node<int>(9));
                root.Nachfolger[2].Nachfolger.Add(new Node<int>(10));
                root.Nachfolger[2].Nachfolger[1].Nachfolger.Add( new Node<int>(11));
                root.Nachfolger[2].Nachfolger[1].Nachfolger.Add(target = new Node<int>(12));
                

                Action<TreeSearch.Node<int>> act = delegate(TreeSearch.Node<int> k) { };

                Func<Node<int>, bool> goalTest = nodecheck;
                root.Traverse(act);
                Console.WriteLine("Main Method");
                Node<int> res = null;
                Console.Out.WriteLine(Search(root, goalTest, result: out res));
                Console.Out.WriteLine(res.data);
                
            }
            catch (NullReferenceException e)
            {
            }
            
        }

        public static void printNumber()
        {
        }

        static bool nodecheck(Node<int> myTargetNode)
        {
            if (myTargetNode == target) return true;
            return false;
        }


        public static bool Search<T>(Node<T> startNode, Func<Node<T>, bool> goalTest, out Node<T> result)
        {
            startNode.initBFS();
            startNode.color = "gray";
            startNode.Value = 0;
            startNode.Vorgaenger = null;
            Queue<Node<T>> Q = new Queue<Node<T>>();
            Q.Enqueue(startNode);
            while (Q.Count != 0)
            {
                Node<T> currentNode = Q.Dequeue();

                if (goalTest.Invoke(currentNode))
                {
                    result = currentNode;
                    return true;
                }

                foreach (Node<T> nachfolger in currentNode.Nachfolger)
                {
                    if (nachfolger.color.Equals("white"))
                    {
                        nachfolger.color = "gray";
                        nachfolger.Value = currentNode.Value + 1;
                        nachfolger.Vorgaenger = currentNode;
                        Q.Enqueue(nachfolger);
                    }
                }

                currentNode.color = "black";
            }

            result = null;
            return false;
        }
    }
}