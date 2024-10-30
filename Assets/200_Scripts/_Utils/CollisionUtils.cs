using UnityEngine;

public static class CollisionUtils
{
    public static bool IsCollisionFromTopOrBottom(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.y) > 0.5f)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsCollisionFromSide(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsGroundLayer(LayerMask groundLayer, int layer)
    {
        return (groundLayer.value & (1 << layer)) != 0;
    }
}
