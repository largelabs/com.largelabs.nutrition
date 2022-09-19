using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class VCamSwitcher : MonoBehaviourBase
{
    [SerializeField] CinemachineVirtualCamera[] VCams = null;

    private const int highVal = 100;
    private const int lowVal = 0;

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
        foreach (CinemachineVirtualCamera vcam in VCams)
        {
            if (vcam == i_VCam)
                vcam.Priority = highVal;
            else
                vcam.Priority = lowVal;
        }
    }
}
