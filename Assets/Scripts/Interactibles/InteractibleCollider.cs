using UnityEngine;
using UnityEngine.Events;

public class InteractibleCollider : MonoBehaviour
{
    public PlayerEvent OnPlayerEnterTrigger = new();
    private MainCharacterController player;
    public InteractibleTrigger triggerRequirement;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!other.TryGetComponent<PlayerActionCollider>(out var actionCol))
             return;
            if (triggerRequirement == InteractibleTrigger.Proximity)
        {
            Hit();
            return;
        }
            switch (actionCol.actionData.State)
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
            OnPlayerEnterTrigger?.Invoke(actionCol.actionData);

        }

    }
}