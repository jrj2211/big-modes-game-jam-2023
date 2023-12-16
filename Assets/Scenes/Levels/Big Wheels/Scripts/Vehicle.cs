using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [HideInInspector]
    public FuelController fuel;

    public bool damaged = false;

    protected Rigidbody2D rigidBody;


    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        fuel = GetComponent<FuelController>();
    }

    public bool IsDrivable()
    {
        return damaged == false;
    }

    public bool IsDamaged()
    {
        return damaged;
    }

    public void SetDamaged(bool d)
    {
        damaged = d;
    }

    public virtual void Restart()
    {
        SetDamaged(false);

        if(rigidBody)
        {
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0;
            rigidBody.transform.rotation = Quaternion.identity;
        }
      
        if(fuel)
        {
            fuel.Restart();
        }
    }

    public void OnSwitch()
    {
        TrailRenderer[] trails = transform.GetComponentsInChildren<TrailRenderer>(true);
        foreach (TrailRenderer trail in trails)
        {
            trail.Clear();
        }
    }

    public float GetSpeed()
    {
        return rigidBody.velocity.magnitude * 2.23694f;
    }
}
