using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    [SerializeField]
    private Transform body;

    [SerializeField]
    private float duration;

    [SerializeField]
    private float scale;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = body.localPosition;
    }

    void Update()
    {
        float progress = Mathf.Lerp(0, 2 * Mathf.PI, Time.time / duration % 1);
        progress = Mathf.Sin(progress);
        body.localPosition = new Vector3(0, progress * scale, 0) + startPos;
    }
}
