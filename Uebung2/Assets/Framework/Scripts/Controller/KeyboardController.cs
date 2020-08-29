using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MsPacMan))]
public class KeyboardController : AgentController<MsPacMan>
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && agent.currentMove != Direction.UP)
            agent.Move(Direction.UP);
        else if (Input.GetKeyDown(KeyCode.DownArrow) && agent.currentMove != Direction.DOWN)
            agent.Move(Direction.DOWN);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && agent.currentMove != Direction.LEFT)
            agent.Move(Direction.LEFT);
        else if (Input.GetKeyDown(KeyCode.RightArrow) && agent.currentMove != Direction.RIGHT)
            agent.Move(Direction.RIGHT);
    }

    public override void OnDecisionRequired()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTileReached()
    {
        //throw new System.NotImplementedException();
    }
}
