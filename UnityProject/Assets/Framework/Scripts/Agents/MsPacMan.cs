/// <summary>
/// Ms. PacMan can make any move that is possible.
/// </summary>
public class MsPacMan : Agent
{
    protected override bool IsMoveValid(Direction newDirection)
    {
        // Ms. PacMan can do whatever she likes.
        return true;
    }
}
