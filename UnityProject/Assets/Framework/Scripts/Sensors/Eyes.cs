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
    public VisualPercept Look(Direction direction, int LayerMask = IgnorePacManMask, bool drawRay = false)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(Agent.currentTile, direction.ToVector2(), 50, LayerMask);
        if (hit2D.collider == null)
            return null;

        GameObject obj = hit2D.collider.gameObject;

        VisualPercept percept = null;

        if (obj.GetComponent<Ghost>())
        {
            if (drawRay) Debug.DrawLine(Agent.currentTile, obj.transform.position, Color.red);

            Ghost g = obj.GetComponent<Ghost>();
            percept = new VisualGhostPercept(g, hit2D.distance);
        }
        else if (obj.GetComponent<PickupItem>())
        {
            if (drawRay) Debug.DrawLine(Agent.currentTile, obj.transform.position, Color.cyan);

            PickupItem p = obj.GetComponent<PickupItem>();
            percept = new VisualPickupPercept(p, hit2D.distance);
        }
        else if (obj.GetComponent<MsPacMan>())
        {
            if (drawRay) Debug.DrawLine(Agent.currentTile, obj.transform.position, Color.yellow);

            MsPacMan msp = obj.GetComponent<MsPacMan>();
            percept = new VisualPacManPercept(msp, hit2D.distance);
        }

        return percept;
    }
}

public abstract class VisualPercept
{
    public readonly Vector2 position;
    public readonly float distance;

    protected VisualPercept(Vector2 position, float distance)
    {
        this.position = position;
        this.distance = distance;
    }

    public override string ToString()
    {
        return string.Format("Percept ({0}, {1})", position.x, position.y);
    }
}


public abstract class VisualPercept<T> : VisualPercept where T : MonoBehaviour
{
    protected VisualPercept(T behavior, float distance) :
        base(behavior.transform.position, distance)
    { }
}


public class VisualGhostPercept : VisualPercept<Ghost>
{
    public readonly Direction currentDirection;
    public readonly GhostName name;
    public readonly bool isEdible;

    public VisualGhostPercept(Ghost ghost, float distance) : base(ghost, distance)
    {
        currentDirection = ghost.currentMove;
        name = ghost.id;
        isEdible = ghost.IsEdible();
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}, edible = {2}, currentMove = {3}", base.ToString(), name, isEdible, currentDirection);
    }
}

public class VisualPacManPercept : VisualPercept<MsPacMan>
{
    public readonly Direction currentDirection;

    public VisualPacManPercept(MsPacMan msPacMan, float distance) : base(msPacMan, distance)
    {
        currentDirection = msPacMan.currentMove;
    }

    public override string ToString()
    {
        return string.Format("{0}: MsPacMan, currentMove = {1}", base.ToString(), currentDirection);
    }
}

public class VisualPickupPercept : VisualPercept<PickupItem>
{
    public readonly PickupType type;

    public VisualPickupPercept(PickupItem pickup, float distance) : base(pickup, distance)
    {
        type = pickup.type;
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}", base.ToString(), type);
    }
}