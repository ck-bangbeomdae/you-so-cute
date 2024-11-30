using UnityEngine;

public class Portal : BasePlayerSpawnpoint, ICollisionable
{
    [SerializeField] private CommonEnums.CardinalDirection cardinalDirection;
    [SerializeField] private PathType pathType;

    public void OnCollision(Player player)
    {
        // 플레이어 스폰포인트 저장
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

        // 게임 진행사항 업데이트
        GameplayManager.Instance.CurrentProgressPortalCount += (int)pathType;

        UIManager.Instance.CloseManual();
    }

    private enum PathType
    {
        Backward = -1,
        Detour = 0,
        Forward = 1
    }
}
