using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides information about the Maze.
/// </summary>
public class MazeMap : Sensor
{
    public Maze Maze
    {
        get;
        private set;
    }

    protected override void Awake()
    {
        base.Awake();
        Maze = GameObject.Find("Maze").GetComponent<Maze>();
    }

    /// <summary>k
    /// Provides the possible moves based on the agents position.
    /// </summary>
    /// <returns>The possible move directions.</returns>
    public List<Direction> GetPossibleMoves()
    {
        return Maze.GetPossibleDirectionsAt(Agent.currentTile);
    }

    /* Extend the Sensor if desired. */
}
