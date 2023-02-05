using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleTV : Interactible
{

    [SerializeField] private CameraSequence onFinishCameraSequence;
    protected override void Behaviour(PlayerActionData playerData)
    {
        if (playerData.State != PlayerStates.AIRBORNE) return;
        if (!CameraManager.Instance.sequences.Exists(seq => seq == onFinishCameraSequence)) return;


        var camIndex = CameraManager.Instance.sequences.FindIndex(0, CameraManager.Instance.sequences.Count - 1, seq => seq == onFinishCameraSequence);
        CameraManager.Instance.ActivateCamera(camIndex);
    }
}
