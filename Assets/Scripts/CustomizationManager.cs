using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationManager : MonoBehaviour
{
    //For active player
    [SerializeField] GameObject players;

    //For theme and custom menu navigation
    [SerializeField] GameObject themesMenu;
    [SerializeField] GameObject customMenu;
    [SerializeField] GameObject levelsUI;

    //For active theme selection
    [SerializeField] GameObject baseTheme;
    [SerializeField] GameObject MainMenuBackground;
    void Start()
    {
        SelectActivePlayer();
        SelectActiveTheme();
    }
    void SelectActivePlayer()
    {
        int active = PlayerPrefs.GetInt("ActivePlayer", 0);
        for(int i = 0; i <players.transform.childCount; i++)
        {
            players.transform.GetChild(i).gameObject.SetActive(false);
        }
        players.transform.GetChild(active).gameObject.SetActive(true);
    }

    public void SelectActiveTheme()
    {
        int level = PlayerPrefs.GetInt("level", 1);
        int active = 0;
        if (level >= 1 && level <= 20)
            active = 0;
        else if (level >= 21)
            active = 1;

        for (int i = 0; i < baseTheme.transform.childCount; i++)
        {
            baseTheme.transform.GetChild(i).gameObject.SetActive(false);
        }
        baseTheme.transform.GetChild(active).gameObject.SetActive(true);

        for (int i = 0; i < MainMenuBackground.transform.childCount; i++)
        {
            MainMenuBackground.transform.GetChild(i).gameObject.SetActive(false);
        }
        MainMenuBackground.transform.GetChild(active).gameObject.SetActive(true);

        for (int i = 0; i < themesMenu.transform.childCount; i++)
        {
            themesMenu.transform.GetChild(i).gameObject.SetActive(false);
        }
        themesMenu.transform.GetChild(active).gameObject.SetActive(true);
    }

    public void GoToCustomMenu()
    {
        themesMenu.SetActive(false);
        levelsUI.SetActive(false);
        customMenu.SetActive(true);
    }
    public void GoToThemeMenu()
    {
        customMenu.SetActive(false);
        SelectActivePlayer();
        themesMenu.SetActive(true);
        levelsUI.SetActive(true);
    }
    public void GotoNextTheme()
    {
        if (themesMenu.transform.GetChild(themesMenu.transform.childCount - 1).gameObject.activeSelf)
            return;
        for (int i = 0; i < themesMenu.transform.childCount-1; i++)
        {
            if (themesMenu.transform.GetChild(i).gameObject.activeSelf)
            {
                themesMenu.transform.GetChild(i).gameObject.SetActive(false);
                baseTheme.transform.GetChild(i).gameObject.SetActive(false);
                themesMenu.transform.GetChild(i+1).gameObject.SetActive(true);
                baseTheme.transform.GetChild(i+1).gameObject.SetActive(true);
                break;
            }
        }
    }
    public void GoToPrevTheme()
    {
        if (themesMenu.transform.GetChild(0).gameObject.activeSelf)
            return;
        for (int i = 1; i < themesMenu.transform.childCount; i++)
        {
            if (themesMenu.transform.GetChild(i).gameObject.activeSelf)
            {
                themesMenu.transform.GetChild(i).gameObject.SetActive(false);
                baseTheme.transform.GetChild(i).gameObject.SetActive(false);
                themesMenu.transform.GetChild(i - 1).gameObject.SetActive(true);
                baseTheme.transform.GetChild(i - 1).gameObject.SetActive(true);
                break;
            }
        }
    }
}
