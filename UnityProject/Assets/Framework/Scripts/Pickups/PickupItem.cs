using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PickupItem : MonoBehaviour
{
    public PickupType type;

    GameMode game;

    void Start()
    {
        game = GameObject.Find("GameMode").GetComponent<GameMode>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        game.OnPickupCollision(collision.gameObject.GetComponent<Agent>(), this);
    }
}

public enum PickupType
{
    PILL,
    CHERRY,
    POWER_PELLET
}