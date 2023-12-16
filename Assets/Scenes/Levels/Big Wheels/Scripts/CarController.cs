using UnityEngine;
public class CarController : Vehicle
{
    public enum DriveTrain
    {
        FWD, RWD, AWD
    }
    private enum Force
    {
        FORWARD, REVERSE, BRAKE, NONE
    }

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private float minSpeed;

    [SerializeField]
    private bool flipCar = false;

    [SerializeField]
    private float forwardAcceleration;

    [SerializeField]
    private float reverseAcceleration;

    [SerializeField]
    private float decelerationCoast;

    [SerializeField]
    private float decelerationBraked;

    [SerializeField]
    private float pitchResponsiveness;

    [SerializeField]
    private float gravity;

    [SerializeField]
    private WheelJoint2D frontWheel;

    [SerializeField]
    private WheelJoint2D rearWheel;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private DriveTrain driveTrain;

    [SerializeField]
    private float maxTorqueMinSpeed;

    [SerializeField]
    private float maxTorquePitchResponsiveness;

    [SerializeField]
    private GameObject explosionPrefab;

    private WheelJoint2D[] powerWheels;



    private float enginePitch = 0;
    private float engineVolume = 0;

    class MotorData
    {
        public MotorData(float s, bool p, bool u)
        {
            speed = s;
            applyPower = p;
            useMotor = u;
        }
        public readonly float speed;
        public readonly bool applyPower;
        public readonly bool useMotor;
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        switch(driveTrain)
        {
            case DriveTrain.FWD:
                powerWheels = new WheelJoint2D[] { frontWheel };  
                break;
            case DriveTrain.RWD:
                powerWheels = new WheelJoint2D[] { rearWheel };
                break;
            case DriveTrain.AWD:
                powerWheels = new WheelJoint2D[] { frontWheel, rearWheel };
                break;
        }
    }

    void Update()
    {
        float curSpeed = getRealSpeed(powerWheels[0].motor.motorSpeed);
  
        Force force = Force.NONE;

        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && fuel.IsNotEmpty())
        {
            force = Force.FORWARD;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && fuel.IsNotEmpty())
        {
            force = Force.REVERSE;
        }
        
