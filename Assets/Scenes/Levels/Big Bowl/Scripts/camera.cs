using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform ball;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - ball.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = ball.transform.position + offset;

        Vector3 position = new Vector3(
            Mathf.Clamp(transform.position.x, Mathf.NegativeInfinity, 70),
            10,
            0
            );

        transform.position = position;
    }
}
