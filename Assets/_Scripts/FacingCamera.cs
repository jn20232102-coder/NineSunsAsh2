using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    Transform[] childobjects;

    void Start()
    {
        childobjects = GetComponentsInChildren<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            childobjects[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < childobjects.Length; i++)
        {
            childobjects[i].rotation = Camera.main.transform.rotation;
        }
    }
}
