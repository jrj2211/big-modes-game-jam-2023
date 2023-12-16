using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplosionController : MonoBehaviour
{
    public GameObject explodingAnimation;
    public GameObject normalEarth;
    public GameObject clouds;
    public GameObject missile;
    public RotateAndPulse flash;

    public AudioSource explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        flash.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerExplode()
    {
        explosionSound.Play();
        missile.SetActive(false);
        flash.gameObject.SetActive(true);
        flash.onPulseComplete += onFlashComplete;
        Invoke(nameof(DestroyMissile), .1f);
        Invoke(nameof(EndLevel), 2f);
    }

    void onFlashComplete()
    {
        flash.gameObject.SetActive(false);
        normalEarth.GetComponent<SpriteRenderer>().enabled = false;
        clouds.GetComponent<SpriteRenderer>().enabled = false;
        explodingAnimation.gameObject.SetActive(true);
    }

    void DestroyMissile()
    {
        missile.SetActive(false);
    }

    void EndLevel()
    {
        LevelManager.GetInstance().Lose();
    }
}
