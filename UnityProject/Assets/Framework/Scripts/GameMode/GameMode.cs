using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameMode : MonoBehaviour
{
    [Header("Agent Prefabs")]
    [SerializeField]
    GameObject msPacMan = null;
    [SerializeField]
    GameObject blinky = null;
    [SerializeField]
    GameObject pinky = null;
    [SerializeField]
    GameObject inky = null;
    [SerializeField]
    GameObject sue = null;

    public MsPacMan MsPacMan
    {
        get { return pacman; }
    }

    [Header("Pickup Prefabs")]
    [SerializeField]
    GameObject pill = null;
    [SerializeField]
    GameObject powerPellet = null;

    Maze maze;
    GameObject agents;
    GameObject pickups;
    LayerMask GhostLayer;

    protected MsPacMan pacman;
    protected Dictionary<GhostName, Ghost> ghosts = new Dictionary<GhostName, Ghost>();
    protected int numberOfPills;
    protected int pillsEaten;

    public void OnPickupCollision(Agent agent, PickupItem item)
    {
        MsPacMan pacMan = agent.GetComponent<MsPacMan>();
        OnPickup(pacMan, item);
    }

    public void OnAgentCollision(Agent agent, Collision2D collision)
    {
        if (collision.gameObject.layer == GhostLayer)
        {
            OnGhostEncounter(agent.GetComponent<MsPacMan>(), collision.gameObject.GetComponent<Ghost>());
        }
    }

    public Ghost GetGhost(GhostName name)
    {
        return ghosts[name];
    }

    protected abstract void OnGhostEncounter(MsPacMan pacMan, Ghost ghost);

    protected abstract void OnPickup(MsPacMan pacMan, PickupItem item);

    protected void ResetPacMan()
    {
        pacman.Reset(maze.msPacManSpawn);
    }

    protected void ResetGhost(Ghost ghost)
    {
        ghost.Reset(maze.ghostSpawn);
    }

    protected void ResetGhosts()
    {
        foreach (Ghost g in ghosts.Values)
        {
            ResetGhost(g);
        }
    }

    protected void ResetGame()
    {
        GameData.Reset();
        ReloadScene();
    }

    protected void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    protected void ResetAgents()
    {
        ResetGhosts();
        ResetPacMan();
    }

    void Awake()
    {
        GhostLayer = LayerMask.NameToLayer("Ghosts");
        maze = GameObject.Find("Maze").GetComponent<Maze>();
        agents = new GameObject("Agents");
    }

    void Start()
    {
        LoadLevel(GameData.level);

        InstantiateGhosts();
        InstantiatePlayer();
    }

    void LoadLevel(int level)
    {
        // Find the maze for the current or first available lower level
        TextAsset levelMaze = null;
        do
        {
            levelMaze = Resources.Load<TextAsset>("maze-" + level);
        } while (levelMaze == null && level-- > 0);

        maze.LoadMaze(levelMaze);

        InstantiatePowerUps();
    }

    void InstantiatePlayer()
    {
        pacman = Instantiate(msPacMan, maze.msPacManSpawn, Quaternion.identity, agents.transform).GetComponent<MsPacMan>();
    }

    void InstantiateGhosts()
    {
        ghosts.Add(GhostName.BLINKY, Instantiate(blinky, maze.ghostSpawn, Quaternion.identity, agents.transform).GetComponent<Ghost>());
        ghosts.Add(GhostName.PINKY, Instantiate(pinky, maze.ghostSpawn, Quaternion.identity, agents.transform).GetComponent<Ghost>());
        ghosts.Add(GhostName.INKY, Instantiate(inky, maze.ghostSpawn, Quaternion.identity, agents.transform).GetComponent<Ghost>());
        ghosts.Add(GhostName.SUE, Instantiate(sue, maze.ghostSpawn, Quaternion.identity, agents.transform).GetComponent<Ghost>());
    }

    void InstantiatePowerUps()
    {
        pickups = new GameObject("PowerUps");

        foreach (PickupType type in maze.pickupItems.Keys)
        {
            foreach (Vector2 position in maze.pickupItems[type])
            {
                GameObject prefab;
                switch (type)
                {
                    case PickupType.PILL:
                        prefab = pill;
                        numberOfPills++;
                        break;
                    case PickupType.POWER_PELLET:
                        prefab = powerPellet;
                        break;
                    default:
                        throw new UnityException("Not Implemented");
                }
                Instantiate(prefab, position, Quaternion.identity, pickups.transform);
            }
        }
    }
}
