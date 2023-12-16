using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsStats : MonoBehaviour
{
    public TextMeshProUGUI titleGUI;
    public TextMeshProUGUI levelsGUI;
    public TextMeshProUGUI timeGUI;


    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameState state = FindFirstObjectByType<GameState>();   

        LevelManager lm = LevelManager.GetInstance();
        titleGUI.text = state.DidWin() ? "BIG WIN" : "BIG LOSS";
        levelsGUI.text = "Mini modes " + state.CurrentLevel() + " / " + lm.NumTotal();
        timeGUI.text = state.ClockString();
    }

    public void ShowMainMenu()
    {
        LevelManager.GetInstance().ShowMainMenu();
    }

}
