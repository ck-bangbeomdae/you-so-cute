using System;
using UnityEngine;

public class BasePlayerSpawnpoint : MonoBehaviour
{
    [SerializeField] protected PlayerSpawnpoint playerSpawnpoint;
}

[System.Serializable]
public struct PlayerSpawnpoint
{
    public SceneTransition sceneTransition;
    public bool isGravityFlipped;
    public Vector2 spawnPosition;
    public Vector2 velocity;
    public bool isFacingLeft;
    public bool isCollidingWithGravityFlip;

    public override int GetHashCode()
    {
        return HashCode.Combine
        (
            sceneTransition,
            spawnPosition,
            velocity,
            isGravityFlipped,
            isFacingLeft,
            isCollidingWithGravityFlip
        );
    }
}
