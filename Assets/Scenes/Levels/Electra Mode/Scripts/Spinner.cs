using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 45f; 

    void Update()
    {

        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}