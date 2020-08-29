using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(MazeMap))]
public class Nose : Sensor
{
    public MazeMap _map;
    private Agent _agent;

    private void Awake()
    {
        _map = GetComponent<MazeMap>();
        _agent = GetComponent<Agent>();
    }

    public int Smell(Direction direction)
    {
        return _map.GetSmellAt(_agent.currentTile + direction.ToVector2());
    }
}