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
    [HideInInspector] public Vector2 spawnPosition;
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public bool isFacingLeft;
    [HideInInspector] public bool isCollidingWithGravityFlip;

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
