
using UnityEngine;

public class RotateAndPulse : MonoBehaviour
{
    public float rotateSpeed;
    public float pulseSpeed;
    public AnimationCurve pulseCurve;
    public AnimationCurve rotateCurve;

    public bool enableRotate = false;
    public bool enablePulse = false;

    public delegate void OnPulseComplete();
    public event OnPulseComplete onPulseComplete;

    public delegate void OnRotateComplete();
    public event OnRotateComplete onRotateComplete;

    private float lastRotate = 0;
    private float lastPulse = 0;
    private float initalTime = 0;

    // Start is called before the first frame update
    private void OnEnable()
    {
        initalTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time - initalTime;

        if (enableRotate)
        {
            float t = time * rotateSpeed % 1;
            if (t < lastRotate && onRotateComplete != null)
            {
                onRotateComplete();
            }
            transform.rotation = Quaternion.AngleAxis(rotateCurve.Evaluate(t) * 360, Vector3.forward);
            lastRotate = t;
        }

        if (enablePulse)
        {
            float t = time * pulseSpeed % 1;
            if(t < lastPulse && onPulseComplete != null)
            {
                onPulseComplete();
            }
            transform.localScale = Vector3.one * pulseCurve.Evaluate(t);
            lastPulse = t;
        }
    }
}
