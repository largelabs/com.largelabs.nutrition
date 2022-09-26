using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class VCamSwitcher : MonoBehaviourBase
{
    [SerializeField] CinemachineVirtualCamera[] VCams = null;

    private const int highVal = 100;
    private const int lowVal = 0;

    private bool locked = false;

    protected override void Awake()
    {
        base.Awake();

        if (VCams == null || VCams.Length < 1)
        {
            VCams = GetComponentsInChildren<CinemachineVirtualCamera>();
        }
    }

    public void SwitchToVCam(CinemachineVirtualCamera i_VCam)
    {
        if (locked) return;

        foreach (CinemachineVirtualCamera vcam in VCams)
        {
            if (vcam == i_VCam)
                vcam.Priority = highVal;
            else
                vcam.Priority = lowVal;
        }
    }

    public void LockSwitching(bool i_lock)
    {
        locked = i_lock;
    }
}
