using UnityEngine;

public class GravityFlip : MonoBehaviour, ICollisionable
{
    public void OnCollision(Player player)
    {
        if (player.isCollidingWithGravityFlip)
        {
            return;
        }

        player.rb2d.velocity /= 3f;
        player.GravityFlip();
    }
}
