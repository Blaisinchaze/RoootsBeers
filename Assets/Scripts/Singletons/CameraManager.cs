using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public CinemachineVirtualCamera vCam;
    public List<CameraSequence> sequences;

    public CameraSequence activeSequence;

    public Transform lookPoint;
    public Transform camPoint;

    public float timer;

    public void ActivateCamera(int idx) 
    {
        if (activeSequence != null || idx >= sequences.Count || sequences[idx] == null) return;

        //make the new cam take priority
        activeSequence = sequences[idx];
        vCam.Priority = 100;

        //This just needs to always be true... should be in start
        vCam.LookAt = lookPoint;
        vCam.Follow = camPoint;

        timer = 0;
    }

    public void Update()
    {
        if (activeSequence != null)
        {
            timer += Time.deltaTime;
            if (timer >= activeSequence.duration + activeSequence.timeBeforeExit) { DeactivateCamera(); return; }

            //update lookat point;
            //update camera position;

            lookPoint.position = activeSequence.aimingPath.EvaluatePositionAtUnit(
                Mathf.MoveTowards(0, 1, timer / activeSequence.duration),
                CinemachinePathBase.PositionUnits.Normalized);

            camPoint.position = activeSequence.travelPath.EvaluatePositionAtUnit(
                Mathf.MoveTowards(0, 1, timer / activeSequence.duration),
                CinemachinePathBase.PositionUnits.Normalized);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateCamera(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateCamera(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateCamera(2);
        
    }

    private void DeactivateCamera()
    {
        timer = -1;
        activeSequence = null;

        //make the current cam reset priority
        vCam.Priority = -1;
    }
}