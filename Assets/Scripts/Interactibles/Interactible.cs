using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InteractibleTrigger
{
    SprayedByPlayer = 1,
    LaunchedInto
}

public class Interactible : MonoBehaviour
{
    [SerializeField] private Animation idleAnimation;
    [SerializeField] private Animation interactibleAnimation;
    [SerializeField] private Animation TriggeredAnimation;

    [SerializeField] private Collider playerDetectionCollider;

    private InteractibleTrigger interactTrigger;
    public InteractibleTrigger Trigger { get => interactTrigger; set => interactTrigger = value; }

    public UnityEvent OnTriggered;
}