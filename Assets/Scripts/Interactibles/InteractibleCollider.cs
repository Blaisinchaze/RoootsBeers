using UnityEngine;

public class InteractibleCollider : MonoBehaviour
{
    public PlayerEvent OnPlayerEnterTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerAction")) return;

        if (other.TryGetComponent<PlayerActionData>(out var playerActionData))
        {
            OnPlayerEnterTrigger?.Invoke(playerActionData);
        }
    }
}