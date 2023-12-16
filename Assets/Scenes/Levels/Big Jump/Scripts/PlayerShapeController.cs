using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShapeController : MonoBehaviour
{
    public float jumpForce = 5f;
    private bool isGrounded = true;
    public TMP_Text WinnerText;

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0f, jumpForce, 0f);
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // TODO WIN/LOSE
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Grounded");
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("LoseObject"))
        {
            Invoke(nameof(TriggerLose), 1f);
            WinnerText.SetText("BIG LOSS");
            
        }
        if (collision.gameObject.CompareTag("WinObject"))
        {
            Invoke(nameof(TriggerWin), 1f);
            WinnerText.SetText("WINNER");
        }
    }

    void TriggerLose()
    {
        LevelManager.GetInstance().Lose();
    }

    void TriggerWin()
    {
        LevelManager.GetInstance().Win();
    }
}
