using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Eyes))]
[RequireComponent(typeof(MazeMap))]
[RequireComponent(typeof(MazeGraphForPacMan))]
[RequireComponent(typeof(GlobalKnowledgeSensor))]


public class MsPacManController : AgentController<MsPacMan>
{
    Eyes eyes;
    MazeMap map;
    MazeGraphForPacMan graph;
    GlobalKnowledgeSensor knowledge;


    const int width = 29;
    const int height = 31;
    const double a = 0.01;
    const double g = 0.99;
    int episodes = 0;
    double [,] QMatrix = new double[width,height];
    Dictionary<Situation,double> QDic = new Dictionary<Situation, double>();
    int [,] RMatrix = new int[width,height];
    RTiles[,] tiles = new RTiles[width, height];
    List<Vector2> Pallets = new List<Vector2>();
    List<Vector2> Pills = new List<Vector2>();
    List<Vector2> PalletsDefault = new List<Vector2>();
    List<Vector2> PillsDefault = new List<Vector2>();
    string path = "Assets/Framework/Mazes/Maze1.txt";
    private Vector2 oldLocation;
    private Direction oldMove;


    void OnApplicationQuit()
    {
        WriteQMatrix(QDic);
        read();
        /*
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < width; i++)
        {
            text.Append("\n");
            for (int j = 0; j < height; j++)
            {
                text.Append(Math.Round(QMatrix[i, j],2).ToString());
                text.Append(" ");
            }
        }
        
        /*
        string realp = @"D:\pacppac\Q-Learning\test.txt";
        
        if (!File.Exists(realp)) 
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(realp)) 
            {
                sw.Write(text);
            }	
        }*/
    }

    protected override void Awake()
    {
        base.Awake();

        eyes = GetComponent<Eyes>();
        map = GetComponent<MazeMap>();
        graph = GetComponent<MazeGraphForPacMan>();
        knowledge = GetComponent<GlobalKnowledgeSensor>();



        //GenerateQMatrixDefault();

        //reading from the file
        QMatrix.Initialize();
        //WriteQMatrix(QMatrix);
        //QMatrix = ReadQData();
       

        

        //Read the text from directly from the test.txt file
        InitRMatrix();
        RMatrix.Initialize();
        
    }

