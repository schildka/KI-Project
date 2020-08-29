using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reads a Maze from a file, instantiates the walls, and provides positional information for ghosts, player start, and pickups.
/// </summary>
public class Maze : MonoBehaviour
{
    public Vector2 lair;
    public Vector2 ghostSpawn;
    public Vector2 msPacManSpawn;

    public Dictionary<PickupType, List<Vector2>> pickupItems = new Dictionary<PickupType, List<Vector2>>();

    public int mazeWidth;
    public int mazeHeight;
    public char[,] mazeData;
    public Tile[,] tiles;

    /// <summary>
    /// Checks if a tile  is walkable, meaning not a Wall.
    /// </summary>
    /// <returns><c>true</c>, if tile walkable, <c>false</c> otherwise.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public bool IsTileWalkable(int x, int y)
    {
        if (x < 0 || x >= mazeWidth || y < 0 || y >= mazeHeight)
            return false;

        return !(tiles[x, y].Type == TileType.WALL
                 || tiles[x, y].Type == TileType.OUT_OF_BOUNDS);
    }

    /// <summary>
    /// Checks if a tile is walkable, meaning not a Wall.
    /// </summary>
    /// <returns><c>true</c>, if tile walkable, <c>false</c> otherwise.</returns>
    /// <param name="position">The (x,y) coordinate.</param>
    public bool IsTileWalkable(Vector2 position)
    {
        return IsTileWalkable((int)position.x, (int)position.y);
    }

    /// <summary>
    /// Returns the possible move directions for a tile.
    /// </summary>
    /// <returns>The possible directions of tile.</returns>
    /// <param name="tilePosition">Tile position.</param>
    public List<Direction> GetPossibleDirectionsAt(Vector2 tilePosition)
    {
        List<Direction> result = new List<Direction>();

        if (IsTileWalkable(tilePosition + Vector2.up))
            result.Add(Direction.UP);
        if (IsTileWalkable(tilePosition + Vector2.down))
            result.Add(Direction.DOWN);
        if (IsTileWalkable(tilePosition + Vector2.left))
            result.Add(Direction.LEFT);
        if (IsTileWalkable(tilePosition + Vector2.right))
            result.Add(Direction.RIGHT);

        return result;
    }

    public void LoadMaze(TextAsset mazeFile)
    {
        pickupItems.Clear();

        ReadTextfile(mazeFile);
        ParseTileData();

        UpdateReachability();

        var generator = GetComponent<WallSpriteGenerator>();
        if (generator)
            generator.Generate(this);
    }

    void ReadTextfile(TextAsset mazeFile)
    {
        string[] lines = mazeFile.text.Split('\n');

        mazeWidth = lines[0].Length;
        mazeHeight = lines.Length;

        mazeData = new char[mazeWidth, mazeHeight];

        int worldY = 0;
        for (int arrayY = mazeHeight - 1; arrayY >= 0; arrayY--)
        { //Starts at the last line because we want the tiles' array-positions to match their world-position.
            for (int x = 0; x < lines[arrayY].Length; x++)
            {
                mazeData[x, worldY] = lines[arrayY][x];
            }
            worldY++;
        }
    }

    void ParseTileData()
    {
        tiles = new Tile[mazeWidth, mazeHeight];
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                char tile = mazeData[x, y];
                switch (tile)
                {
                    case '+': // Junctions currently not implemented
                    case '#':
                        tiles[x, y] = new Tile(TileType.WALL);
                        break;
                    case 'p':
                        tiles[x, y] = new Tile(TileType.PLAYER_SPAWN);
                        msPacManSpawn = new Vector2(x, y);
                        break;
                    case '.':
                        tiles[x, y] = new Tile(TileType.PILL);

                        if (!pickupItems.ContainsKey(PickupType.PILL))
                            pickupItems[PickupType.PILL] = new List<Vector2>();

                        pickupItems[PickupType.PILL].Add(new Vector2(x, y));
                        break;
                    case '*':
                        tiles[x, y] = new Tile(TileType.POWER_PELLET);

                        if (!pickupItems.ContainsKey(PickupType.POWER_PELLET))
                            pickupItems[PickupType.POWER_PELLET] = new List<Vector2>();

                        pickupItems[PickupType.POWER_PELLET].Add(new Vector2(x, y));
                        break;
                    case 'g':
                        tiles[x, y] = new Tile(TileType.GHOST_SPAWN);
                        ghostSpawn = new Vector2(x, y);
                        break;
                    case 'l':
                        tiles[x, y] = new Tile(TileType.EMPTY);
                        lair = new Vector2(x, y);
                        break;
                    default:
                        tiles[x, y] = new Tile(TileType.EMPTY);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Sets the type of tiles that are neither walls nor reachable from the spawn point of pacman to TileType.OUT_OF_BOUNDS.
    /// </summary>
    void UpdateReachability()
    {
        var unreachableSet = new HashSet<Vector2>();
        var toCheck = new Queue<Vector2>();

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (tiles[x, y].Type != TileType.WALL)
                    unreachableSet.Add(new Vector2(x, y));
            }
        }

        toCheck.Enqueue(msPacManSpawn);
        unreachableSet.Remove(msPacManSpawn);

        while (toCheck.Count > 0)
        {
            var current = toCheck.Dequeue();

            foreach (var d in GetPossibleDirectionsAt(current))
            {
                var succ = current + d.ToVector2();

                if (!unreachableSet.Contains(succ))
                    continue;

                unreachableSet.Remove(succ);
                toCheck.Enqueue(succ);
            }
        }

        foreach (var p in unreachableSet)
        {
            tiles[(int)p.x, (int)p.y] = new Tile(TileType.OUT_OF_BOUNDS);
        }
    }
}
