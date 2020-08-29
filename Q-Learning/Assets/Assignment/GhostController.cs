using System.Collections.Generic;
using UnityEngine;
using Graphs;
using UnityEngine.Networking;

[RequireComponent(typeof(Eyes))]
[RequireComponent(typeof(Nose))]
[RequireComponent(typeof(Ears))]
[RequireComponent(typeof(MazeMap))]
[RequireComponent(typeof(MazeGraphForGhosts))]
[RequireComponent(typeof(GlobalKnowledgeSensor))]
public class GhostController : AgentController<Ghost>
{
    Eyes eyes;
    Nose nose;
    Ears ears;
    MazeMap map;
    MazeGraphForGhosts graph;
    GlobalKnowledgeSensor knowledge;
    private Vector2 nextloc = new Vector2(1, 1);

    protected override void Awake()
    {
        base.Awake();

        eyes = GetComponent<Eyes>();
        map = GetComponent<MazeMap>();
        graph = GetComponent<MazeGraphForGhosts>();
        knowledge = GetComponent<GlobalKnowledgeSensor>();
        nose = GetComponent<Nose>();
        ears = GetComponent<Ears>();
    }

    public override void OnDecisionRequired()
    {
        /*
	    var directions = map.GetPossibleMoves();
        if (agent.IsEdible())
        {            
            foreach (var dir in directions)
            {
                Vector2 dirVector = new Vector2(0,0);
                switch (dir)
                {
                    case Direction.DOWN:
                        dirVector = Vector2.down;
                        break;
                    case Direction.UP:
                        dirVector = Vector2.up;
                        break;
                    case Direction.RIGHT:
                        dirVector = Vector2.right;
                        break;
                    case Direction.LEFT:
                        dirVector = Vector2.left;
                        break;
                }

                var ghostPos = agent.currentTile;
                var pacPos = knowledge.GetMsPacMan().currentTile;
                var standardlength = (pacPos - ghostPos).magnitude;
                if ((ghostPos + dirVector - pacPos).magnitude >= standardlength)
                {
                    agent.Move(dir);
                    return;
                }
            }
           
            agent.Move(GetRandomPossibleDirection());
            return;
        }
    
        if (agent.currentTile.x < nextloc.x && directions.Contains(Direction.RIGHT))
	    {
	        agent.Move(Direction.RIGHT);
	        return;
	    }

        if (agent.currentTile.x > nextloc.x && directions.Contains(Direction.LEFT))
        {
            agent.Move(Direction.LEFT);
            return;
        }

        if (agent.currentTile.y < nextloc.y && directions.Contains(Direction.UP))
        {
            agent.Move(Direction.UP);
            return;
        }

        if (agent.currentTile.y > nextloc.y && directions.Contains(Direction.DOWN))
        {
            agent.Move(Direction.DOWN);
            return;
        } */
        //if(nose._map.maze.smells[agent.currentTile])


        var directions = map.GetPossibleMoves();
        var bestDir = GetRandomPossibleDirection();
        var noSmell = true;


        //SmellMove(directions, noSmell, bestDir);
        HearMove(directions, noSmell, bestDir);
    }

    private void SmellMove(List<Direction> directions, bool noSmell, Direction bestDir)
    {
        if (agent.IsEdible())
        {
            var max = 25;

            foreach (var dir in directions)
            {
                var i = nose.Smell(dir);

                if (i != 0) noSmell = false;

                if (i <= max && !dir.Equals(agent.currentMove.Opposite()))
                {
                    max = i;
                    bestDir = dir;
                }
            }
        }
        else
        {
            var min = 0;

            foreach (var dir in directions)
            {
                var i = nose.Smell(dir);

                if (i != 0) noSmell = false;

                if (i >= min && !dir.Equals(agent.currentMove.Opposite()))
                {
                    min = i;
                    bestDir = dir;
                }
            }
        }

        if (noSmell)
        {
            agent.Move(GetRandomPossibleDirection());
            return;
        }

        agent.Move(bestDir);
    }

    private void HearMove(List<Direction> directions, bool noSound, Direction bestDir)
    {
        var pacPosition = knowledge.GetMsPacMan().currentTile;

        var intensity = ears.hear(pacPosition, agent.currentTile + Direction.UP.ToVector2());

        if (!agent.IsEdible())
        {
            var min = 10.0;

            foreach (var dir in directions)
            {
                var i = ears.hear(pacPosition, agent.currentTile + dir.ToVector2());

                if (i != 10) noSound = false;

                if (i <= min && !dir.Equals(agent.currentMove.Opposite()))
                {
                    min = i;
                    bestDir = dir;
                }
            }
        }
        else
        {
            var max = 0.0;

            foreach (var dir in directions)
            {
                var i = ears.hear(pacPosition, agent.currentTile + dir.ToVector2());

                if (i != 10) noSound = false;

                if (i >= max && !dir.Equals(agent.currentMove.Opposite()))
                {
                    max = i;
                    bestDir = dir;
                }
            }
        }

        if (noSound)
        {
            agent.Move(GetRandomPossibleDirection());
            return;
        }

        agent.Move(bestDir);
    }

    public override void OnTileReached()
    {
        OnDecisionRequired();
    }

    private void Update()
    {
        var goal = graph.GetClosestNode(knowledge.GetMsPacMan().currentTile).data.position;
        var start = graph.GetClosestNode(agent.currentTile);

        var path = new List<Node<MazeGraphForGhosts.TileData>>();
        double cost;
        /*
        if(!AStar.Search(
           start,
            (n) => { return (n.data.position - goal).magnitude; },
            (n) => { return n.data.position == goal; },
            out path,
            out cost))
        {
            Debug.Log("No path found!");
            nextloc = goal;
        }
        else
        {
            nextloc = path[0].data.position;
        }
        return;
        //Debug.Log("PacManTile");
        //Debug.Log(knowledge.GetMsPacMan().currentTile);


        BreadthFirstSearch.Search(
            start,
            (n) => { return n.data.canHavePickup; },
            out path);

        graph.DrawPath(path);
         */
    }

    /// <summary>
    /// Returns the possible moves for the current tile, excluding those supplied as parameters.
    /// </summary>
    /// <returns>A randomly chosen possible direction, or NONE if none remains.</returns>
    /// <param name="exceptDirection">Directions to exclude.</param>
    Direction GetRandomPossibleDirection(params Direction[] exceptDirection)
    {
        List<Direction> possibleMoves = map.GetPossibleMoves();

        // Ghosts cannot turn around
        if (possibleMoves.Count == 2)
        {
            possibleMoves.Remove(agent.currentMove.Opposite());
            return possibleMoves[0];
        }

        foreach (var d in exceptDirection)
            possibleMoves.Remove(d);

        if (possibleMoves.Count > 0)
            return possibleMoves[Random.Range(0, possibleMoves.Count)];

        return Direction.NONE;
    }
}