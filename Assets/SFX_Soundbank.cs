using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Soundbank : MonoBehaviour
{
    public static SFX_Soundbank instance;

    public List<AudioClip> FizzSFX;
    public List<AudioClip> BottleOpening;
    public List<AudioClip> CarpetFootsteps;
    public List<AudioClip> WoodFootsteps;
    public List<AudioClip> BottleClinks;
    public List<AudioClip> SloshingLiquid;
    public List<AudioClip> CollectPickup;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }
}
