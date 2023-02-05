using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : NonPersistantSingleton<CameraManager>
{
    public CinemachineVirtualCamera vCam;
    public List<CameraSequence> sequences;

    public CameraSequence activeSequence;
    public delegate void OnSequenceComplete();
    private OnSequenceComplete onSequenceComplete;

    public Transform lookPoint;
    public Transform camPoint;

    public float timer;

    public void ActivateSequence(int idx) 
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

    public void ActivateSequence(CameraSequence seq, OnSequenceComplete callbackOnComplete = null)
    {
        if (seq == null) return;

        //make the new cam take priority
        activeSequence = seq;
        vCam.Priority = 100;

        //This just needs to always be true... should be in start
        vCam.LookAt = lookPoint;
        vCam.Follow = camPoint;

        if (callbackOnComplete is not null)
            onSequenceComplete = callbackOnComplete;
        timer = 0;
    }

    public void Update()
    {
        if (activeSequence != null)
        {
            timer += Time.deltaTime;
            if (timer >= activeSequence.duration + activeSequence.timeBeforeExit) 
            {
                if (activeSequence.followOnSequence != null) ActivateSequence(activeSequence.followOnSequence);
                else
                {
                    onSequenceComplete?.Invoke();
                    if (onSequenceComplete is not null) onSequenceComplete = null;
                    ReturnToDefault();
                }
                return; 
            }
            else if (timer >= activeSequence.duration && timer < activeSequence.timeBeforeExit)
            {
                onSequenceComplete?.Invoke();
                if (onSequenceComplete is not null) onSequenceComplete = null;
            }

            //update lookat point;
            //update camera position;

            lookPoint.position = activeSequence.aimingPath.EvaluatePositionAtUnit(
                Mathf.MoveTowards(0, 1, timer / activeSequence.duration),
                activeSequence.aimingUnits);

            camPoint.position = activeSequence.travelPath.EvaluatePositionAtUnit(
                Mathf.MoveTowards(0, 1, timer / activeSequence.duration),
                activeSequence.movementUnits);
        }

    }

    private void ReturnToDefault()
    {
        timer = -1;
        activeSequence = null;

        //make the current cam reset priority
        vCam.Priority = -1;
    }
}
