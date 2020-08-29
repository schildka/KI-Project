using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reflex agent controller with state.
/// 
/// A reflex agent with state can change its state depending on observed sensory input or actions.
/// This includes timers, counters, seen / unseen objects, ...
/// </summary>
[RequireComponent(typeof(MazeMap))]
[RequireComponent(typeof(MsPacMan))]
public class ReflexAgentWithState : AgentController<MsPacMan>
{
    MazeMap map;
    Eyes eyes;
    GameMode gameMode;

    protected override void Awake()
    {
        base.Awake();
        map = GetComponent<MazeMap>();
        eyes = GetComponent<Eyes>();
        gameMode = GameObject.Find("GameMode").GetComponent<GameMode>();
    }

    void Update()
    {
    }

    public override void OnDecisionRequired()
    {
        var directions = map.GetPossibleMoves();

        foreach (var dir in directions)
        {
            var k = eyes.Look(dir);
            if (k == null) continue;
            if (k.GetType() == typeof(VisualGhostPercept))
            {
                VisualGhostPercept Ghost = (VisualGhostPercept) k;
                if (!Ghost.isEdible)
                {
                    foreach (var tempdir in directions)
                    {
                        if (tempdir != dir)
                        {
                            agent.Move(tempdir);
                            return;
                        }
                    }

                    return;
                }
                else
                {
                    agent.Move(dir);
                    return;
                }
            }

            if (k.GetType() == typeof(VisualPickupPercept))
            {
                agent.Move(dir);
                return;
            }
        }

        Debug.Log("HI");
        agent.Move(DirectionExtensions.Random());
    }


    public override void OnTileReached()
    {
        OnDecisionRequired();


        // agent.Move(DirectionExtensions.Random());
    }
}