using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : Interactible
{
    [SerializeField] private ParticleSystem fireFX;

    [SerializeField] GameObject collectibleReward;
    [SerializeField] AudioSource fireAudioSource;
    [SerializeField] AudioSource fireAlarmAudioSource;
    [SerializeField] AudioClip fireOutOneshot;

    public LadyNPCController Lady;
    protected override void Behaviour(PlayerActionData playerData)
    {
        fireFX.Stop();
        fireAudioSource.Stop();
        fireAlarmAudioSource.Stop();
        fireAudioSource.PlayOneShot(fireOutOneshot);
        collectibleReward.SetActive(true);
        Lady.UnPanic();
    }

    private void Start()
    {
        fireFX.Play();
        collectibleReward.SetActive(false);
    }
}
