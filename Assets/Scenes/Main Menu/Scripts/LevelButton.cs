using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI title;
    public Image image;
    public Image outline;
    public Button button;

    LevelDefinition level;

    public void SetLevel(LevelDefinition level)
    {
        this.level = level;
  
        title.text = level.title;
        image.sprite = level.image;

        switch(level.difficulty)
        {
            case LevelDefinition.Difficulty.EASY:
                outline.color = new Color(69 / 255f, 134 / 255f, 104 / 255f);
                break;
            case LevelDefinition.Difficulty.MODERATE:
                outline.color = new Color(253 / 255f, 167 / 255f, 16 / 255f);
                break;
            case LevelDefinition.Difficulty.HARD:
                outline.color = new Color(203 / 255f, 62 / 255f, 29 / 255f);
                break;
        }

        button.onClick.AddListener(PlayLevel);
    }


    void PlayLevel()
    {
        LevelManager.GetInstance().StartFromLevel(level);
    }
}
