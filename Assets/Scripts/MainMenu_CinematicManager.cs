using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_CinematicManager : MonoBehaviour
{
    float timer;
    public float timeToSwitch;

    // Update is called once per frame
    void Update()
    {
        if (CameraManager.Instance.activeSequence != null) return;


        if (timer <= 0)
        {
            CameraManager.Instance.ActivateSequence(0);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
