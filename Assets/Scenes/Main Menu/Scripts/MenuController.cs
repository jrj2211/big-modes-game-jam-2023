using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct MenuData
{
    public string name;
    public GameObject gameObject;
}

public class MenuController : MonoBehaviour
{
    public MenuData[] menus;

    public static string currentMenuName;

    private void OnEnable()
    {
        Show(currentMenuName); 
    }

    public void Show(string menuName)
    {
        Cursor.lockState = CursorLockMode.None;

        if (IsMainMenu())
        {
            bool shown = false;
            foreach (MenuData menu in menus)
            {
                if (menu.name == menuName) shown = true;
                EnableMenu(menu, menu.name == menuName);
            }

            if (shown == false)
            {
                EnableMenu(menus[0], true);
            }
        }
        else
        {
            MenuController.currentMenuName = menuName;
            SceneManager.LoadScene("Assets/Scenes/Main Menu/Main Menu.unity");
        }
    }

    public bool IsMainMenu()
    {
        return SceneManager.GetActiveScene().name == "Main Menu";
    }


    private void EnableMenu(MenuData menuData, bool enabled = true) 
    {
        if (enabled)
        {
            menuData.gameObject.transform.SetAsLastSibling();
        }
    }
}
