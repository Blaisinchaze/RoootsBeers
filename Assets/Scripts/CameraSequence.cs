using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraSequence : MonoBehaviour
{
    public float duration;
    public float timeBeforeExit;

    public CinemachineSmoothPath travelPath;
    public CinemachineSmoothPath aimingPath;

    public CinemachinePathBase.PositionUnits aimingUnits = CinemachinePathBase.PositionUnits.Normalized;
    public CinemachinePathBase.PositionUnits movementUnits = CinemachinePathBase.PositionUnits.Normalized;

    public CameraSequence followOnSequence;

    //[Header("Only for use with PathUnits")]
    //public float[] timeAtCheckPoint_aiming;
    //public float[] timeAtCheckPoint_travel;

    public Vector3 GetPointOnPath(PathType type, float time) 
    {
        CinemachineSmoothPath path = type == PathType.AIMING ? aimingPath : travelPath;
        CinemachinePathBase.PositionUnits units = type == PathType.AIMING ? aimingUnits : movementUnits;

        float t = 0;
        
        switch (units)
        {
            case CinemachinePathBase.PositionUnits.PathUnits:
                //float[] timesAtPoints = type == PathType.AIMING ? timeAtCheckPoint_aiming : timeAtCheckPoint_travel;
                //for (int i = 0; i < path.m_Waypoints.Length; i++)
                //{
                //    if (t > runningTotal)
                //    {

                //    }
                //}
                Debug.LogWarning("I don't think this one is as nice as I want it to be");
                break;
            case CinemachinePathBase.PositionUnits.Distance:
                t = path.PathLength * (time / duration);
                break;
            case CinemachinePathBase.PositionUnits.Normalized:
                t = Mathf.MoveTowards(0, 1, time / duration);
                break;
            default:
                break;
        }


        return aimingPath.EvaluatePositionAtUnit(t, units);
    }
}

public enum PathType 
{
    AIMING = 0,
    MOVEMENT = 1
}

