using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPaddle : Paddle
{
    private Vector2 _dir;
    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _dir = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _dir = Vector2.down;
        }
        else
        {
            _dir = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if(_dir.sqrMagnitude != 0)
        {
            _rigidbody.AddForce(_dir * this.speed);
        }
    }
}
