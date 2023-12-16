using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public TextMeshProUGUI Result;
    public Image CPUChoice;
    public string[] Choices;
    public Sprite Imp, Goblin, Kobold, door;

    public Button[] PlayerOptions;
    public bool win = false;
    public bool lose = false;

    public AudioSource doorOpenSound;

    private void Start()
    {
        ResetBoard();

        foreach(Button option in PlayerOptions)
        {
            option.onClick.AddListener(() => PickChampion(option));
        }
    }

    public void Play(string pChoice)
    {
        Debug.Log("In Play");
        string rChoice = Choices[Random.Range(0, Choices.Length)];

        doorOpenSound.Play();

        if (!win && !lose)
        {
            switch (rChoice)
            {
                case "Imp":
                    switch (pChoice)
                    {
                        case "Imp":
                            Result.text = "There too good friends!";
                            break;
                        case "Goblin":
                            Result.text = "The Goblin Goes Goblin Mode, You Win";
                            win = true;
                            break;
                        case "Kobold":
                            Result.text = "The Kobold Gets Squashed, You Lose";
                            lose = true;
                            break;
                    }
                    CPUChoice.sprite = Imp;
                    break;
                case "Goblin":
                    switch (pChoice)
                    {
                        case "Imp":
                            Result.text = "You Get Goblin'd On, You Lose";
                            lose = true;
                            break;
                        case "Goblin":
                            Result.text = "You Just Hug it Out!";
                            break;
                        case "Kobold":
                            Result.text = "You Slash that MF Goblin Up, You Win";
                            win = true;
                            break;
                    }
                    CPUChoice.sprite = Goblin;
                    break;
                case "Kobold":
                    switch (pChoice)
                    {
                        case "Imp":
                            Result.text = "You Turn That Kobold into a Pancake, You Win";
                            win = true;
                            break;
                        case "Goblin":
                            Result.text = "You Get All Slashed Up, You Lose";
                            lose = true;
                            break;
                        case "Kobold":
                            Result.text = "You Parry the Attack, No Contest";
                            break;
                    }
                    CPUChoice.sprite = Kobold;
                    break;
            }

            if (win)
            {
                Invoke(nameof(TriggerWin), 3f);
            }
            else if (lose)
            {
                Invoke(nameof(TriggerLose), 3f);
            }
            else
            {
                Invoke(nameof(ResetBoard), 3f);
            }
        }
    }

    void ResetBoard()
    {
        CPUChoice.sprite = door;
        Result.text = "Choose your champion!";
        PickChampion(null);
    }

    void TriggerWin()
    {
        LevelManager.GetInstance().Win();
    }
    void TriggerLose()
    {
        LevelManager.GetInstance().Lose();
    }

    void PickChampion(Button button)
    {
        foreach (Button option in PlayerOptions)
        {
            if (option == button)
            {
                option.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else
            {
                option.transform.localScale = new Vector3(.9f, .9f, .9f);
            }
        }
    }

}
