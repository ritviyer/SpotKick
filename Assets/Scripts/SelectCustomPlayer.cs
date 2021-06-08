using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCustomPlayer : MonoBehaviour
{
    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject selectButton;
    [SerializeField] Text selectText;
    [SerializeField] GameObject lockedButton;
    [SerializeField] Text lockedText;
    [SerializeField] GameObject players;

    int active;

    private void OnEnable()
    {
        active = PlayerPrefs.GetInt("ActivePlayer", 0);
    }
    private void Update()
    {
        if (Time.frameCount % 10 == 9)
        {
            if (Input.GetKey(KeyCode.Escape))
                FindObjectOfType<CustomizationManager>().GoToThemeMenu();
        }

        if (active == 0)
            leftButton.GetComponent<Button>().interactable = false;
        else
            leftButton.GetComponent<Button>().interactable = true;

        if (active == players.transform.childCount-1)
            rightButton.GetComponent<Button>().interactable = false;
        else
            rightButton.GetComponent<Button>().interactable = true;

        if (active == PlayerPrefs.GetInt("ActivePlayer", 0))
        {
            selectButton.GetComponent<Button>().interactable = false;
            selectText.text = "Selected";
        }
        else
        {
            if (PlayerUnlocked(active))
            {
                selectButton.GetComponent<Button>().interactable = true;
                selectText.text = "Select";
            }
            else
                selectButton.GetComponent<Button>().interactable = false;
        }
    }
    public void RightPlayer()
    {
        players.transform.GetChild(active).gameObject.SetActive(false);
        active++;
        players.transform.GetChild(active).gameObject.SetActive(true);
    }
    public void LeftPlayer()
    {
        players.transform.GetChild(active).gameObject.SetActive(false);
        active--;
        players.transform.GetChild(active).gameObject.SetActive(true);
    }
    public void SelectPlayer()
    {
        PlayerPrefs.SetInt("ActivePlayer", active);
    }
    bool PlayerUnlocked(int id)
    {
        int level = PlayerPrefs.GetInt("level", 1);
        int[] unlockAt = new int[5] {0,0,10,20,20};
        if (level > unlockAt[id])
            return true;

        selectText.text = "Unlock after level " + unlockAt[id].ToString();
        return false;
    }
}
