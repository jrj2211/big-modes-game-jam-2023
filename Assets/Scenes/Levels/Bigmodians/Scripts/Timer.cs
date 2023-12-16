using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float remaining;
    private bool active = false;

    // Observable event for when the timer reaches zero
    public delegate void OnTimerExpire();
    public event OnTimerExpire onTimerExpire;

    // Start is called before the first frame update
    public void StartTimer(float time)
    {
        active = true;
        remaining = time;
    }

    public void StopTimer()
    {
        active = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (active)
        {
            RemoveTime(Time.deltaTime);

            if (remaining <= 0)
            {
                active = false;
                if(onTimerExpire != null)
                {
                    onTimerExpire();
                }
                
            }
        }
    }

    public void AddTime(float time)
    {
        remaining = Mathf.Clamp(remaining + time, 0, Mathf.Infinity);
    }

    public void RemoveTime(float time)
    {
        remaining = Mathf.Clamp(remaining - time, 0, Mathf.Infinity);
    }

    public float GetRemaining()
    {
        return remaining;
    }

    public bool IsActive()
    {
        return active;
    }

    public bool HasRemaining(float time, bool requireActive = false)
    {
        return time > remaining && (active || requireActive);
    }
}
