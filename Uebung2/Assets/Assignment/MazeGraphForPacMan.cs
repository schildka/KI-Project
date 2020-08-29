using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Graphs;

[RequireComponent(typeof(GlobalKnowledgeSensor))]
public class MazeGraphForPacMan : MazeGraph<MazeGraphForPacMan.TileData>
{
	public class TileData : PositionData
	{
		// TODO
	    public Vector2 Coordinate;
	    public bool isBetterPowerUp;
	    public bool isPallet;
	    public bool isVisitedAlready = false;
	}

	GlobalKnowledgeSensor knowledge;
    Dictionary<Vector2, Node<TileData>> nodeDict = new Dictionary<Vector2, Node<TileData>>();

    void Start()
	{
		knowledge = GetComponent<GlobalKnowledgeSensor>();


	    GenerateGraph();
	}

    public Node<TileData> getNode(Vector2 tile)
    {
        //Debug.Log("Debug Count");
        //Debug.Log(nodeDict.Count);
        return nodeDict[tile];
    }
	public void ModifyEdgeWeight(Vector2 tile, double weight)
	{
		nodeDict[tile].SetEdge(nodeDict[tile], weight);
        
	}

	public void RestoreEdgeWeight(Vector2 tile)
	{
	    nodeDict[tile].SetEdge(nodeDict[tile],1);
        // TODO
    }

    public void GenerateGraph()
    {
        
        graph = new Node<TileData>(new TileData());
        
        graph.data.position = new Vector2(1, 1);
        graph.data.Coordinate = new Vector2(1,1);
        graph.data.isVisitedAlready = true;
        graph.data.isBetterPowerUp = false;
        GenerateRecursivelyGraph(graph);
    }


    public void GenerateRecursivelyGraph(Node<TileData> nodeMY)
    {
        Vector2 Newposition = nodeMY.data.Coordinate;
        var l = maze.PossibleMoves(Newposition);
        foreach (var d in l)
        {           
            var neighbourPos = Newposition + d.ToVector2();

            if (nodeDict.ContainsKey(neighbourPos))
            {
	            nodeMY.SetEdge(nodeDict[neighbourPos],1); 
	            nodeDict[neighbourPos].SetEdge(nodeMY,1);
	            continue;
            }
            
            var NewNode = new Node<TileData>(new TileData());
            NewNode.data.position = neighbourPos;
            NewNode.data.Coordinate = neighbourPos;
            var cherryList = new List<Vector2>();
            NewNode.data.isBetterPowerUp = maze.pickupItems.TryGetValue(PickupType.POWER_PELLET,out cherryList) && cherryList.Contains(neighbourPos);
            List<Vector2> pillList;
            NewNode.data.isPallet = maze.pickupItems.TryGetValue(PickupType.PILL,out  pillList) && pillList.Contains(neighbourPos);
            nodeDict.Add(neighbourPos,NewNode);            
            nodeMY.SetEdge(NewNode,1); 
            NewNode.SetEdge(nodeMY,1);
            GenerateRecursivelyGraph(NewNode);
        }
        
    }
   
}
