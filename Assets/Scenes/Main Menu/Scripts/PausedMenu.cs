using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausedMenu : MonoBehaviour
{

    private GameObject container;
    private Button resumeButton;
    private Button mainMenuButton;

    public AudioSource openSound;
    public AudioSource closeSound;

    private static PausedMenu instance;

    private CursorLockMode previousCursor;

    void Awake()
    {
        if (instance != this && instance)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        container = transform.Find("Container").gameObject;
        resumeButton = transform.Find("Container/Buttons/Resume Button/Button").GetComponent<Button>();
        mainMenuButton = transform.Find("Container/Buttons/Main Menu Button/Button").GetComponent<Button>();

        mainMenuButton.onClick.AddListener(() =>
        {
            Paused(false);
            Invoke(nameof(DelayMainMenu), .5f);
        });

        resumeButton.onClick.AddListener(() =>
        {
            Paused(false);
        });

        Paused(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && LevelManager.GetInstance().IsInGame())
        {
            if(Time.timeScale == 1)
            {
                Paused(true);
            }
            else
            {
                Paused(false);
            }
        }
    }

    void Paused(bool paused)
    {
      

        if (IsPaused() != paused)
        {
            container.SetActive(paused);
            Time.timeScale = paused ? 0 : 1;

            if (paused)
            {
                previousCursor = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;
                openSound.Play();
            }
            else
            {
                Cursor.lockState = previousCursor;
                closeSound.Play();
            }
        }
    }

    bool IsPaused()
    {
        return Time.timeScale == 0;
    }

    void DelayMainMenu()
    {
        LevelManager.GetInstance().ShowMainMenu();
    }
}
