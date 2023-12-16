using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LevelManager;

public class GameState : MonoBehaviour
{
    private enum State
    {
        ON, OFF, PAUSED
    }

    float timerValue = 0;
    State state;

    private static GameState instance;
    private LevelManager levelManager;

    public TextMeshProUGUI titleGUI;
    public TextMeshProUGUI descriptionGUI;
    public TextMeshProUGUI clockGUI;
    public TextMeshProUGUI levelGUI;

    private GameObject container;
    private Transform transmissionContainer;

    private GameObject currentTransmission;

    public TextMeshProUGUI loadingGUI;

    private int currentLevel = 0;
    private bool won = false;

    // Start is called before the first frame update
    void Start()
    {
        container = transform.Find("Container").gameObject;
        transmissionContainer = transform.Find("Transmissions");

        OnGameEnd();

        levelManager = LevelManager.GetInstance();

        levelManager.onLevelStarted += OnLevelStarted;
        levelManager.onLevelStarting += OnLevelStarting;
        levelManager.onGameStart += OnGameStart;
        levelManager.onGameEnd += OnGameEnd;
        levelManager.onLoadProgress += OnLoadProgress;
        levelManager.onResult += OnResult;

        SceneManager.sceneLoaded += OnSceneLoaded;

        loadingGUI.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Awake()
    {
        if (instance != this && instance)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameState GetInstance()
    {
        return instance; 
    }

    // Update is called once per frame
    void Update()
    {
        if(state== State.ON) {
            timerValue += Time.deltaTime;
        }

        clockGUI.text = ClockString();
    }

    void OnLevelStarting(LevelDefinition level)
    {
        currentLevel += 1;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LevelManager lm = LevelManager.GetInstance();
        container.SetActive(false);
        levelGUI.text = "Level " + CurrentLevel() + " / " + lm.NumTotal();
        titleGUI.text = level.title;
        descriptionGUI.text = level.description;

        state = State.PAUSED;

        loadingGUI.gameObject.SetActive(true);

        AudioListener.volume = 0;

        CleanupTransmission();

        if (level.transmission)
        {
            currentTransmission = Instantiate(level.transmission);
            currentTransmission.transform.SetParent(transmissionContainer);
            currentTransmission.transform.GetComponent<RectTransform>().localScale = new Vector3(1.02f, 1.02f, 1.02f);
            currentTransmission.GetComponent<Animator>().Play("Show");
        }
    }

    void OnLevelStarted(LevelDefinition level)
    {
        container.SetActive(true);
        state = State.ON;

    }

    private void OnGameEnd()
    {
        container.SetActive(false);
        state = State.OFF;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnGameStart()
    {
        won = false;
        currentLevel = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timerValue = 0;
    }

    private void OnLoadProgress(float progress)
    {
        if(progress < 1)
        {
            loadingGUI.text = "Loading " + Math.Round(progress * 100) + "%";
        } 
 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        loadingGUI.gameObject.SetActive(false);
        AudioListener.volume = 1;
        if (currentTransmission != null)
        {
            currentTransmission.GetComponent<Animator>().Play("Hide");
        }
        Invoke(nameof(CleanupTransmission), 1f);
    }

    void CleanupTransmission()
    {
        if (currentTransmission != null) Destroy(currentTransmission);
    }

    public string ClockString()
    {
        int minutes = Mathf.FloorToInt(timerValue / 60F);
        int seconds = Mathf.FloorToInt(timerValue - minutes * 60);

        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public int CurrentLevel()
    {
        return currentLevel;
    }

    void OnResult(bool won)
    {
        this.won = won;
    }

    public bool DidWin()
    {
        return won;
    }
}
