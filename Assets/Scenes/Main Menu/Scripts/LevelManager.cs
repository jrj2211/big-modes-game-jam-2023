
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class LevelManager : MonoBehaviour
{
    [SerializeField]
    public LevelDefinition[] levels;

    private List<LevelDefinition> shuffled;
    private int id = 0;

    private MenuController menu;

    static LevelManager instance;

    public delegate void OnLevelStarting(LevelDefinition level);
    public event OnLevelStarting onLevelStarting;

    public delegate void OnLevelStarted(LevelDefinition level);
    public event OnLevelStarting onLevelStarted;

    public delegate void OnGameStart();
    public event OnGameStart onGameStart;

    public delegate void OnGameEnd();
    public event OnGameEnd onGameEnd;

    public delegate void OnLoadProgress(float progress);
    public event OnLoadProgress onLoadProgress;

    public delegate void OnResult(bool won);
    public event OnResult onResult;

    private bool isLoading = false;

    void Awake()
    {
        if(instance != this && instance)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        menu = GameObject.FindFirstObjectByType<MenuController>();


        shuffled = new List<LevelDefinition>(levels);
        Shuffle(shuffled);
        menu.Show("main");
    }

    static public LevelManager GetInstance()
    {
        return instance;    
    }

    public void Play()
    {
        id = 0;
        SelectLevel(shuffled[id]);

        if (onGameStart != null)
        {
            onGameStart();
        }
    }

    public void Win()
    {
        if(onResult != null)
        {
            onResult(true);
        }
        id++;
        if (HasNextLevel())
        {
            SelectLevel(shuffled[id]); 
        }
        else
        {
            if(onGameEnd != null)
            {
                onGameEnd();
            }
            ShowResults();

        }
    }

    public void Lose()
    {
        if (onResult != null)
        {
            onResult(false);
        }

        if (onGameEnd != null)
        {
            onGameEnd();
        }

        ShowResults();
    }

    public void ShowResults()
    {
        SceneManager.LoadScene("Assets/Scenes/Results/Results.unity");
    }

    public void ShowLevelSelect()
    {
        menu.Show("levelSelect");
    }

    public void ShowMainMenu()
    {
        menu.Show("main");
    }

    public void ShowCredits()
    {
        menu.Show("credits");
    }

    public void SelectLevel(LevelDefinition level)
    { 
        StartCoroutine(LoadLevelAsync(level));
    }

    IEnumerator LoadLevelAsync(LevelDefinition level)
    {
        if (isLoading == false)
        {
            isLoading = true;
            yield return null;

            if (onLevelStarting != null)
            {
                onLevelStarting(level);
            }

            yield return new WaitForSeconds(1);

            float start = Time.time;

            print("Loading level: " + level.scene);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level.scene);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                if (onLoadProgress != null)
                {
                    onLoadProgress(asyncLoad.progress);
                }
                yield return new WaitForEndOfFrame();
            }


            yield return new WaitForSeconds(2f - (Time.time - start));

            asyncLoad.allowSceneActivation = true;

            if (onLevelStarted != null)
            {
                onLevelStarted(level);
            }

            isLoading = false;
        }
    }

    public int NumBeat()
    {
        return id + 1;
    }

    public int NumCurrent()
    {
        return id;
    }

    public int NumTotal()
    {
        return shuffled.Count;
    }

    public LevelDefinition[] GetLevels()
    {
        return levels;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    static List<T> Shuffle<T>(List<T> _list)
    {
        print("Shuffling list");
        for (int i = 0; i < _list.Count; i++)
        {
            T temp = _list[i];
            int randomIndex = UnityEngine.Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }

        return _list;
    }

    public LevelDefinition GetShuffledLevel(int id = 0)
    {
         return shuffled[id];
    }

    public bool HasNextLevel()
    {
        return id + 1 < shuffled.Count;
    }

    public void StartFromLevel(LevelDefinition level)
    {
        Shuffle(shuffled);

        LevelDefinition first = shuffled[0];

        if(first.Equals(level) == false)
        {
            int swapId = shuffled.FindIndex((LevelDefinition l) =>
            {
                return level.Equals(l);
            });

            // Swap the shuffled first level with the selected start level
            shuffled[swapId] = shuffled[0];
            shuffled[0] = level;
        }

        Play();
    }

    public bool IsInGame()
    {
        return menu.IsMainMenu() == false;
    }
}
