using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlaneController : Vehicle
{
    [SerializeField]
    private Transform flames;

    [SerializeField]
    private SpriteResolver wingAnimation;

    [SerializeField]
    private float acceleration;

    [SerializeField]
    private float deceleration;

    [SerializeField]
    private float maxThrust;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private float pitchAcceleration;

    [SerializeField]
    private float pitchResponsiveness;

    [SerializeField]
    private float liftForce;

    [SerializeField]
    private GameObject explosionPrefab;

    private float curThrust = 1;
    private float curPitch = 0;

    private float enginePitch = 0;
    private float engineVolume = 0;
    float targetSpeed;

    private void Start()
    {
        targetSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float thrust = 0;
        float pitch = 0;

        rigidBody.gravityScale = .25f;

        if (fuel.IsEmpty() || IsDrivable() == false)
        {
            setFlameSize(new Vector3(0, 0, 0), 14);
            thrust = 0;
            rigidBody.gravityScale = 1f;
            targetSpeed = maxSpeed;
        } 
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            setFlameSize(new Vector3(.12f, .20f, .25f), 20);
            thrust = Mathf.Lerp(curThrust, .75f, Time.deltaTime * deceleration);
            targetSpeed = Mathf.Lerp(targetSpeed, maxSpeed * thrust, Time.deltaTime);
            fuel.Use(0.4f);
        }
        else
        {
            setFlameSize(new Vector3(.17f, .25f, .25f), 14);
            thrust = Mathf.Lerp(curThrust, 1, Time.deltaTime * deceleration);
            targetSpeed = Mathf.Lerp(targetSpeed, maxSpeed * thrust, Time.deltaTime);
            fuel.Use(0.6f);
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            wingAnimation.SetCategoryAndLabel("wings", "up");
            pitch = Mathf.Lerp(curPitch, 1, Time.deltaTime * pitchAcceleration);
        } 
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            wingAnimation.SetCategoryAndLabel("wings", "down");
            pitch = Mathf.Lerp(curPitch, -1, Time.deltaTime * pitchAcceleration);
        }
        else
        {
            wingAnimation.SetCategoryAndLabel("wings", "neutral");
            pitch = Mathf.Lerp(curPitch, 0, Time.deltaTime * pitchAcceleration);
        }

        pitch *= rigidBody.velocity.magnitude / maxSpeed;

        if (thrust > 0)
        {
            rigidBody.angularVelocity = 0;
            rigidBody.AddTorque(pitchResponsiveness * pitch * Time.deltaTime);
            rigidBody.AddForce(transform.right * (maxThrust * thrust) * Time.deltaTime);
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, targetSpeed);
        }

        if (fuel.IsEmpty())
        {
            enginePitch = Mathf.Lerp(enginePitch, .4f, Time.deltaTime * 2);
            engineVolume = Mathf.Lerp(engineVolume, 0, Time.deltaTime);
        }
        else
        {
            enginePitch = Mathf.Lerp(enginePitch, 1f * thrust, Time.deltaTime * 3);
            engineVolume = Mathf.Lerp(engineVolume, 1.5f * thrust, Time.deltaTime);
        }

        AudioSource engineSound = GetComponent<AudioSource>();
        engineSound.pitch = enginePitch;
        engineSound.volume = engineVolume;

        curPitch = pitch;
        curThrust = thrust;
    }

    void setFlameSize(Vector3 size, float rate)
    {
        flames.localScale = Vector3.Lerp(flames.localScale, size, Time.deltaTime * rate);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normalImpulse > 1f)
            {
                Vector2 hitPoint = contact.point;
                GameObject explosion = Instantiate(explosionPrefab, new Vector3(hitPoint.x, hitPoint.y, 0), Quaternion.identity);
                RandomizePitchVolume explosionAudio = explosion.GetComponent<RandomizePitchVolume>();
                explosionAudio.minVolume = Mathf.Clamp(contact.normalImpulse / 20, 0, .6f);
                explosionAudio.maxVolume = Mathf.Clamp(contact.normalImpulse / 20, 0, .6f);
                SetDamaged(true);
                fuel.Use(Mathf.Infinity);
            }
        }
    }
}
