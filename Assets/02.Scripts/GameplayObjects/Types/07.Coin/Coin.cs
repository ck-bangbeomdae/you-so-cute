using UnityEngine;

public class Coin : MonoBehaviour, ICollisionable
{
    // TODO : 코인 애니메이션 재생

    public void OnCollision(Player player)
    {
        // TODO : 코인 획득 효과음 재생

        GameplayManager.Instance.CoinCount++;
        Destroy(gameObject);
    }
}
