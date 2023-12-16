using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BowlingGame : MonoBehaviour
{
    public Transform ball;
    public Transform pins;
    public float ballSpeed = 5f;
    public GameObject pinPrefab;

    private Vector3 ballStartPosition;
    private Quaternion ballStartRotation;

    private Vector3[] pinsStartPosition = new Vector3[10];
    private Quaternion[] pinsStartRotation = new Quaternion[10];

    private Vector3 pinsTipStartingPosition;

    private int frameScore = 0;

    public TMP_Text scoreText;

    private bool inputLockout = false;

    public AudioSource laneAudClip; 

    void Start()
    {
        SetScore(0);
        // Sotre original positions
        ballStartPosition = ball.position;
        ballStartRotation = ball.rotation;

        // Set pins start positions
        for (int i = 0; i < pins.childCount; i++)
        {
            pinsStartPosition[i] = pins.GetChild(i).position;
            pinsStartRotation[i] = pins.GetChild(i).rotation;
        }

        pinsTipStartingPosition = pins.GetChild(0).GetChild(0).position;
    }

    void Update()
    {
        float horizontalInput = -Input.GetAxis("Horizontal");
        MoveBall(horizontalInput);

        if (inputLockout == false) 
        {
            if (Input.GetKeyDown("space"))
            {
                ReleaseBall();
            }
        }

    }

    void MoveBall(float horizontalInput)
    {
        Vector3 currentPos = ball.position;
        currentPos.z += horizontalInput * ballSpeed * Time.deltaTime;
        ball.position = currentPos;
    }

    void ReleaseBall()
    {
        inputLockout = true;
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.isKinematic = false;
        ballRigidbody.AddForce(Vector3.right * 10000f);

        Invoke("ResetAlley", 10f);
        Invoke("CalculateScore", 9.9f);
        Invoke("inputLockoutFree", 10f);
    }

    void ResetAlley()
    {
        ball.position = ballStartPosition;
        ball.rotation = ballStartRotation;

        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.isKinematic = true;

        for (int i = 0; i < pins.childCount; i++)
        {
            pins.GetChild(i).position = pinsStartPosition[i];
            pins.GetChild(i).rotation = pinsStartRotation[i];
            Rigidbody pinRigidbody = pins.GetChild(i).GetComponent<Rigidbody>();
            pinRigidbody.isKinematic = true;
            pinRigidbody.isKinematic = false;
        }
    }

    void CalculateScore()
    {
        for (int i = 0; i < pins.childCount; i++)
        {
            if (pins.GetChild(i).GetChild(0).position.y < (pinsTipStartingPosition.y - 1))
            {
                frameScore += 1;
            }
            print("Pin Starting Pos: " + pinsTipStartingPosition.y + " Pin Ending Pos: " + (pins.GetChild(i).GetChild(0).position.y - 1));
        }

        SetScore(frameScore);

        if (frameScore == 10) {

            Invoke(nameof(TriggerWin), 1f);
        }
        frameScore = 0;
    }

    void inputLockoutFree() 
    {
        inputLockout = false;
    }

    void TriggerWin()
    {
        LevelManager.GetInstance().Win();
    }

    void SetScore(int score)
    {
        scoreText.SetText("Pins:" + score);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BowlingLane"))
        {
            laneAudClip.Play();
        }
    }

}
