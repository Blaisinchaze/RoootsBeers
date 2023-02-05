using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleTV : Interactible
{

    [SerializeField] private CameraSequence onFinishCameraSequence;
    [SerializeField] private ManNPCController manNpcController;
    [SerializeField] private GameObject staticTexture;
    protected override void Behaviour(PlayerActionData playerData)
    {
        if (playerData.State != PlayerStates.AIRBORNE) return;

        staticTexture.SetActive(true);
        CameraManager.Instance.ActivateSequence(onFinishCameraSequence, WakeUpDad);
    }

    private void Start()
    {
        staticTexture.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4)) CameraManager.Instance.ActivateSequence(onFinishCameraSequence, WakeUpDad);

    }

    private void WakeUpDad()
    {
        manNpcController.WakeUp();
    }
}
