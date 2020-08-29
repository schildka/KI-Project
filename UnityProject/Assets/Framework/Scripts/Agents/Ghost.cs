using UnityEngine;

/// <summary>
/// Ghosts cannot turn around 180° on their own.
/// 
/// Also, depending on the GameState, the animation changes.
/// </summary>
public class Ghost : Agent
{
    public GhostName id = GhostName.PINKY;

    float edibleTimer = 0f;

    protected override bool IsMoveValid(Direction move)
    {
        // Do not turn around, except at dead ends
        return (currentMove.Opposite() != move && move != Direction.NONE)
            || maze.GetPossibleDirectionsAt(currentTile).Count == 1;
    }

    protected override void Update()
    {
        base.Update();

        if (edibleTimer > 0f)
            edibleTimer -= Time.deltaTime;
    }

    protected override void UpdateAnimation()
    {
        if (IsEdible())
        {
            animator.SetTrigger("BLINK");
        }
        else
        {
            base.UpdateAnimation();
        }
    }

    public bool IsEdible()
    {
        return edibleTimer > 0;
    }

    public override void Reset(Vector2 position)
    {
        base.Reset(position);
        edibleTimer = 0f;
    }

    public void SetEdible(float seconds)
    {
        edibleTimer = seconds;
    }
}


public enum GhostName
{
    PINKY,
    INKY,
    BLINKY,
    SUE
}