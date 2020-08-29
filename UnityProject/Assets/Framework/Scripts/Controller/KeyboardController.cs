using UnityEngine;

[RequireComponent(typeof(MsPacMan))]
public class KeyboardController : AgentController<MsPacMan>
{

    
    void Update()
    {
        //TODO: implement logic
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            agent.Move(Direction.DOWN);

        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            agent.Move(Direction.UP);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            agent.Move(Direction.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            agent.Move(Direction.LEFT);
        }

    }
    
    

    
    public override void OnDecisionRequired()
    {
        // Nothing to do
    }

    public override void OnTileReached()
    {
        // Nothing to do
    }
    

}
