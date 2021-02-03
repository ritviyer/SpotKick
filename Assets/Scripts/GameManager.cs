using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LionStudios;

public class GameManager : MonoBehaviour
{
    //Main Menu Variables
    [SerializeField] Text playText;
    Vector3 tweenSize = new Vector3(0.75f, 0.75f, 0.75f);
    [SerializeField] GameObject mainMenu;

    // Game Menu Varaibles
    [SerializeField] Text levelText;
    [SerializeField] GameObject gameMenu;

    //GameMenu LevelUp Variables
    [SerializeField] GameObject levelUpCanvas;
    [SerializeField] Text winLoseText;
    [SerializeField] Text nextLevelText;
    Vector3 nextLevelTextScale;


    //Level Variables
    [SerializeField] GameObject levels;
    void Start()
    {
        LeanTween.scale(playText.gameObject, tweenSize, 0.5f).setLoopPingPong();
        nextLevelTextScale = nextLevelText.transform.localScale;
    }
    private void OnEnable()
    {
        EventManager.onStartGame += ChooseLevel;
        EventManager.onRefreshGame += Analy;
        EventManager.onLevelComplete += LevelUp;
    }
    private void OnDisable()
    {
        EventManager.onStartGame -= ChooseLevel;
        EventManager.onRefreshGame -= Analy;
        EventManager.onLevelComplete -= LevelUp;
    }
    void Analy()
    {
        Analytics.Events.LevelRestart(PlayerPrefs.GetInt("level", 1));
    }
    void ChooseLevel()
    {
        if (mainMenu.activeSelf)
        {
            LeanTween.cancel(playText.gameObject);
            mainMenu.SetActive(false);
        }
        if (!gameMenu.activeSelf)
            gameMenu.SetActive(true);

        if (levelUpCanvas.activeSelf)
        {
            LeanTween.cancel(nextLevelText.gameObject);
            levelUpCanvas.SetActive(false);
        }

        int level = PlayerPrefs.GetInt("level", 1);
        Analytics.Events.LevelStarted(level);

        levelText.text = "Level " + level.ToString();
        if(level==1)
            levels.transform.GetChild(0).gameObject.SetActive(true);
        else
        {
            if (levels.transform.childCount < level)
            {
                PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") - 1);
                level -= 1;
                Refresh();
            }
            levels.transform.GetChild(level-2).gameObject.SetActive(false);
            levels.transform.GetChild(level-1).gameObject.SetActive(true);
        }
    }
    void LevelUp()
    {
        Analytics.Events.LevelComplete(PlayerPrefs.GetInt("level", 1));
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level",1) + 1);

        var winText = new List<string>()
                    {
                        "Amazing Kick!",
                        "Perfect Shot!",
                        "Almost a Pro!",
                        "Legendary Skills!",
                        "World-Class Goal!",
                        "Flawless Hit!",
                        "Dicey Shot!",
                        "Touch-and-Go!",
                        "Crazy Kick!"
                    };
        int random = Random.Range(0, winText.Count);
        levelUpCanvas.SetActive(true);
        winLoseText.text = winText[random];
        nextLevelText.transform.localScale = nextLevelTextScale;
        LeanTween.scale(nextLevelText.gameObject, tweenSize, 0.5f).setLoopPingPong();
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Refresh()
    {
        EventManager.RefreshGame();
    }
}
