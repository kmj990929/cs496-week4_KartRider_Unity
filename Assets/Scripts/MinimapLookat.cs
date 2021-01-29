using System;
using System.Collections.Generic;
using UnityEngine;
public class MinimapLookat : MonoBehaviour
{
    public Transform target;
    public Transform camTransform;
    public Vector3 Offset;
    public float SmoothTime = 0.0f;
    private Vector3 velocity = Vector3.zero;
    public float dampRotate = 5.0f;
    public float dist = 0.1f;
    public float height = 0.1f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Offset = camTransform.position - target.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + Offset;
        float currYAngle = Mathf.LerpAngle(transform.eulerAngles.y, target.eulerAngles.y, dampRotate * Time.deltaTime);
        Quaternion rot = Quaternion.Euler(0, currYAngle, 0);

        Vector3 testPosition = target.position - (rot * Vector3.forward * dist) + (Vector3.up * height);
        Vector3 resultPosition = new Vector3(testPosition.x, 50, testPosition.z);
        transform.position = Vector3.SmoothDamp(transform.position, resultPosition, ref velocity, SmoothTime);

        transform.LookAt(target);
    }
}