    private void UpdateRMatrix()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                switch (tiles[i, j].tilerep)
                {
                    case TileRep.None:
                        RMatrix[i, j] = 0;
                        break;
                    case TileRep.Wall:
                        RMatrix[i, j] = -50;
                        break;
                    default:
                        RMatrix[i, j] = 0;
                        break;
                }
            }
        }

        if (ResetUpdated.OpenReset)
        {
            ResetUpdated.OpenReset = false;
            episodes++;
            Pills = PillsDefault;
            Pallets = PalletsDefault;
            var currQ = QMatrix[(int) oldLocation.x, (int) oldLocation.y];
            var currR = -100;
            if(episodes % 50== 0)Debug.Log("episodes"+episodes);
            
            QMatrix[(int) oldLocation.x, (int) oldLocation.y] =
                (1.0 - a) * currQ + a * (currR + g * GetQPlusOneValue(oldLocation));
            //UpdateQ(oldLocation,oldMove);
            //Debug.Log("NewGameNewLuck Died at :"+oldLocation);
        }

        if (Pills.Contains(agent.currentTile))
        {
            Pills.Remove(agent.currentTile);
            episodes++;
            if(episodes % 50== 0)Debug.Log("episodes"+episodes);
        }
        foreach (var loc in Pills)
        {
            RMatrix[(int) loc.x, (int) loc.y] = 10;
        }

        if (Pallets.Contains(agent.currentTile))
        {
            Pallets.Remove(agent.currentTile);
            episodes++;
            if(episodes % 50== 0)Debug.Log("episodes"+episodes);
        }
        foreach (var loc in Pallets)
        {
            RMatrix[(int) loc.x, (int) loc.y] = 50;
        }

        var BlinkyLoc = knowledge.GetGhost(GhostName.BLINKY).currentTile;
        var InkyLoc = knowledge.GetGhost(GhostName.INKY).currentTile;
        var PinkyLoc = knowledge.GetGhost(GhostName.PINKY).currentTile;
        var SueLoc = knowledge.GetGhost(GhostName.SUE).currentTile;
        if (knowledge.GetGhost(GhostName.BLINKY).IsEdible())
        {
            RMatrix[(int) BlinkyLoc.x, (int) BlinkyLoc.y] = 100;
            RMatrix[(int) InkyLoc.x, (int) InkyLoc.y] = 100;
            RMatrix[(int) PinkyLoc.x, (int) PinkyLoc.y] = 100;
            RMatrix[(int) SueLoc.x, (int) SueLoc.y] = 100;
        }
        else
        {
            RMatrix[(int) BlinkyLoc.x, (int) BlinkyLoc.y] = -100;
            RMatrix[(int) InkyLoc.x, (int) InkyLoc.y] = -100;
            RMatrix[(int) PinkyLoc.x, (int) PinkyLoc.y] = -100;
            RMatrix[(int) SueLoc.x, (int) SueLoc.y] = -100;
        }
    }

    private void InitRMatrix()
    {
        StreamReader reader = new StreamReader(path);
        string maze = reader.ReadToEnd();
        reader.Close();

        string[] lines = maze.Split('\n');

        var Width = lines[0].Length;
        var Height = lines.Length; 
        //Debug.Log(Width);
        //Debug.Log(Height);
        var mazeData = new char[Width, Height];

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

        StringBuilder text = new StringBuilder();

        for (int y = 0; y < Height; y++)
        {
            text.Append("\n");
            for (int x = 0; x < Width; x++)
            {
                char tile = mazeData[x, y];
                switch (tile)
                {
                    case '#':
                        tiles[x, y] = new RTiles(TileRep.Wall);
                        text.Append("#");
                        break;
                    case 'p':
                        tiles[x, y] = new RTiles(TileRep.None);
                        text.Append(" ");
                        break;
                    case '.':
                        tiles[x, y] = new RTiles(TileRep.None);
                        Pills.Add(new Vector2(x, y));
                        text.Append(" ");
                        break;
                    case '*':
                        tiles[x, y] = new RTiles(TileRep.None);
                        Pallets.Add(new Vector2(x, y));
                        text.Append(" ");
                        break;
                    case 'g':
                    case 'l':
                    default:
                        tiles[x, y] = new RTiles(TileRep.None);
                        text.Append(" ");
                        break;
                }
            }
        }
        

        PillsDefault = Pills;
        PalletsDefault = Pallets;
    }


    public override void OnDecisionRequired()
    {
        // TODO
        OnTileReached();
    }

    private bool bka = true;
    public override void OnTileReached()
    {
        if (bka)
        {
            episodes++;
            UpdateRMatrix();
            bka = false;
        }
        UpdateRMatrix();
        var randDecision = Random.Range(0, 100);
        Direction[] decisions = new[] { Direction.DOWN, Direction.LEFT, Direction.NONE, Direction.RIGHT, Direction.UP };
        Direction move;
        var currentTile = agent.currentTile;
        if (randDecision <= 1 && episodes < 1000)
        {
            move = MakeRandomMove(decisions);
        }
        else
        {
            move = MakeQMove(currentTile);
        }
        
        if(episodes<1000)UpdateQ(currentTile,move);
        //UpdateRMatrix();
       
        oldLocation = agent.currentTile;
        oldMove = move;
    }

    private void UpdateQ(Vector2 currentTile,Direction move)
    {
        //var currQ = QMatrix[(int) currentTile.x, (int) currentTile.y];
        var sit = GetSituation(currentTile);
        var sitObject = new Situation(sit, move);
        if (!QDic.ContainsKey(sitObject))
        {
            AddSituations(sit);
        }
        
        var currR = RMatrix[(int) (currentTile.x+ move.ToVector2().x), (int) (currentTile.y+move.ToVector2().y)];
        //Debug.Log("Location"+(int)currentTile.x+ " "+currentTile.y+"   CurrR"+currR +"   currQ"+currQ);
        double currq;
        QDic.TryGetValue(sitObject, out currq);
        QDic[sitObject] = (1.0 - a) * currq + a * (currR + g * GetPlusOne(currentTile));
        //QMatrix[(int) currentTile.x, (int) currentTile.y] =
        //    (1.0 - a) * currq + a * (currR + g * GetPlusOne(currentTile));
        foreach (var var in QDic.Keys)
        {
            if(var.EqualsSit(sitObject))Debug.Log("Found"+getString(QDic,var));
        }
        Debug.Log( getString(QDic,sitObject));
        Debug.Log("          ---------------------              ");
    }

    private void AddSituations(TileRep[] sit)
    {
        QDic.Add(new Situation(sit, Direction.UP), 0);
        QDic.Add(new Situation(sit, Direction.DOWN), 0);
        QDic.Add(new Situation(sit, Direction.LEFT), 0);
        QDic.Add(new Situation(sit, Direction.RIGHT), 0);
        QDic.Add(new Situation(sit, Direction.NONE), 0);
    }

    private double GetQPlusOneValue(Vector2 currentTile)
    {
        List<double> dlist = new List<double>();
        var currD = currentTile + Vector2.down;
        var currU = currentTile + Vector2.up;
        var currL = currentTile + Vector2.left;
        var currR = currentTile + Vector2.right;

        var currDL = currentTile + Vector2.down + Vector2.left;
        var currUR = currentTile + Vector2.up + Vector2.right;
        var currLU = currentTile + Vector2.left + Vector2.up;
        var currRD = currentTile + Vector2.right + Vector2.down;

        if ((int)currD.x >= 0 && (int)currD.x < width
        && (int)currD.y >= 0 && (int)currD.y < height) dlist.Add(QMatrix[(int)currD.x, (int)currD.y]);

        if ((int)currU.x >= 0 && (int)currU.x < width
        && (int)currU.y >= 0 && (int)currU.y < height) dlist.Add(QMatrix[(int)currU.x, (int)currU.y]);

        if ((int)currL.x >= 0 && (int)currL.x < width
        && (int)currL.y >= 0 && (int)currL.y < height) dlist.Add(QMatrix[(int)currL.x, (int)currL.y]);

        if ((int)currR.x >= 0 && (int)currR.x < width
        && (int)currR.y >= 0 && (int)currR.y < height) dlist.Add(QMatrix[(int)currR.x, (int)currR.y]);
        //

        if ((int)currDL.x >= 0 && (int)currDL.x < width
        && (int)currDL.y >= 0 && (int)currDL.y < height) dlist.Add(QMatrix[(int)currDL.x, (int)currDL.y]);

        if ((int)currUR.x >= 0 && (int)currUR.x < width
        && (int)currUR.y >= 0 && (int)currUR.y < height) dlist.Add(QMatrix[(int)currUR.x, (int)currUR.y]);

        if ((int)currLU.x >= 0 && (int)currLU.x < width
        && (int)currLU.y >= 0 && (int)currLU.y < height) dlist.Add(QMatrix[(int)currLU.x, (int)currLU.y]);

        if ((int)currRD.x >= 0 && (int)currRD.x < width
        && (int)currRD.y >= 0 && (int)currRD.y < height) dlist.Add(QMatrix[(int)currRD.x, (int)currRD.y]);

        double min = -1000000;
        foreach (var val in dlist)
        {
            if (val > min)
            {
                min = val;
            }
        }

        return min;

    }
    
    private double GetPlusOne(Vector2 currentTile)
    {
        List<double> listQvalue = new List<double>();
        var arrDir = new Direction[] {Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT, Direction.NONE};
        foreach (var dir in arrDir)
        {
            var up = GetSituation(currentTile + dir.ToVector2());
            Situation upSit = new Situation(up,dir);
            if (QDic.ContainsKey(upSit))
            {
                listQvalue.Add(QDic[upSit]);
            }
            else
            {
                listQvalue.Add(0);
                AddSituations(up);
            }
        }
        listQvalue.Sort();
        return listQvalue[listQvalue.Count - 1];



    }
    private TileRep[] GetSituation(Vector2 currentTile)
    {
        List<double> dlist = new List<double>();
        TileRep[] sit = new TileRep[8];
        var currD = currentTile + Vector2.down;
        var currU = currentTile + Vector2.up;
        var currL = currentTile + Vector2.left;
        var currR = currentTile + Vector2.right;

        var currDL = currentTile + Vector2.down + Vector2.left;
        var currUR = currentTile + Vector2.up + Vector2.right;
        var currLU = currentTile + Vector2.left + Vector2.up;
        var currRD = currentTile + Vector2.right + Vector2.down;

        
        if ((int)currLU.x >= 0 && (int)currLU.x < width
                               && (int)currLU.y >= 0 && (int)currLU.y < height)  sit[0] = RValueToTile((int) currLU.x, (int) currLU.y);
        else sit[0] = TileRep.Wall;
        
        if ((int)currU.x >= 0 && (int)currU.x < width
                              && (int)currU.y >= 0 && (int)currU.y < height)  sit[1] = RValueToTile((int) currU.x, (int) currU.y);
        else sit[1] = TileRep.Wall;
        
        if ((int)currUR.x >= 0 && (int)currUR.x < width
                               && (int)currUR.y >= 0 && (int)currUR.y < height)  sit[2] = RValueToTile((int) currUR.x, (int) currUR.y);
        else sit[2] = TileRep.Wall;
        
        if ((int)currL.x >= 0 && (int)currL.x < width
                              && (int)currL.y >= 0 && (int)currL.y < height)  sit[3] = RValueToTile((int) currL.x, (int) currL.y);
        else sit[3] = TileRep.Wall;
        
        if ((int)currR.x >= 0 && (int)currR.x < width
                              && (int)currR.y >= 0 && (int)currR.y < height)  sit[4] = RValueToTile((int) currR.x, (int) currR.y);
        else sit[4] = TileRep.Wall;
        
        

        if ((int)currDL.x >= 0 && (int)currDL.x < width
        && (int)currDL.y >= 0 && (int)currDL.y < height)  sit[5] = RValueToTile((int) currDL.x, (int) currDL.y);
        else sit[5] = TileRep.Wall;

        if ((int) currD.x >= 0 && (int) currD.x < width
                               && (int) currD.y >= 0 &&(int) currD.y < height) sit[6] = RValueToTile((int) currD.x, (int) currD.y);
        else sit[6] = TileRep.Wall;
        
      
        if ((int)currRD.x >= 0 && (int)currRD.x < width
        && (int)currRD.y >= 0 && (int)currRD.y < height)  sit[7] = RValueToTile((int) currRD.x, (int) currRD.y);
        else sit[7] = TileRep.Wall;

        if(sit.Contains(TileRep.EdGhost) || sit.Contains(TileRep.Ghost))Debug.Log("there is a ghost");
        return sit;

    }

    private TileRep RValueToTile(int x, int y)
    {
        switch (RMatrix[x, y])
        {
            case 0:
                return TileRep.None;
                break;
            case 10:
                return  TileRep.Pill;
                break;
            case -50:
                return TileRep.Wall;
            case -100:
                return TileRep.Ghost;
            case 100:
                return TileRep.EdGhost;
            case 50:
                return TileRep.Pallet;
            default:
                return TileRep.None;
                    
        }
    }

    private Direction MakeRandomMove(Direction[] decisions)
    {
        var RandomMove = decisions[Random.Range(0, decisions.Length)];
        agent.Move(RandomMove);
        return RandomMove;
    }

    private Situation last; 
    private Direction MakeQMove(Vector2 current)
    {
        var situation = GetSituation(current);
        
        List<Direction> arrDir = new List<Direction>(){Direction.DOWN, Direction.UP, Direction.NONE, Direction.RIGHT, Direction.LEFT};
        int rand = Random.Range(0, arrDir.Count);
        Direction temp = Direction.LEFT;
        arrDir[arrDir.Count - 1] = arrDir[rand];
        arrDir[rand] = temp;
        foreach (var dir in arrDir)
        {                                                       
            var situationNew = new Situation(situation,dir);
            if(!QDic.ContainsKey(situationNew))QDic.Add(situationNew,0);
        }
        
        
        
        List<Situation> mylist = new List<Situation>();
        foreach (Direction dir in arrDir)                          
        {
            Situation sitCurr = new Situation(situation,dir);            
            
            if (!QDic.ContainsKey(sitCurr))
            {
                Debug.Log(dir);
            }

            double val = QDic[sitCurr]; 
            sitCurr.value = val;
            mylist.Add(sitCurr);
        }

        Situation returnVal = new Situation(situation,Direction.UP);
        double min = -1000000;
        double sved = -10000;
        var randArr = new List<Direction>();
        foreach (var val in mylist)
        {
            if (val.value >= min)
            {
                min = val.value;
                returnVal = val;

              
            }
        }

        
        
        agent.Move(returnVal.direction);

        return returnVal.direction;
    }

    
    public Vector2 getClosestGhost(Vector2 yourLocation)
    {
        var Blinky = knowledge.GetGhost(GhostName.BLINKY).currentTile;
        var Inky = knowledge.GetGhost(GhostName.INKY).currentTile;
        var Pinky = knowledge.GetGhost(GhostName.PINKY).currentTile;
        var Sue = knowledge.GetGhost(GhostName.SUE).currentTile;
        var GhostArray = new List<Vector2>() {Blinky, Inky, Pinky, Sue};
        float smallestDist = float.MaxValue;
        Vector2 location = new Vector2();
        foreach (var ghost in GhostArray)
        {
            float dist = (ghost - yourLocation).magnitude;
            if (dist < smallestDist)
            {
                smallestDist = dist;
                location = ghost;
            }
        }

        return location;
    }

    public bool isAnyGhostEdible()
    {
        return knowledge.GetGhost(GhostName.BLINKY).IsEdible() ||
               knowledge.GetGhost(GhostName.INKY).IsEdible() ||
               knowledge.GetGhost(GhostName.PINKY).IsEdible() ||
               knowledge.GetGhost(GhostName.SUE).IsEdible();
    }

    /// <summary>
    /// Returns the possible moves for the current tile, excluding those supplied as parameters.
    /// </summary>
    /// <returns>A randomly chosen possible direction, or NONE if none remains.</returns>
    /// <param name="exceptDirection">Directions to exclude.</param>
    Direction GetRandomPossibleDirection(params Direction[] exceptDirection)
    {
        List<Direction> possibleMoves = map.GetPossibleMoves();

        foreach (var d in exceptDirection)
            possibleMoves.Remove(d);

        if (possibleMoves.Count > 0)
            return possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)];

        return Direction.NONE;
    }

    private bool GenerateQMatrixDefault()
    {
        BinaryWriter bw;
        //create the file
        try
        {
            bw = new BinaryWriter(new FileStream("QData_Default", FileMode.Create));
        }
        catch (IOException e)
        {
            return true;
        }
        //writing into the file
        try
        {
            double k = 0;
            for (var i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bw.Write(k);
                }
            }
        }
        catch (IOException e)
        {
            return true;
        }
        bw.Close();
        return false;
    }

    private double[,] ReadQData()
    {
        BinaryReader br;
        try
        {
            br = new BinaryReader(new FileStream("QData", FileMode.Open));
        }
        catch (IOException e)
        {
            return null;
        }

        double[,] returnMatrix = new double[width, height];
        try
        {
            double d;
            for (var i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    d = br.ReadDouble();
                    returnMatrix[i, j] = d;
                    Debug.Log(d);
                }
            }
        }
        catch (IOException e)
        {
            Debug.Log("oh no");
            return null;
        }

        br.Close();
        return returnMatrix;
    }

    private bool WriteQMatrix(Dictionary<Situation,double> good)
    {
        BinaryWriter bw;
        string realp = @"D:\paci\Q-Learning\Assets\Assignment\savedQ.txt";
        
         using (StreamWriter sw = File.CreateText(realp)) 
         {
            foreach (var variable in good.Keys)
            {
                var tst = getString(good, variable);
                sw.WriteLine(tst);
                Debug.Log(tst);
            }
        
        
         }

        return false;
    }

    private Dictionary<Situation, double> read()
    {
        string realp = @"D:\paci\Q-Learning\Assets\Assignment\savedQ.txt";

        using (StreamReader sr = new StreamReader(realp))
        {
            string ln;
            while ((ln = sr.ReadLine()) != null)
            {

                char[] arr = ln.ToCharArray();
                foreach (var ch in arr)
                {
                    //switch(ch)
                    
                }
            }
            sr.Close();
        }

        return null;
    }

    private static StringBuilder getString(Dictionary<Situation, double> good, Situation variable)
    {
        StringBuilder tst = new StringBuilder();
        foreach (var var in variable.situation)
        {
            tst.Append("="+var.ToString());
        }

        tst.Append("#");
        tst.Append(variable.direction);
        tst.Append("+");
        tst.Append(good[variable]);
        return tst;
    }
}



