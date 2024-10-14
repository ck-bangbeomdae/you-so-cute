using UnityEngine;

public class BasePlayerSpawnpoint : MonoBehaviour
{
    [SerializeField] protected PlayerSpawnpoint playerSpawnpoint;
}

[System.Serializable]
public struct PlayerSpawnpoint
{
    public SceneTransition sceneTransition;
    [HideInInspector] public Vector2 spawnPosition;
    [HideInInspector] public Vector2 velocity;
    public bool isGravityFlipped;
    public bool isFacingLeft;
    public bool isCollidingWithGravityFlip;
}
