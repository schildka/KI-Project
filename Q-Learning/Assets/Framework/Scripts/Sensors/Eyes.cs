using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Eyes provide the agent the ability to identify objects at a distance.
/// </summary>
[RequireComponent(typeof(Agent))]
public class Eyes : Sensor
{

    public const int IgnorePacManMask = ~(1 << 8);
    public const int IgnoreGhostsMask = ~(1 << 9);
    public const int IgnorePickups = ~(1 << 10);

    /// <summary>
    /// Checks for Ghosts or Items in the specified direction and returns the first object and its distance found, if any.
    /// </summary>
    /// <returns>The first perceived object.</returns>
    /// <param name="direction">Direction to look at.</param>
    public Percept Look(Direction direction, int LayerMask = IgnorePacManMask, bool debug = false)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(agent.currentTile, direction.ToVector2(), 50, LayerMask);
        GameObject obj = hit2D.collider.gameObject;

        Percept percept;

        if (obj.GetComponent<Ghost>())
        {
            if(debug) Debug.DrawLine(agent.currentTile, obj.transform.position, Color.red);

            percept.type = obj.GetComponent<Ghost>().IsEdible() ? PerceptType.SCARED_GHOST : PerceptType.GHOST;
            percept.distance = Mathf.RoundToInt(hit2D.distance);
        }
        else if (obj.GetComponent<PickupItem>())
        {
            if (debug) Debug.DrawLine(agent.currentTile, obj.transform.position, Color.cyan);

            percept.type = PerceptType.ITEM;
            percept.distance = Mathf.RoundToInt(hit2D.distance);
        }
        else if (obj.GetComponent<MsPacMan>())
        {
            if (debug)  Debug.DrawLine(agent.currentTile, obj.transform.position, Color.yellow);

            percept.type = PerceptType.PACMAN;
            percept.distance = Mathf.RoundToInt(hit2D.distance);
        }
        else
        {
            percept.type = PerceptType.NOTHING;
            percept.distance = 0;
        }

        return percept;
    }
}

public struct Percept
{
    public int distance;
    public PerceptType type;
}

public enum PerceptType
{
    GHOST,
    SCARED_GHOST,
    ITEM,
    NOTHING,
    PACMAN
}