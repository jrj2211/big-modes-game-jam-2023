using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2000f;
    public Transform player;
    private float rotationX = 0f;
    private float mouseX;
    private float mouseY;

    void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        rotationX = 0f;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if (mouseY < 5 && mouseY > -5)
        {
            rotationX -= mouseY;
        }
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}