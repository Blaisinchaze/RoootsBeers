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
}

