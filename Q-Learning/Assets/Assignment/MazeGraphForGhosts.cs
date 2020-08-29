using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs;
using System;

[RequireComponent(typeof(GlobalKnowledgeSensor))]
public class MazeGraphForGhosts : MazeGraph<MazeGraphForGhosts.TileData>
{

	public class TileData : PositionData
	{
       public bool canHavePickup;
	}

	GlobalKnowledgeSensor knowledge;

    Dictionary<Vector2, Node<TileData>> nodeDict = new Dictionary<Vector2, Node<TileData>>();

	void Start()
	{
		knowledge = GetComponent<GlobalKnowledgeSensor>();

        // TODO: Create a graph representation of the maze
        // TODO: One node at each junction
        // TODO: Edges when junctions are connected



        GenerateGraph();

	}

    void GenerateGraph()
    {  
        graph = new Node<TileData>( new TileData());
        graph.data.position = new Vector2(1, 1);

        RecursivelyGenerateGraph(graph);
    }

    void RecursivelyGenerateGraph(Node<TileData> startNode)
    {
        // How do we create the graph?

        Vector2 startPosition = startNode.data.position;

        // - walk throuhg the maze, check for possible moves (>=2)

        foreach (var d in maze.PossibleMoves(startPosition))
        {
            // walk in direction d until   
            // - at junction
            // - in corner
            var neighbourPosition = startPosition + d.ToVector2();
            while (maze.IsTileWalkable(neighbourPosition + d.ToVector2()) && maze.PossibleMoves(neighbourPosition).Count <= 2)
            {
                neighbourPosition += d.ToVector2();
            }

            if (!nodeDict.ContainsKey(neighbourPosition))
            {
                var node = new Node<TileData>(new TileData());
                node.data.position = neighbourPosition;

                node.data.canHavePickup = null != knowledge.TryGetPickupItem(PickupType.PILL, neighbourPosition);

                // We need to know where we've been.
                nodeDict[neighbourPosition] = node;

                RecursivelyGenerateGraph(nodeDict[neighbourPosition]);
            }

            double weight = (startPosition - neighbourPosition).magnitude;
            startNode.SetEdge(nodeDict[neighbourPosition], weight);

        }
         
    }

    public Node<TileData> GetClosestNode(Vector2 position)
	{
        // Look in all possible directions for the closest node in dictNode

        int MAX_DISTANCE = 20;
        for(int i = 0; i < MAX_DISTANCE; i++)
        {
            foreach(var d in maze.PossibleMoves(position))
            {
                var tile = d.ToVector2() * i + position;
                if (nodeDict.ContainsKey(tile))
                    return nodeDict[tile];
            }
        }

		return null;
	}

	public Node<TileData> GetClosestNode(Vector2 position, Direction d)
	{
		// TODO

		return null;
	}

	public void ModifyEdgeWeight(Vector2 tile, double weight)
	{
		// TODO
	}

	public void RestoreEdgeWeight(Vector2 tile)
	{
		// TODO
	}

}
