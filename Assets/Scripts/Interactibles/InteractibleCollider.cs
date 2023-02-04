using UnityEngine;
using UnityEngine.Events;

public class InteractibleCollider : MonoBehaviour
{
    public PlayerEvent OnPlayerEnterTrigger;
    private MainCharacterController player;
    public InteractibleTrigger triggerRequirement;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerAction")) return;
        if (!other.TryGetComponent<PlayerActionData>(out var playerActionData)) return;

        if (triggerRequirement == InteractibleTrigger.Proximity)
            Hit();
        switch (playerActionData.State)
        {
            case PlayerStates.GROUNDED:
                break;
            case PlayerStates.AIRBORNE:
                if (triggerRequirement == InteractibleTrigger.HitByPlayer)
                    Hit();
                break;
            case PlayerStates.AIMING:
                break;
            default:
                break;
        }
        void Hit()
        {
            OnPlayerEnterTrigger?.Invoke(playerActionData);

        }

    }
}