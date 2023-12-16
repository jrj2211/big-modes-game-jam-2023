using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowNextGame : MonoBehaviour
{
    private void Start()
    {
        LevelManager manager = LevelManager.GetInstance();
        Image image = GetComponent<Image>();

        if (manager != null && manager.HasNextLevel())
        {
            image.sprite = manager.GetShuffledLevel(0).image;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = Color.gray;
        }
    }
}
