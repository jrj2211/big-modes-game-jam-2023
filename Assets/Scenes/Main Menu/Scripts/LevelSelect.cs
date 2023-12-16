using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public GameObject levelPrefab;

    private LevelManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = LevelManager.GetInstance();

        foreach(LevelDefinition level in manager.GetLevels())
        {
            GameObject instance = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
      
            instance.transform.SetParent(this.transform);
            instance.transform.localScale = Vector3.one;

            LevelButton button = instance.GetComponent<LevelButton>();
            button.SetLevel(level);
            
        }
    }
}
