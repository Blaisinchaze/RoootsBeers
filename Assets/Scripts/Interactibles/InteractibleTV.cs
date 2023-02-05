using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleTV : Interactible
{

    [SerializeField] private CameraSequence onFinishCameraSequence;
    [SerializeField] private ManNPCController manNpcController;
    [SerializeField] private GameObject staticTexture;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tvOnClip;
    [SerializeField] private AudioClip tvStaticClip;
    protected override void Behaviour(PlayerActionData playerData)
    {
        if (playerData.State != PlayerStates.AIRBORNE) return;
        audioSource.PlayOneShot(tvOnClip);
        staticTexture.SetActive(true);
        audioSource.clip = tvStaticClip;
        audioSource.PlayDelayed(0.3f);
        audioSource.loop = true;
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