public class RTiles
{
    

    public TileRep tilerep = TileRep.None;

    public RTiles(TileRep rep)
    {
        this.tilerep = rep;
    }
}

public class DirDouble
{


    public Direction dir;
    public double val;

    public DirDouble(Direction direction,double val)
    {
        this.dir = direction;
        this.val = val;
    }
}

public enum TileRep
{
    Wall = 0,
    None = 1,
    Ghost = 2,
    EdGhost = 3,
    Pill = 4,
    Pallet = 5
}

public static class ResetUpdated
{
    public static bool OpenReset = false;
}



public class Situation
{
    public TileRep[] situation = new TileRep[8];
    public Direction direction;
    public double value = 0;
    
    public Situation(TileRep[] situation, Direction direction)
    {
        this.direction = direction;
        this.situation = situation;
        
    }

   
    public bool EqualsSit(Situation other)
    {
        for (int i = 0; i < other.situation.Length; i++)
        {
            if (situation[i] != other.situation[i]) return false;
        }
        
        return true;  
    }
    
    public bool Equals(Situation other)
    {
        for (int i = 0; i < other.situation.Length; i++)
        {
            if (situation[i] != other.situation[i]) return false;
        }
        
        return direction == other.direction;  
    }

    public override bool Equals(object obj)
    {
        return obj.GetType() == GetType() && Equals((Situation)obj);
       
    }

    

    public override int GetHashCode()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var var in situation)
        {
            sb.Append(var.ToString());
        }
        sb.Append(direction.ToString());
        return sb.ToString().GetHashCode();
   
    }

}
