using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public GameObject worldPart;

    public GameObject[,] world;

    public GameObject player;

    public GameObject wumpus;

    public GameObject hole;

    public GameObject food;

    public Text points;

    public Text smell;

    public Text wind;

    public int x;

    public int y;

    private int _points = 0;

    private string _pointText = "Points: ";

    private string _windText = "Wind: ";

    private string _smellText = "Smell: ";

    private Vector2 _playerPosition;

    private bool _loaded = true;

    private bool gameOver = false;

    private Dictionary<Vector2, PotWump> _wumpusKnowledgebase =
        new Dictionary<Vector2, PotWump>();

    Dictionary<Vector2, int> _smellCount =
        new Dictionary<Vector2, int>();

    Dictionary<Vector2, int> _windCounter =
        new Dictionary<Vector2, int>();

    Dictionary<Vector2, int> Knowledge =
        new Dictionary<Vector2, int>();

    Dictionary<Vector2, int> DangerZone =
        new Dictionary<Vector2, int>();

    private void Start()
    {
        world = new GameObject[y, x];

        for (var i = y - 1; i >= 0; i--)
        {
            for (var j = x - 1; j >= 0; j--)
            {
                world[i, j] = Instantiate(worldPart, new Vector3(i, j, 0), Quaternion.identity);
            }
        }

        player = Instantiate(player, new Vector3(0, y - 1, -1), Quaternion.identity);
        _playerPosition = new Vector2(0, y - 1);
        DangerZone.Add(new Vector2(0, y - 2), 0);
        DangerZone.Add(new Vector2(1, y - 1), 0);
        Knowledge.Add(new Vector2(0, y - 1), 1);

        var pX = Random.Range(x - 2, x);
        var pY = Random.Range(0, y);

        wumpus = Instantiate(wumpus, new Vector3(pX, pY, -1), new Quaternion(0.0f, 180.0f, 0.0f, 0.0f));
        world[pX, pY] = wumpus;
        initSmell(new Vector2(pX, pY));


        for (var i = 0; i < 3; i++)
        {
            do
            {
                pX = Random.Range(0, x);
                pY = Random.Range(0, y - 1);
            } while (!world[pX, pY].tag.Equals("Block"));

            world[pX, pY] = Instantiate(hole, new Vector3(pX, pY, -1), Quaternion.identity);
            initWind(new Vector2(pX, pY));
        }


        do
        {
            pX = Random.Range(0, x);
            pY = Random.Range(0, y - 1);
        } while (!world[pX, pY].tag.Equals("Block"));

        world[pX, pY] = Instantiate(food, new Vector3(pX, pY, -1), Quaternion.identity);

        sensor();

        StartCoroutine(MyFunc(0.5f));
    }

    private void initWind(Vector2 loc)
    {
        var locDir = loc + Vector2.up;
        if (_windCounter.ContainsKey(locDir))
        {
            _windCounter[locDir]++;
        }
        else
        {
            _windCounter.Add(locDir, 1);
        }

        locDir = loc + Vector2.down;
        if (_windCounter.ContainsKey(locDir))
        {
            _windCounter[locDir]++;
        }
        else
        {
            _windCounter.Add(locDir, 1);
        }

        locDir = loc + Vector2.right;
        if (_windCounter.ContainsKey(locDir))
        {
            _windCounter[locDir]++;
        }
        else
        {
            _windCounter.Add(locDir, 1);
        }

        locDir = loc + Vector2.left;
        if (_windCounter.ContainsKey(locDir))
        {
            _windCounter[locDir]++;
        }
        else
        {
            _windCounter.Add(locDir, 1);
        }
    }

    private void initSmell(Vector2 loc)
    {
        var locDir = loc + Vector2.up;
        if (_smellCount.ContainsKey(locDir))
        {
            _smellCount[locDir]++;
        }
        else
        {
            _smellCount.Add(locDir, 1);
        }

        locDir = loc + Vector2.down;
        if (_smellCount.ContainsKey(locDir))
        {
            _smellCount[locDir]++;
        }
        else
        {
            _smellCount.Add(locDir, 1);
        }

        locDir = loc + Vector2.right;
        if (_smellCount.ContainsKey(locDir))
        {
            _smellCount[locDir]++;
        }
        else
        {
            _smellCount.Add(locDir, 1);
        }

        locDir = loc + Vector2.left;
        if (_smellCount.ContainsKey(locDir))
        {
            _smellCount[locDir]++;
        }
        else
        {
            _smellCount.Add(locDir, 1);
        }
    }


    private void Update()
    {
        if (gameOver) return;
        //KeyboardController();
    }

    public IEnumerator MyFunc(float everySeconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(everySeconds);
            if(player is null)yield break;
            int min = 1000;
            var vec = new Vector2();
            foreach (var VARIABLE in DangerZone)
            {
                if (VARIABLE.Value < min)
                {
                    vec = VARIABLE.Key;
                    min = VARIABLE.Value;
                }
            }
            
            if (_wumpusKnowledgebase.ContainsKey(_playerPosition))
            {
                _wumpusKnowledgebase.Remove(_playerPosition);
                
                Debug.Log("wumpus Removed");
            }
            
            if (EndGame()) yield break;

            foreach (var entry in _wumpusKnowledgebase)
            {
                Debug.Log(entry.Value.location);
                
            }
            
            

            var position = new Vector3(vec.x, vec.y, -1);

            player.transform.position = position;
            _playerPosition = vec;
            move(new Vector2(0.0f, 0.0f));

            Knowledge.Add(vec, 1);
            DangerZone.Remove(vec);
            int addValue = 0;
            
           
            
            if (_smellCount.ContainsKey(_playerPosition))
            {
                addValue += _smellCount[vec];

                foreach (var d in GetPossibleDirections())
                {
                    var loc = _playerPosition + d;
                    
                    if (!Knowledge.ContainsKey(loc))
                    {
                        PotWump wum = new PotWump(d);
                        if(!_wumpusKnowledgebase.ContainsKey(loc))_wumpusKnowledgebase.Add(loc,wum);
                        //else if (EndGame()) yield break;
                        Debug.Log("wumpus added");
                        var oldLoc = _playerPosition;
                        _playerPosition = loc;
                        bool destroy = false;
                        foreach (var e in GetPossibleDirections())
                        {
                            if (Knowledge.ContainsKey(loc + e) && loc+e != vec) destroy = true;
                            wum.PotSmells.Add(loc+e);
                        }
                        _playerPosition = oldLoc;
                        if (destroy)
                        {
                            _wumpusKnowledgebase.Remove(loc);
                            Debug.Log("wumpus Removed");
                        }
                    }                   
                }
            }
            

            if (_windCounter.ContainsKey(vec)) addValue += _windCounter[vec];


            List<Vector2> VecList = GetPossibleDirections();
            foreach (var dir in VecList)
            {
                if (!Knowledge.ContainsKey(vec + dir))
                {
                    if (DangerZone.ContainsKey(vec + dir))
                    {
                        if(addValue == 0)DangerZone[vec + dir] = 0;
                        else DangerZone[vec + dir] += addValue;
                    }
                    else DangerZone.Add(vec + dir, addValue);
                }
            }
            
        }
    }

    private bool EndGame()
    {
        if (_wumpusKnowledgebase.Count == 1)
        {
            var locationWump = new Vector2();
            foreach (var curr in _wumpusKnowledgebase)
            {
                locationWump = curr.Key;
            }

            foreach (var locWin in Knowledge)
            {
                if ((locWin.Key - locationWump).magnitude == 1.0)
                {
                    player.transform.position = locWin.Key;
                    Debug.Log("win");
                    shoot(locationWump - locWin.Key);
                    return true;
                }
            }
        }

        return false;
    }

    private Vector2 GetPossibleRandomDirection()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0:
                if (player.transform.position.y < y - 1) return Vector2.up;
                else return GetPossibleRandomDirection();
                break;
            case 1:
                if (player.transform.position.y > 0) return Vector2.down;
                else return GetPossibleRandomDirection();
                break;
            case 2:
                if (player.transform.position.x > 0) return Vector2.left;
                else return GetPossibleRandomDirection();
                break;
            case 3:
                if (player.transform.position.x < x - 1) return Vector2.right;
                else return GetPossibleRandomDirection();
                break;
            default:
                return new Vector2();
        }

        return new Vector2();
    }

    private List<Vector2> GetPossibleDirections()
    {
        List<Vector2> retList = new List<Vector2>();
        if (_playerPosition.y < y - 1) retList.Add(Vector2.up);
        if (_playerPosition.y > 0) retList.Add(Vector2.down);
        if (_playerPosition.x < x - 1) retList.Add(Vector2.right);
        if (_playerPosition.x > 0) retList.Add(Vector2.left);
        return retList;
    }

    private void KeyboardController()
    {
        if (Input.GetKeyDown("up") && player.transform.position.y < y - 1)
        {
            var position = player.transform.position;
            position = new Vector3(position.x, position.y + 1, position.z);
            player.transform.position = position;
            move(Vector2.up);
        }

        if (Input.GetKeyDown("down") && player.transform.position.y > 0)
        {
            var position = player.transform.position;
            position = new Vector3(position.x, position.y - 1, position.z);
            player.transform.position = position;
            move(Vector2.down);
        }

        if (Input.GetKeyDown("right") && player.transform.position.x < x - 1)
        {
            var position = player.transform.position;
            position = new Vector3(position.x + 1, position.y, position.z);
            player.transform.position = position;
            move(Vector2.right);
        }

        if (Input.GetKeyDown("left") && player.transform.position.x > 0)
        {
            var position = player.transform.position;
            position = new Vector3(position.x - 1, position.y, position.z);
            player.transform.position = position;
            move(Vector2.left);
        }

        if (Input.GetKeyDown("w"))
        {
            shoot(Vector2.up);
        }

        if (Input.GetKeyDown("s"))
        {
            shoot(Vector2.down);
        }

        if (Input.GetKeyDown("d"))
        {
            shoot(Vector2.right);
        }

        if (Input.GetKeyDown("a"))
        {
            shoot(Vector2.left);
        }
    }


    private void shoot(Vector2 direction)
    {
        var shootLocation = _playerPosition + direction;

        try
        {
            if (world[(int) shootLocation.x, (int) shootLocation.y].tag.Equals("Wumpus") && _loaded)
            {
                AddPoint(1000);
                points.text = "Winner Winner Chicken Dinner";
                Destroy(world[(int) shootLocation.x, (int) shootLocation.y]);
                world[(int) shootLocation.x, (int) shootLocation.y] = worldPart;
                gameOver = true;
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.Log("Idiot");
        }

        _loaded = false;
    }

    private void move(Vector2 direction)
    {
        _playerPosition += direction;
        AddPoint(-1);

        sensor();

        if (_smellCount.ContainsKey(_playerPosition))
        {
            var i = _smellCount[_playerPosition];
            smell.text = _smellText + i;
        }
        else
        {
            smell.text = _smellText + 0;
        }

        if (_windCounter.ContainsKey(_playerPosition))
        {
            var i = _windCounter[_playerPosition];
            wind.text = _windText + i;
        }
        else
        {
            wind.text = _windText + 0;
        }


        switch (world[(int) _playerPosition.x, (int) _playerPosition.y].tag)
        {
            case "Wumpus":
                AddPoint(-1000);
                points.text = "Game Over";
                Destroy(player);
                gameOver = true;
                break;
            case "Hole":
                AddPoint(-1000);
                points.text = "Game Over";
                Destroy(player);
                gameOver = true;
                break;
            case "Food":
                AddPoint(1000);
                Destroy(world[(int) _playerPosition.x, (int) _playerPosition.y]);
                world[(int) _playerPosition.x, (int) _playerPosition.y] = worldPart;
                break;
            default:
                break;
        }
    }

    private void sensor()
    {
        if (_smellCount.ContainsKey(_playerPosition))
        {
            var i = _smellCount[_playerPosition];
            smell.text = _smellText + i;
        }
        else
        {
            smell.text = _smellText + 0;
        }

        if (_windCounter.ContainsKey(_playerPosition))
        {
            var i = _windCounter[_playerPosition];
            wind.text = _windText + i;
        }
        else
        {
            wind.text = _windText + 0;
        }
    }

    private void AddPoint(int point)
    {
        _points += point;
        points.text = _pointText + _points;
    }

    private void SetPoint(int point)
    {
        points.text = _pointText + point;
    }
    
}

public class PotWump
{

    public Vector2 location;
    public List<Vector2> PotSmells = new List<Vector2>();


    public List<Vector2> GetPossibleSmells()
    {
        return PotSmells;
    }
    

    public PotWump(Vector2  loc)
    {
        location = loc;
        
        
    }

}

