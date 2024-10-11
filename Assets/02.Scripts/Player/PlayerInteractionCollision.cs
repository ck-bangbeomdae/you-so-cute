using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionCollision : MonoBehaviour
{
    // 컴포넌트
    private Player player;

    // 상호작용 가능한 오브젝트 리스트
    private readonly List<GameObject> interactablesInRange = new List<GameObject>();

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactablesInRange.Remove(other.gameObject);
            if (interactable == player.closestInteractable)
            {
                player.closestInteractable = null;
            }
        }
    }

    private void Update()
    {
        FindNewClosestInteractable();
    }

    private void FindNewClosestInteractable()
    {
        float closestDistance = float.MaxValue;
        IInteractable closest = null;

        foreach (GameObject interactable in interactablesInRange)
        {
            float distance = Vector2.Distance(transform.position, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable.GetComponent<IInteractable>();
            }
        }

        player.closestInteractable = closest;
    }
}
