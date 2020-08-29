using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(MazeMap))]
public class Ears : Sensor
{
    public int radius;


    public double hear(Vector2 pacPos, Vector2 ghostPos)
    {
        var dist = (pacPos - ghostPos).magnitude;

        if (dist > 9) return 10;
        return dist;
    }
}