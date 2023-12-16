
using UnityEngine;

[System.Serializable]
public struct LevelDefinition
{
    public enum Difficulty
    {
        EASY, MODERATE, HARD
    }

    public string title;
    public string description;
    public string scene;
    public Sprite image;
    public Difficulty difficulty;
    public GameObject transmission;
}