using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFaceCamera : MonoBehaviour
{
    private Transform MainCameraTransform;
    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + MainCameraTransform.rotation * Vector3.forward, MainCameraTransform.rotation * Vector3.up);
    }
}
