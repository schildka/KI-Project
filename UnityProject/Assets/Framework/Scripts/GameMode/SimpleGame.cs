using UnityEngine;

public class SimpleGame : GameMode
{
    [Header("Points")]

    [SerializeField]
    int PointsPerLevel = 1000;
    [SerializeField]
    int PointsPerPill = 1;
    [SerializeField]
    int PointsPerCherry = 10;
    [SerializeField]
    int PointsPerGhostEaten = 500;
    [SerializeField]
    int GhostsEdibleTime = 10;

    protected override void OnGhostEncounter(MsPacMan pacMan, Ghost ghost)
    {
        if (ghost.IsEdible())
        {
            GameData.score += PointsPerGhostEaten;
            ResetGhost(ghost);
        }
        else
        {
            GameData.lives--;
            if (GameData.lives == 0)
            {
                ResetGame();
            }
            else
            {
                ResetAgents();
            }
        }
    }

    protected override void OnPickup(MsPacMan pacMan, PickupItem pickup)
    {
        switch (pickup.type)
        {
            case PickupType.PILL:
                GameData.score += PointsPerPill;
                pillsEaten++;
                break;
            case PickupType.POWER_PELLET:
                MakeGhostsEdible();
                break;
            case PickupType.CHERRY:
                GameData.score += PointsPerCherry;
                break;
        }

        Destroy(pickup.gameObject);

        if (pillsEaten == numberOfPills)
        {
            GameData.level++;
            GameData.score += PointsPerLevel;
            ReloadScene();
        }
    }

    void MakeGhostsEdible()
    {
        foreach (Ghost g in ghosts.Values)
        {
            g.SetEdible(GhostsEdibleTime);
        }
    }
}
