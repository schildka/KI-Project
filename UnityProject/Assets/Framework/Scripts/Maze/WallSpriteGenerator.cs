using UnityEngine;
using UnityEngine.U2D;

using D = Walkability.D;
using WalkabilityRule = Walkability.WalkabilityRule;

public class WallSpriteGenerator : MonoBehaviour
{
    [SerializeField]
    SpriteAtlas spriteAtlas = null;
    [SerializeField]
    GameObject wallTile = null;

    GameObject walls;

    WalkabilityRule PATH_H = new WalkabilityRule(D.W | D.X | D.E,
                                                 D.N | D.NE | D.NW | D.SE | D.S | D.SW);
    WalkabilityRule INTERSECT_Q = new WalkabilityRule(D.S | D.N | D.X | D.E | D.W,
                                                      D.NE | D.NW | D.SE | D.SW);
    WalkabilityRule INTERSECT_T = new WalkabilityRule(D.E | D.S | D.W | D.X,
                                                      D.NE | D.NW | D.N | D.SW | D.SE);
    WalkabilityRule CORNER_L = new WalkabilityRule(D.N | D.X | D.E,
                                                   D.NE | D.NW | D.SE | D.SW | D.S | D.W);
    WalkabilityRule GAP_1 = new WalkabilityRule(D.SW | D.W | D.X | D.E,
                                                D.N | D.NE | D.NW | D.S | D.SE);
    WalkabilityRule GAP_2 = new WalkabilityRule(D.W | D.X | D.E | D.SE,
                                                D.N | D.NE | D.NW | D.S | D.SW);
    WalkabilityRule DEADEND = new WalkabilityRule(D.S | D.X,
                                                 D.N | D.NE | D.NW | D.SE | D.E | D.SW | D.W);

    public void Generate(Maze maze)
    {
        if (walls != null)
            Destroy(walls);

        walls = new GameObject("Walls");

        for (int y = 0; y < maze.mazeHeight; y++)
        {
            for (int x = 0; x < maze.mazeWidth; x++)
            {
                var w = Walkability.Calculate(maze, x, y);
                if ((w & D.X) == D.X)
                {
                    if (spriteAtlas != null)
                    {
                        var p = new Vector2(x, y);
                        if (PATH_H.Match(w, 0))
                        {
                            CreatePath(p, 0);
                            continue;
                        }
                        if (PATH_H.Match(w, 2))
                        {
                            CreatePath(p, 2);
                            continue;
                        }
                        if (INTERSECT_Q.Match(w))
                        {
                            CreateCrossing(p);
                            continue;
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            int rot = i * 2;
                            if (INTERSECT_T.Match(w, rot))
                            {
                                CreateT(p, rot);
                                continue;
                            }
                            if (CORNER_L.Match(w, rot))
                            {
                                CreateL(p, rot);
                                continue;
                            }
                            if (GAP_1.Match(w, rot))
                            {
                                FillGap(p, rot);
                                continue;
                            }
                            if (GAP_2.Match(w, rot))
                            {
                                FillGap(p, rot);
                                continue;
                            }
                            if (DEADEND.Match(w, rot))
                            {
                                Deadend(p, rot);
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    void FillGap(Vector2 p, int rot)
    {
        var q = Quaternion.AngleAxis(45 * rot, Vector3.back);
        CreateWall(p, Walkability.Rotate(D.N, rot), WallSprite.NORMAL_WALL, q * UP);
    }

    void CreatePath(Vector2 p, int rot)
    {
        var q = Quaternion.AngleAxis(45 * rot, Vector3.back);
        CreateWall(p, Walkability.Rotate(D.N, rot), WallSprite.NORMAL_WALL, q * UP);
        CreateWall(p, Walkability.Rotate(D.S, rot), WallSprite.NORMAL_WALL, q * DOWN);
    }

    void CreateL(Vector2 p, int rot)
    {
        var q = Quaternion.AngleAxis(45 * rot, Vector3.back);
        CreateWall(p, Walkability.Rotate(D.W, rot), WallSprite.NORMAL_WALL, q * LEFT);
        CreateWall(p, Walkability.Rotate(D.S, rot), WallSprite.NORMAL_WALL, q * DOWN);
        CreateWall(p, Walkability.Rotate(D.SW, rot), WallSprite.NORMAL_CORNER_INNER, q * LEFT);
        CreateWall(p, Walkability.Rotate(D.NE, rot), WallSprite.NORMAL_CORNER, q * LEFT);
    }

    void CreateT(Vector2 p, int rot)
    {
        var q = Quaternion.AngleAxis(45 * rot, Vector3.back);
        CreateWall(p, Walkability.Rotate(D.N, rot), WallSprite.NORMAL_WALL, q * UP);
        CreateWall(p, Walkability.Rotate(D.SW, rot), WallSprite.NORMAL_CORNER, q * RIGHT);
        CreateWall(p, Walkability.Rotate(D.SE, rot), WallSprite.NORMAL_CORNER, q * UP);
    }

    void Deadend(Vector2 p, int rot)
    {
        var q = Quaternion.AngleAxis(45 * rot, Vector3.back);
        CreateWall(p, Walkability.Rotate(D.N, rot), WallSprite.NORMAL_WALL, q * UP);
        CreateWall(p, Walkability.Rotate(D.E, rot), WallSprite.NORMAL_WALL, q * RIGHT);
        CreateWall(p, Walkability.Rotate(D.W, rot), WallSprite.NORMAL_WALL, q * LEFT);
        CreateWall(p, Walkability.Rotate(D.NE, rot), WallSprite.NORMAL_CORNER_INNER, q * RIGHT);
        CreateWall(p, Walkability.Rotate(D.NW, rot), WallSprite.NORMAL_CORNER_INNER, q * UP);
    }

    void CreateCrossing(Vector2 p)
    {
        CreateWall(p, D.NE, WallSprite.NORMAL_CORNER, LEFT);
        CreateWall(p, D.SW, WallSprite.NORMAL_CORNER, RIGHT);
        CreateWall(p, D.NW, WallSprite.NORMAL_CORNER, DOWN);
        CreateWall(p, D.SE, WallSprite.NORMAL_CORNER, UP);
    }

    GameObject CreateWall(Vector2 p, D offset, WallSprite t, Quaternion ori)
    {
        var pAdj = p + Walkability.ToVec2(offset);
        return CreateWall(pAdj, t, ori);
    }

    GameObject CreateWall(Vector2 p, WallSprite t, Quaternion ori)
    {
        var go = Instantiate(wallTile, p, ori, walls.transform);
        Sprite sprite = spriteAtlas.GetSprite("walls_" + (int)t);
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        return go;
    }

    enum WallSprite
    {
        BOUNDS_CORNER,
        NORMAL_CORNER,
        NORMAL_DOUBLE_CORNER_INNER,
        NORMAL_CORNER_INNER,
        BOUNDS_WALL,
        NORMAL_WALL,
        LAIR_CORNER,
        LAIR_WALL,
        LAIR_EDGE,
        EMPTY
    }

    readonly Quaternion UP = Quaternion.identity;
    readonly Quaternion RIGHT = Quaternion.AngleAxis(90, Vector3.back);
    readonly Quaternion LEFT = Quaternion.AngleAxis(-90, Vector3.back);
    readonly Quaternion DOWN = Quaternion.AngleAxis(180, Vector3.back);
}
