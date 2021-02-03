using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void StartGameDelegate();
    public static StartGameDelegate onStartGame;
    public static StartGameDelegate onRefreshGame;
    public static StartGameDelegate onLevelComplete;


    public static void StartGame()
    {
        if (onStartGame != null)
            onStartGame();
    }
    public static void RefreshGame()
    {
        if (onRefreshGame != null)
            onRefreshGame();
    }
    public static void LevelComplete()
    {
        if (onLevelComplete != null)
            onLevelComplete();
    }
}