using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Main Menu Variables
    [SerializeField] Text playText;
    Vector3 tweenSize = new Vector3(0.75f, 0.75f, 0.75f);
    [SerializeField] GameObject mainMenu;

    // Game Menu Varaibles
    [SerializeField] Text levelText;
    [SerializeField] GameObject gameMenu;
    [SerializeField] GameObject themeMenu;

    //GameMenu LevelUp Variables
    [SerializeField] GameObject levelUpCanvas;
    [SerializeField] Text winLoseText;
    [SerializeField] Text nextLevelText;
    Vector3 nextLevelTextScale;


    //Level Variables
    [SerializeField] GameObject levels;
    bool unlockedNewTheme = false;

    void Start()
    {
        LeanTween.scale(playText.gameObject, tweenSize, 0.5f).setLoopPingPong();
        nextLevelTextScale = nextLevelText.transform.localScale;
        unlockedNewTheme = false;
    }
    private void OnEnable()
    {
        EventManager.onStartGame += StartLevel;
        EventManager.onLevelComplete += LevelUp;
    }
    private void OnDisable()
    {
        EventManager.onStartGame -= StartLevel;
        EventManager.onLevelComplete -= LevelUp;
    }
    public void GoToThemeMenu()
    {
        if (mainMenu.activeSelf)
        {
            LeanTween.cancel(playText.gameObject);
            mainMenu.SetActive(false);
        }

        if (!themeMenu.activeSelf)
            themeMenu.SetActive(true);
    }
    public void SelectLevel(int l)
    {
        GlobalVariables.currentLevel = l;
        EventManager.StartGame();
    }
    void StartLevel()
    {
        if (themeMenu.activeSelf)
            themeMenu.SetActive(false);

        if (!gameMenu.activeSelf)
            gameMenu.SetActive(true);

        if (levelUpCanvas.activeSelf)
        {
            LeanTween.cancel(nextLevelText.gameObject);
            levelUpCanvas.SetActive(false);
        }

        int level = GlobalVariables.currentLevel;

        TinySauce.OnGameStarted(levelNumber: level.ToString());

        levelText.text = "Level " + level.ToString();
        if(level==1)
            levels.transform.Find("Level"+level.ToString()).gameObject.SetActive(true);
        else
        {
            if (levels.transform.childCount < level)
            {
                PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1) - 1);
                unlockedNewTheme = false;
                BackToMainMenu();
            }
            levels.transform.Find("Level" + (level-1).ToString()).gameObject.SetActive(false);
            levels.transform.Find("Level" + level.ToString()).gameObject.SetActive(true);
            if (unlockedNewTheme)
            {
                unlockedNewTheme = false;
                FindObjectOfType<CustomizationManager>().SelectActiveTheme();
            }
        }
    }
    void LevelUp()
    {
        GlobalVariables.currentLevel += 1;
        //FindObjectOfType<GPGController>().AddScoreToLeaderBorad((long)GlobalVariables.currentLevel);

        bool isNewTheme = false;
        if (GlobalVariables.currentLevel > PlayerPrefs.GetInt("level", 1))
        {
            PlayerPrefs.SetInt("level", GlobalVariables.currentLevel);
            if (PlayerPrefs.GetInt("level", 1) % 20 == 1)
            {
                unlockedNewTheme = true;
                isNewTheme = true;
            }
        }
        else if (GlobalVariables.currentLevel % 20 == 1)
            unlockedNewTheme = true;

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
        if (isNewTheme)
            winLoseText.text = "Congratulations!" +System.Environment.NewLine+ "New theme unlocked.";
        else    
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
