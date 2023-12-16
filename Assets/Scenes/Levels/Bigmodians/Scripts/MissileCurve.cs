using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class MissileCurve : MonoBehaviour
{
    public Transform target;
    public AnimationCurve positionCurve;
    public AnimationCurve scaleCurve;

    private float lastPosition = 0;
    private float length;
    private Spline spline;

    [HideInInspector]
    public Vector3 velocity;

    public delegate void OnComplete(MissileCurve curve);
    public event OnComplete onComplete;

    public Func<MissileCurve, float> getStep;
    public Func<MissileCurve, float> getPosition;

    private void Awake()
    {
        SplineContainer container = GetComponent<SplineContainer>();
        spline = container.Spline;
        length = spline.GetLength();
    }

    public void Activate()
    {
        enabled = true;
        lastPosition = 0;
    }

    public void Deactivate()
    {
        enabled = false;
    }

    void Update()
    {
        if(getStep != null)
        {
            lastPosition += getStep(this);
        }
        else if(getPosition != null)
        {
            lastPosition = getPosition(this);
        }

        if(lastPosition <= 1 && lastPosition >= 0)
        {
            spline.Evaluate(positionCurve.Evaluate(lastPosition), out float3 position, out float3 tangent, out float3 upVector);

            Vector3 positionChange = position - new float3(target.position);

            // Calculate the velocity
            velocity = positionChange / Time.deltaTime;

            // Update Target
            target.position = position;
            target.rotation = Quaternion.LookRotation(upVector, Quaternion.AngleAxis(-90, tangent) * upVector);
            target.localScale = Vector3.one * scaleCurve.Evaluate(lastPosition);
        }

        if(lastPosition >= 1)
        {
            if(onComplete != null)
            {
                onComplete(this);
            }
            
            enabled = false;
        }
    }

    public float RemainingLength()
    {
        return length * (1 - lastPosition);
    }

    public float TraveledLength()
    {
        return length * lastPosition;
    }

    public float TotalLength()
    {
        return length;
    }

    public float GetPosition()
    {
        return lastPosition;
    }
}
