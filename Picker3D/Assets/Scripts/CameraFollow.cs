using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform TargetTransform;
    private Vector3 PositionTempVector;
    private float Zdifference = 0;

    void Start()
    {
        Zdifference = Mathf.Abs((TargetTransform.position.z - transform.position.z));
    }
    void LateUpdate()
    {
        if(TargetTransform != null)
        {
            PositionTempVector = transform.position;
            //print("TargetTransform.position.z "+ TargetTransform.position.z+ "  Mathf.Abs(TargetTransform.position.z - transform.position.z) :"+Mathf.Abs(TargetTransform.position.z - transform.position.z));
            PositionTempVector.z = TargetTransform.position.z - Zdifference;
            transform.position = PositionTempVector;

            //PositionTempVector = TargetTransform.position;
            //transform.LookAt(TargetTransform);
        }
    }
}
