using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class TimerController : Timer
{
    private enum State {
        WIN, LOSE, ACTIVE
    }

    public AudioSource errorSound;
    public AudioSource successSound;
    public AudioSource nearDeathSound;
   
    public AudioSource missileSound;

    public AnimationCurve missileSoundCurve;

    public ExplosionController explosion;

    public TextMeshProUGUI timerGUI;

    public float totalTime;
    public float errorPenalty;
    public float wordBonus;

    public SplineContainer spline;
    public SplineContainer splineFail;

    private State state = State.ACTIVE;

    public Transform missile;

    bool destroyEarth = false;
    bool finished = false;
    bool saved = false;
    float releaseVelocity;

    MissileCurve currentCurve;

    // Start is called before the first frame update
    void Start()
    {
        StartTimer(totalTime);
        onTimerExpire += TriggerFail;

        MissileCurve curve = spline.gameObject.GetComponent<MissileCurve>();
        curve.target = missile;
        curve.getStep = MissilePositionFromTimer;
        curve.onComplete += InitialPathComplete;
        curve.Activate();

        currentCurve = curve;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
      
        if(GetRemaining() <= 10)
        {
            if(nearDeathSound.enabled && nearDeathSound.isPlaying == false)
            {
                nearDeathSound.Play(); 
            }
            timerGUI.color = new Color(1, .325f, .325f, 1);
        } 
        else if(nearDeathSound.isPlaying)
        {
            nearDeathSound.Stop();
            timerGUI.color = Color.white;
        }

        if(saved == false && destroyEarth == false)
        {
            missileSound.volume = missileSoundCurve.Evaluate(currentCurve.GetPosition());
        }

        if (saved)
        {
            timerGUI.text = "Disarmed";
            timerGUI.color = new Color(0.419f, 0.877f, .415f, 1);
            timerGUI.color = new Color(0.419f, 0.877f, .415f, 1);
        }
        else if (destroyEarth)
        {
            timerGUI.text = "Humanity Destroyed";
            timerGUI.color = new Color(1, .325f, .325f, 1);
        }
        else
        {
            timerGUI.text = "Impact in " + GetRemaining().ToString("F1") + "s";
        }

        if (saved)
        {
            if(releaseVelocity < 300)
            {
                releaseVelocity = Mathf.SmoothStep(releaseVelocity, 300, Time.deltaTime * 2);
            }
            missile.position += (currentCurve.velocity.normalized * releaseVelocity) * Time.deltaTime;
        }
    }

    void InitialPathComplete(MissileCurve lastCurve)
    {
        nearDeathSound.Stop();

        MissileCurve curve = splineFail.gameObject.GetComponent<MissileCurve>();
        curve.target = missile;
        curve.velocity = lastCurve.velocity;
        curve.getPosition = MissilePositionFail;
        curve.onComplete += DestroyEarth; 
        curve.Activate();

        currentCurve = curve;
    }

    float MissilePositionFail(MissileCurve curve)
    {
        float magnitude = curve.velocity.magnitude;
        if(magnitude < 60)
        {
            magnitude = Mathf.Lerp(magnitude, 60, Time.deltaTime * 5);
        }
        float traveled = curve.TraveledLength() + (magnitude * Time.deltaTime);
        return Mathf.Clamp(traveled / curve.TotalLength(), 0, 1);
    }

    float MissilePositionFromTimer(MissileCurve curve)
    {
        float distanceThisFrame = (curve.RemainingLength() / GetRemaining()) * Time.deltaTime;
        return distanceThisFrame / curve.TotalLength();
    }

    public void TriggerSuccess()
    {
        StopTimer();
        saved = true;
        releaseVelocity = currentCurve.velocity.magnitude;
        currentCurve.Deactivate();
        Invoke(nameof(TriggerWin), 7f);
    }

    public void TriggerWord()
    {
        AddTime(wordBonus);
        successSound.Play();
    }

    public void TriggerError(bool reduceTime)
    {
        if(reduceTime)
        {
            RemoveTime(errorPenalty);
        }
        errorSound.Play();
    }

    void TriggerFail()
    {
        destroyEarth = true;
    }

    void DestroyEarth(MissileCurve lastCurve)
    {
        nearDeathSound.Stop();
        nearDeathSound.enabled = false;
        explosion.TriggerExplode();
    }

    void TriggerWin()
    {
        LevelManager.GetInstance().Win();
    }
   
}
