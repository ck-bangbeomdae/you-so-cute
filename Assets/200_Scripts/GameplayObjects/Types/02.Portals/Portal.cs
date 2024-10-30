using UnityEngine;

public class Portal : BasePlayerSpawnpoint, ICollisionable
{
    [SerializeField] private CommonEnums.CardinalDirection cardinalDirection;

    public void OnCollision(Player player)
    {
        Vector2 newPosition = player.transform.position;

        switch (cardinalDirection)
        {
            case CommonEnums.CardinalDirection.North:
                newPosition.y = -10f;
                break;
            case CommonEnums.CardinalDirection.East:
                newPosition.x = -18f;
                break;
            case CommonEnums.CardinalDirection.South:
                newPosition.y = 10f;
                break;
            case CommonEnums.CardinalDirection.West:
                newPosition.x = 18f;
                break;
        }

        playerSpawnpoint.spawnPosition = newPosition;
        playerSpawnpoint.velocity = player.rb2d.velocity;
        playerSpawnpoint.isGravityFlipped = player.IsGravityFlipped;
        playerSpawnpoint.isFacingLeft = player.IsFacingLeft;
        playerSpawnpoint.isCollidingWithGravityFlip = player.isCollidingWithGravityFlip;

        TransitionManager.Instance.LoadSceneWithPlayer(playerSpawnpoint);
    }
}
