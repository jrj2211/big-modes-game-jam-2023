using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ball ball;
    public TextMeshProUGUI pScoreText;
    public TextMeshProUGUI cScoreText;
    private int _pScore;
    private int _cScore;
    public bool win = false;
    public bool lose = false;

    public void PlayerScores()
    {
        _pScore++;
        this.pScoreText.text = _pScore.ToString();
        this.ball.ResetPosition();
        if(_pScore == 1)
        {
            this.pScoreText.text = "You Win";
            win = true;

            this.ball.Stop();
            
            Invoke(nameof(TriggerWin), 1f);
        }
        //this.ball.AddStartingForce();
    }

    public void CPUScore()
    {
        _cScore++;
        this.cScoreText.text = _cScore.ToString();
        this.ball.ResetPosition();
        if (_cScore == 2 && win == false)
        {
            this.pScoreText.text = "You Lose";
            lose = true;
            this.ball.Stop();
            Invoke(nameof(TriggerLose), 1f);
           
        }
        // this.ball.AddStartingForce();
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
