using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CircleCollision : MonoBehaviour
{
    public Transform circle;
    private Vector2 startPosition;
    public TMP_Text endingText;

 private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        startPosition = circle.position;
    }
    private void OnTriggerEnter2D(Collider2D colObject)
    {   
        if (colObject.CompareTag("LoseObject"))
        {
            // TODO WIN/LOSE
            endingText.SetText("LOSER");
            //circle.transform.position = startPosition;
            Invoke(nameof(TriggerLose), 2f);
        } 
        else if (colObject.CompareTag("WinObject"))
        {
            endingText.SetText("WINNER");
            Invoke(nameof(TriggerWin), 2f);
        }
    }

    private void ClearText() {
        endingText.SetText(" ");
    }

    void TriggerWin()
    {
        LevelManager.GetInstance().Win();
    }

    void TriggerLose()
    {
        LevelManager.GetInstance().Lose();
    }
}