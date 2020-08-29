
public class Tile
{
    public TileType Type { get; private set; }

    public Tile(TileType type)
    {
        Type = type;
    }
}

public enum TileType
{
    EMPTY,
    WALL,
    PLAYER_SPAWN,
    PILL,
    POWER_PELLET,
    GHOST_SPAWN,
    JUNCTION,
    OUT_OF_BOUNDS
}