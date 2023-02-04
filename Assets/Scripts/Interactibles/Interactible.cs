using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InteractibleTrigger
{
    SprayedByPlayer = 1,
    HitByPlayer,
    Proximity
}

public abstract class Interactible : MonoBehaviour
{
    [Header("Interactible Components")]
    [SerializeField] private Animation idleAnimation;
    [SerializeField] private Animation interactibleAnimation;
    [SerializeField] private Animation TriggeredAnimation;

    [SerializeField] private InteractibleCollider playerDetectionCollider;

    [SerializeField] protected InteractibleTrigger interactTrigger;
    public InteractibleTrigger Trigger 
    { 
        get => interactTrigger; 
    }

    protected virtual void Awake()
    {
        playerDetectionCollider.OnPlayerEnterTrigger.AddListener(Behaviour);
        playerDetectionCollider.triggerRequirement = interactTrigger;
    }
    private void OnDestroy()
    {
        playerDetectionCollider.OnPlayerEnterTrigger.RemoveListener(Behaviour);
    }
    protected abstract void Behaviour(PlayerActionData playerData);

}
