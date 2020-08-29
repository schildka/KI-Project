using System;
using UnityEngine;

public static class Walkability
{
    [Flags]
    public enum D
    {
        ZERO = 0,
        N = 1 << 1,
        NE = 1 << 2,
        E = 1 << 3,
        SE = 1 << 4,
        S = 1 << 5,
        SW = 1 << 6,
        W = 1 << 7,
        NW = 1 << 8,
        X = 1 << 9,
    }

    public static Vector2 ToVec2(D state)
    {
        switch (state)
        {
            case D.N:
                return Vector2.up;
            case D.NE:
                return Vector2.up + Vector2.right;
            case D.E:
                return Vector2.right;
            case D.S:
                return Vector2.down;
            case D.SE:
                return Vector2.down + Vector2.right;
            case D.SW:
                return Vector2.down + Vector2.left;
            case D.W:
                return Vector2.left;
            case D.NW:
                return Vector2.up + Vector2.left;
        }
        return Vector2.zero;
    }

    public static D Rotate(D state)
    {
        // Rotate Directions
        int s = ((int)state & ~(int)D.X) << 1;

        // Wrap NW (curr. X) to N
        if ((s & (int)D.X) == (int)D.X)
            s |= (int)D.N;

        // Correct X
        s &= ~(int)D.X;
        s |= (int)(state & D.X);

        //Debug.Log(state + " -> " + rotState);
        return (D)s;
    }

    public static D Rotate(D state, int times)
    {
        D state2 = state;
        for (int i = 0; i < times; i++)
        {
            state2 = Rotate(state2);
        }

        return state2;
    }

    public static D Calculate(Maze maze, int x, int y)
    {
        D state = D.ZERO;

        if (maze.IsTileWalkable(x, y))
            state |= D.X;

        if (maze.IsTileWalkable(x + 1, y + 1))
            state |= D.NE;
        if (maze.IsTileWalkable(x + 1, y - 1))
            state |= D.SE;
        if (maze.IsTileWalkable(x - 1, y - 1))
            state |= D.SW;
        if (maze.IsTileWalkable(x - 1, y + 1))
            state |= D.NW;

        if (maze.IsTileWalkable(x - 1, y))
            state |= D.W;
        if (maze.IsTileWalkable(x + 1, y))
            state |= D.E;
        if (maze.IsTileWalkable(x, y + 1))
            state |= D.N;
        if (maze.IsTileWalkable(x, y - 1))
            state |= D.S;

        return state;
    }

    public struct WalkabilityRule
    {
        public D can;
        public D cannot;

        public WalkabilityRule(D can, D cannot)
        {
            this.can = can;
            this.cannot = cannot;
        }

        public bool Match(D state)
        {
            return (can & state) == can && (cannot & ~state) == cannot;
        }

        public bool Match(D state, int rotations)
        {
            var canRot = Rotate(can, rotations);
            var cannotRot = Rotate(cannot, rotations);
            return (canRot & state) == canRot && (cannotRot & ~state) == cannotRot;
        }
    }
}
