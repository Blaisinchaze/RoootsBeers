using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : Interactible
{
    [SerializeField] private ParticleSystem fireFX;

    [SerializeField] GameObject collectibleReward;
    protected override void Behaviour(PlayerActionData playerData)
    {
        fireFX.Stop();
        collectibleReward.SetActive(true);
    }

    private void Start()
    {
        fireFX.Play();
        collectibleReward.SetActive(false);
    }
}
