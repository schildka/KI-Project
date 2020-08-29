using UnityEngine;

/// <summary>
/// Sensors provide the agent program with information about the environment.
/// </summary>
public class Sensor : MonoBehaviour
{
    protected Agent Agent
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        Agent = GetComponent<Agent>();
    }
}
