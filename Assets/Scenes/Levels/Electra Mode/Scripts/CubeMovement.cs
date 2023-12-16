using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    public float speed = 2.0f;
    public float maxHeight = 4.0f;  
    public float minHeight = -4.0f;  
    public bool opposite = false;

    void Update()
    {
        float newYPosition;
        switch (opposite)
        {
            case true:
                newYPosition = Mathf.PingPong(Time.time * speed, maxHeight - minHeight) + maxHeight;
                break;
            case false:
                newYPosition = Mathf.PingPong(Time.time * speed, maxHeight - minHeight) + minHeight;
                break;
        }
        

        // Update the object's position
        transform.position = new Vector2(transform.position.x, newYPosition);
    }
}