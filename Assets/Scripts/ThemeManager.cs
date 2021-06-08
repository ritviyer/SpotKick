using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] int startLevel;
    [SerializeField] int endLevel;
    [SerializeField] Text totalStars;
    [SerializeField] Text themeTitle;
    [SerializeField] string title;
    [SerializeField] GameObject levels;
    [SerializeField] GameObject lockedBanner;
    [SerializeField] int minStarsNeeded;
    int starsTotal = 0;


    private void OnEnable()
    {
        themeTitle.text = title;
        //if(CheckThemeUnlocked())
        UnlockLevels();
        totalStars.text = starsTotal.ToString() + "/60";
    }
    private void Update()
    {
        if(Time.frameCount%10 == 5)
        {
            if (Input.GetKey(KeyCode.Escape))
                FindObjectOfType<GameManager>().BackToMainMenu();
        }
    }
    bool CheckThemeUnlocked()
    {
        int s = 0;
        bool l = true;
        for (int i = 1; i < startLevel; i++)
        {
            int temp = PlayerPrefs.GetInt("level" + i.ToString() + "stars", 0);
            if (temp == 0)
            {
                l = false;
                break;
            }
            s += temp;
        }
        if(l && s >= minStarsNeeded)
        {
            lockedBanner.SetActive(false);
            return true;
        }

        lockedBanner.SetActive(true);
        lockedBanner.transform.GetChild(0).GetComponent<Text>().text = "Complete previous levels with minimum " + minStarsNeeded.ToString() +" stars to unlock";

        return false;
    }
    void UnlockLevels()
    {
        starsTotal = 0;
        for (int i = startLevel; i <= PlayerPrefs.GetInt("level", 1); i++)
        {
            if (i > endLevel)
                break;
            levels.transform.Find(i.ToString()).GetComponent<Button>().interactable = true;
            CheckStarsForLevels(i);
        }
    }
    void CheckStarsForLevels(int level)
    {
        int stars = PlayerPrefs.GetInt("level" + level.ToString() + "stars",0);
        starsTotal += stars;
        if (stars >= 1)
            levels.transform.Find(level.ToString()).Find("s1").gameObject.SetActive(true);
        if (stars >= 2)
            levels.transform.Find(level.ToString()).Find("s2").gameObject.SetActive(true);
        if (stars >= 3)
            levels.transform.Find(level.ToString()).Find("s3").gameObject.SetActive(true);
    }
}
