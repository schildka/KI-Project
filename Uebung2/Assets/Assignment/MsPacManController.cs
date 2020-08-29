using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Eyes))]
[RequireComponent(typeof(MazeMap))]
[RequireComponent(typeof(MazeGraphForPacMan))]
[RequireComponent(typeof(GlobalKnowledgeSensor))]
public class MsPacManController : AgentController<MsPacMan>
{
    Eyes eyes;
    MazeMap map;
    MazeGraphForPacMan graph;
    GlobalKnowledgeSensor knowledge;

    protected override void Awake()
    {
        base.Awake();

        eyes = GetComponent<Eyes>();
        map = GetComponent<MazeMap>();
        graph = GetComponent<MazeGraphForPacMan>();
        knowledge = GetComponent<GlobalKnowledgeSensor>();
    }

    public override void OnDecisionRequired()
    {
        // TODO
        OnTileReached();
    }

    public override void OnTileReached()
    {
        // TODO
        var loc = agent.currentTile;
        var path = new List<Graphs.Node<MazeGraphForPacMan.TileData>>();
        var newLoc = new Vector2();

        if (isAnyGhostEdible())
        {
            double cost;
            Vector2 FindLoc = getClosestGhost(loc);
            if (!Graphs.AStar.Search(
                graph.getNode(loc),
                (n) => { return (n.data.position - FindLoc).magnitude; },
                (n) => { return n.data.position == FindLoc; },
                out path,
                out cost))
            {
                Debug.Log("No path found!");
                agent.Move(GetRandomPossibleDirection());
            }
            else
            {
                newLoc = path[0].data.position;
                path[0].data.isPallet = false;
                path[0].data.isBetterPowerUp = false;
            }
        }
        else
        {
            Graphs.BreadthFirstSearch.Search(
                graph.getNode(loc),
                (n) => { return n.data.isBetterPowerUp; },
                out path);

            if (path.Count == 0)
            {
                agent.Move(GetRandomPossibleDirection());
                Graphs.BreadthFirstSearch.Search(
                    graph.getNode(loc),
                    (n) => { return n.data.isPallet; },
                    out path);
                if (path.Count == 0)
                {
                    agent.Move(GetRandomPossibleDirection());
                    return;
                }
            }


            path[1].data.isPallet = false;
            path[1].data.isBetterPowerUp = false;
            newLoc = path[1].data.position;
        }

        map.maze.DecreaseSmells();

        map.maze.IncreaseSmell(newLoc);


        if (agent.currentTile.x < newLoc.x)
        {
            agent.Move(Direction.RIGHT);
            return;
        }

        if (agent.currentTile.x > newLoc.x)
        {
            agent.Move(Direction.LEFT);
            return;
        }

        if (agent.currentTile.y < newLoc.y)
        {
            agent.Move(Direction.UP);
            return;
        }

        if (agent.currentTile.y > newLoc.y)
        {
            agent.Move(Direction.DOWN);
            return;
        }
    }

    public Vector2 getClosestGhost(Vector2 yourLocation)
    {
        var Blinky = knowledge.GetGhost(GhostName.BLINKY).currentTile;
        var Inky = knowledge.GetGhost(GhostName.INKY).currentTile;
        var Pinky = knowledge.GetGhost(GhostName.PINKY).currentTile;
        var Sue = knowledge.GetGhost(GhostName.SUE).currentTile;
        var GhostArray = new List<Vector2>() {Blinky, Inky, Pinky, Sue};
        float smallestDist = float.MaxValue;
        Vector2 location = new Vector2();
        foreach (var ghost in GhostArray)
        {
            float dist = (ghost - yourLocation).magnitude;
            if (dist < smallestDist)
            {
                smallestDist = dist;
                location = ghost;
            }
        }

        return location;
    }

    public bool isAnyGhostEdible()
    {
        return knowledge.GetGhost(GhostName.BLINKY).IsEdible() ||
               knowledge.GetGhost(GhostName.INKY).IsEdible() ||
               knowledge.GetGhost(GhostName.PINKY).IsEdible() ||
               knowledge.GetGhost(GhostName.SUE).IsEdible();
    }

    /// <summary>
    /// Returns the possible moves for the current tile, excluding those supplied as parameters.
    /// </summary>
    /// <returns>A randomly chosen possible direction, or NONE if none remains.</returns>
    /// <param name="exceptDirection">Directions to exclude.</param>
    Direction GetRandomPossibleDirection(params Direction[] exceptDirection)
    {
        List<Direction> possibleMoves = map.GetPossibleMoves();

        foreach (var d in exceptDirection)
            possibleMoves.Remove(d);

        if (possibleMoves.Count > 0)
            return possibleMoves[Random.Range(0, possibleMoves.Count)];

        return Direction.NONE;
    }
}