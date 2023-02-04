using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InteractibleTrigger
{
    SprayedByPlayer = 1,
    LaunchedInto
}

public abstract class Interactible : MonoBehaviour
{
    [SerializeField] private Animation idleAnimation;
    [SerializeField] private Animation interactibleAnimation;
    [SerializeField] private Animation TriggeredAnimation;

    [SerializeField] private InteractibleCollider playerDetectionCollider;

    private InteractibleTrigger interactTrigger;
    public InteractibleTrigger Trigger 
    { 
        get => interactTrigger; 
        set => interactTrigger = value; 
    }

    public PlayerEvent OnTrigger;

    protected virtual void Awake()
    {
        playerDetectionCollider.OnPlayerEnterTrigger.AddListener(Behaviour);
    }

    protected void Behaviour(PlayerActionData playerData)
    {

    }
}
