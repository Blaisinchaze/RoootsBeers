using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleTV : Interactible
{

    [SerializeField] private CameraSequence onFinishCameraSequence;
    protected override void Behaviour(PlayerActionData playerData)
    {
        if (playerData.State != PlayerStates.AIRBORNE) return;
        CameraManager.Instance.ActivateSequence(onFinishCameraSequence, WakeUpDad);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4)) CameraManager.Instance.ActivateSequence(onFinishCameraSequence);

    }

    private void WakeUpDad()
    {

    }
}
