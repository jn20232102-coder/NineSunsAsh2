using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRegister : MonoBehaviour
{
    void OnEnable()
    {
        CameraRig.Instance?.SetTarget(transform);
    }

    void OnDisable()
    {
        if (CameraRig.Instance && CameraRig.Instance.target == transform)
        {
            CameraRig.Instance.SetTarget(null);
        }
    }
}
