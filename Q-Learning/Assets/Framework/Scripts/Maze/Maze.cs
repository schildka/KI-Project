﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reads a Maze from a file, instantiates the walls, and provides positional information for ghosts, player start, and pickups.
/// </summary>
public class Maze : MonoBehaviour
{
    [SerializeField] GameObject wallTile = null;
    [SerializeField] GameObject Walls = null;

    public Vector2 lair;
    public Vector2 ghostSpawn;
    public Vector2 msPacManSpawn;
    public Vector2 junctionOne;
    public Vector2 junctionTwo;

    public Dictionary<PickupType, List<Vector2>> pickupItems = new Dictionary<PickupType, List<Vector2>>();

    // Here the Dictionary?
    public Dictionary<Vector2, int> smells = new Dictionary<Vector2, int>();

    public int Width { get; private set; }

    public int Height { get; private set; }

    char[,] mazeData;
    Tile[,] tiles;


    public void DecreaseSmells()
    {
        var keys = new List<Vector2>(smells.Keys);
        foreach (var smell in keys)
        {
            if (smells[smell] != 0) smells[smell] = smells[smell] - 1;
        }
    }

    public void IncreaseSmell(Vector2 vec)
    {
        smells[vec] = 25;
    }


    /// <summary>
    /// Checks if a tile  is walkable, meaning not a Wall.
    /// </summary>
    /// <returns><c>true</c>, if tile walkable, <c>false</c> otherwise.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public bool IsTileWalkable(int x, int y)
    {
        return !(tiles[x, y].type == TileType.WALL);
    }

    /// <summary>
    /// Checks if a tile is walkable, meaning not a Wall.
    /// </summary>
    /// <returns><c>true</c>, if tile walkable, <c>false</c> otherwise.</returns>
    /// <param name="position">The (x,y) coordinate.</param>
    public bool IsTileWalkable(Vector2 position)
    {
        return IsTileWalkable((int) position.x, (int) position.y);
    }

    /// <summary>
    /// Returns the possible move directions for a tile.
    /// </summary>
    /// <returns>The possible directions of tile.</returns>
    /// <param name="tilePosition">Tile position.</param>
    public List<Direction> PossibleMoves(Vector2 tilePosition)
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
        DestroyChildren(Walls);
        pickupItems.Clear();

        ReadTextfile(mazeFile);
        ParseTileData();
        SpawnWalls();
    }

    void ReadTextfile(TextAsset mazeFile)
    {
        string[] lines = mazeFile.text.Split('\n');

        Width = lines[0].Length;
        Height = lines.Length;

        mazeData = new char[Width, Height];

        int worldY = 0;
        for (int arrayY = Height - 1; arrayY >= 0; arrayY--)
        {
            //Starts at the last line because we want the tiles' array-positions to match their world-position.
            for (int x = 0; x < lines[arrayY].Length; x++)
            {
                mazeData[x, worldY] = lines[arrayY][x];
            }

            worldY++;
        }
    }

    void ParseTileData()
    {
        tiles = new Tile[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char tile = mazeData[x, y];
                switch (tile)
                {
                    case '+':
                    //tiles[x, y] = new Tile(TileType.JUNCTION);
                    //if (junctionOne == Vector2.zero)
                    //    junctionOne = new Vector2(x, y);
                    //else
                    //    junctionTwo = new Vector2(x, y);
                    //break;
                    case '#':
                        tiles[x, y] = new Tile(TileType.WALL);
                        break;
                    case 'p':
                        tiles[x, y] = new Tile(TileType.PLAYER_SPAWN);
                        msPacManSpawn = new Vector2(x, y);
                        smells.Add(msPacManSpawn, 0);
                        break;
                    case '.':
                        tiles[x, y] = new Tile(TileType.PILL);

                        if (!pickupItems.ContainsKey(PickupType.PILL))
                            pickupItems[PickupType.PILL] = new List<Vector2>();

                        pickupItems[PickupType.PILL].Add(new Vector2(x, y));
                        smells.Add(new Vector2(x, y), 0);
                        break;
                    case '*':
                        tiles[x, y] = new Tile(TileType.POWER_PELLET);

                        if (!pickupItems.ContainsKey(PickupType.POWER_PELLET))
                            pickupItems[PickupType.POWER_PELLET] = new List<Vector2>();

                        pickupItems[PickupType.POWER_PELLET].Add(new Vector2(x, y));
                        smells.Add(new Vector2(x, y), 0);
                        break;
                    case 'g':
                        tiles[x, y] = new Tile(TileType.GHOST_SPAWN);
                        ghostSpawn = new Vector2(x, y);
                        smells.Add(ghostSpawn, 0);
                        break;
                    case 'l':
                        tiles[x, y] = new Tile(TileType.NONE);
                        lair = new Vector2(x, y);
                        smells.Add(lair, 0);
                        break;
                    default:
                        tiles[x, y] = new Tile(TileType.NONE);
                        smells.Add(new Vector2(x, y), 0);
                        break;
                }
            }
        }
    }

    void SpawnWalls()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (tiles[x, y].type == TileType.WALL)
                {
                    Instantiate(wallTile, new Vector2(x, y), Quaternion.identity, Walls.transform);
                }
            }
        }
    }

    void DestroyChildren(GameObject go)
    {
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(go.transform.GetChild(i));
        }
    }
}