using UnityEngine;

public class Portal : BasePlayerSpawnpoint, ICollisionable
{
    [SerializeField] private Direction direction;

    public void OnCollision(Player player)
    {
        Vector2 newPosition = player.transform.position;

        switch (direction)
        {
            case Direction.North:
                newPosition.y = -10f;
                break;
            case Direction.East:
                newPosition.x = -18f;
                break;
            case Direction.South:
                newPosition.y = 10f;
                break;
            case Direction.West:
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

    private enum Direction
    {
        North,
        East,
        South,
        West
    }
}
