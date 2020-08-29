using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Graphs
{
	public static class AStar
	{
        public static bool Search<T>(Node<T> startNode, Func<Node<T>, double> heuristic, Func<Node<T>, bool> goalTest, out List<Node<T>> path, out double cost)
        {
            path = new List<Node<T>>();
            cost = float.PositiveInfinity;

            var nodeList = new List<WrapperNode<T>>();
            var checkList = new List<Node<T>>();
            var startWrapperNode = new WrapperNode<T> { NodeAStar = startNode, Predecessor = null };
            nodeList.Add(startWrapperNode);
            //UnityEngine.Debug.Log("HHIHIHIHIHIHIIHI");


            while (nodeList.Count != 0)
            {
                WrapperNode<T> currentNode = nodeList[0];
                checkList.Add(currentNode.NodeAStar);
                nodeList.RemoveAt(0);

                if (goalTest(currentNode.NodeAStar))
                {

                    WrapperNode<T> bla = currentNode;
                    path.Add(currentNode.NodeAStar);
                    cost = 0;
                    while (bla.Predecessor != null)
                    {
                        bla = bla.Predecessor;
                        cost += bla.g;
                        path.Add(bla.NodeAStar);
                    }
                    path.Reverse();
                    path.RemoveAt(0);
                    return true;
                }

                foreach (var childNodes in currentNode.NodeAStar.Edges)
                {
                    if (childNodes.Key == currentNode.NodeAStar || checkList.Contains(childNodes.Key))
                    {
                        continue;
                    }
                    var EdgeWrapperNode = new WrapperNode<T> { NodeAStar = childNodes.Key, Predecessor = currentNode };
                    //var distFromNow = heuristic(EdgeWrapperNode.NodeAStar);
                    EdgeWrapperNode.g = childNodes.Value + currentNode.g;
                    EdgeWrapperNode.h = heuristic(childNodes.Key);
                    //UnityEngine.Debug.Log(" G way before" + EdgeWrapperNode.g + "    H way to go" + EdgeWrapperNode.h);
                    nodeList.Add(EdgeWrapperNode);
                }
                nodeList.Sort();
            }
            return false;
        }

        public class WrapperNode<DataType> : IComparable
        {
            public WrapperNode<DataType> Predecessor;
            public Node<DataType> NodeAStar;

            public double g = 0;
            public double h = 0;

            public double getHeuristic
            {
                get { return g + h; }
            }

            public int CompareTo(object obj)
            {
                if (obj == null) return 1;

                var other = obj as WrapperNode<DataType>;

                if (other != null)
                    return getHeuristic.CompareTo(other.getHeuristic);

                throw new ArgumentException("Object is not a WrapperNode");
            }

        }
    }

}