        // When your speed is low, increase the torque
        float torqueSpeedModifier = Mathf.Lerp(1, pitchResponsiveness / maxTorquePitchResponsiveness, (rigidBody.velocity.magnitude / 5));
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            rigidBody.AddTorque(maxTorquePitchResponsiveness * torqueSpeedModifier * Time.deltaTime);
        } 
        else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            rigidBody.AddTorque(maxTorquePitchResponsiveness * torqueSpeedModifier * Time.deltaTime * -1);
        }  

        MotorData motion = getMotorSpeed(curSpeed, force);

        if (Input.GetKeyDown(KeyCode.C) && isGrounded())
        {
            rigidBody.AddForce(new Vector2(0, 30), ForceMode2D.Impulse);
        }

        float newSpeed = motion.speed * (flipCar ? -1 : 1);
        bool applyPower = false;

        // Update wheel motors
        foreach (WheelJoint2D joint in powerWheels)
        {
            if (motion.useMotor == false)
            {
                newSpeed = joint.jointSpeed;
            }

            JointMotor2D motor = joint.motor;
            motor.motorSpeed = newSpeed;
     
            joint.motor = motor;
            joint.useMotor = motion.useMotor && isWheelGrounded(joint.connectedBody.gameObject);

            if(motion.applyPower)
            {
                applyPower = true;
            }

            if(motion.applyPower && joint.useMotor)
            {
                fuel.Use(0.2f);
            } 
            else
            {
                fuel.Use(0.02f);
            }
        }

        if(fuel.IsEmpty())
        {
            enginePitch = Mathf.Lerp(enginePitch, .4f, Time.deltaTime * 2);
            engineVolume = Mathf.Lerp(engineVolume, 0, Time.deltaTime);
        }
        else if(applyPower)
        {
            enginePitch = Mathf.Lerp(enginePitch, .7f, Time.deltaTime * 3);
            engineVolume = Mathf.Lerp(engineVolume, .7f, Time.deltaTime);
        } 
        else 
        {
            enginePitch = Mathf.Lerp(enginePitch, .55f, Time.deltaTime * 3);
            engineVolume = Mathf.Lerp(engineVolume, .3f, Time.deltaTime);
        }

        AudioSource engineSound = GetComponent<AudioSource>();
        engineSound.pitch = enginePitch;
        engineSound.volume = engineVolume;


    }

    MotorData getMotorSpeed(float curSpeed, Force force)
    {
        float newSpeed = curSpeed;
        bool applyPower = false;
        bool useMotor = false;

        switch (force)
        {
            case Force.FORWARD:
                useMotor = true;

                if (curSpeed >= 0)
                {
                    // Forward speed up
                    newSpeed = interpolateSpeed(curSpeed, forwardAcceleration, 0, maxSpeed);
                    applyPower = true;
                } 
                else
                {
                    // Going in reverse so brake until stopped
                    newSpeed = interpolateSpeed(curSpeed, decelerationBraked, minSpeed, 0);
                }
  
                break;
            case Force.REVERSE:
                useMotor = true;

                if (curSpeed <= 0)
                {
                    // Reverse speed up
                    newSpeed = interpolateSpeed(curSpeed, -reverseAcceleration, minSpeed, maxSpeed);
                    applyPower = true;
                }
                else
                {
                    // Going forward so brake until stopped
                    newSpeed = interpolateSpeed(curSpeed, -decelerationBraked, minSpeed, maxSpeed);
                }
                break;
            case Force.BRAKE:
                useMotor = true;

                if (curSpeed >= 0)
                {
                    // Slow down to zero without crossing over zero from positive speed
                    newSpeed = interpolateSpeed(curSpeed, -decelerationBraked, 0, maxSpeed);
                }
                else
                {
                    // Slow down to zero without crossing over zero from negative speed
                    newSpeed = interpolateSpeed(curSpeed, decelerationBraked, minSpeed, 0);
                }
                break;
            case Force.NONE:
                if (curSpeed >= 0)
                {
                    // Slow down to zero without crossing over zero from positive speed
                    newSpeed = interpolateSpeed(curSpeed, -decelerationCoast, 0, maxSpeed);
                }
                else
                {
                    // Slow down to zero without crossing over zero from negative speed
                    newSpeed = interpolateSpeed(curSpeed, decelerationCoast, minSpeed, 0);
                }
                break;
        }

        return new MotorData(newSpeed, applyPower, useMotor);
    }

    float getRealSpeed(float speed)
    {
        return speed * (flipCar ? -1 : 1);
    }

    float interpolateSpeed(float curSpeed, float modifier, float min, float max)
    {
        float carAngle = getCarAngle();
        return Mathf.Clamp(curSpeed + (modifier - gravity * Mathf.PI * (carAngle / 180) * 80) * Time.deltaTime, min, max);
    }

    bool isGrounded()
    {
        foreach(WheelJoint2D joint in powerWheels)
        {
            if(isWheelGrounded(joint.connectedBody.gameObject) == false)
            {
                return false;
            };
        }
        return true;
    }

    bool isWheelGrounded(GameObject wheel)
    {
        float wheelSize = wheel.GetComponent<CircleCollider2D>().radius;
        return Physics2D.OverlapCircle(wheel.transform.position, wheelSize, groundLayer);
    }

    float getCarAngle()
    {
        float angle = transform.localEulerAngles.z;
        // Clamp to -180 to 180
        if (angle > 180) angle = angle - 360;
        return angle;
    }

    public override void Restart()
    {
        base.Restart();
        ResetWheel(frontWheel);
        ResetWheel(rearWheel);
    }

    private void ResetWheel(WheelJoint2D wheel)
    {
        JointMotor2D motor = wheel.motor;
        motor.motorSpeed = 0;
        wheel.motor = motor;
        wheel.useMotor = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.collider.CompareTag("Obstacle"))
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
