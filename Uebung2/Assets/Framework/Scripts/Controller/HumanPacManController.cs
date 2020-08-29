using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPacManController : AgentController<MsPacMan> {
    
    public override void OnDecisionRequired()
    {
    }

    public override void OnTileReached()
    {
    }

	void Update () {
        if (Input.GetKey(KeyCode.LeftArrow))
            agent.Move(Direction.LEFT);
        if (Input.GetKey(KeyCode.RightArrow))
            agent.Move(Direction.RIGHT);
        if (Input.GetKey(KeyCode.UpArrow))
            agent.Move(Direction.UP);
        if (Input.GetKey(KeyCode.DownArrow))
            agent.Move(Direction.DOWN);
	}
}
