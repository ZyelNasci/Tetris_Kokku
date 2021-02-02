using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : Singleton<UIController>
{
    [Header("Component")]
    public ScoreController scoreControl;

    [Header("Interface Group")]
    public GameObject MenuGroup;
    public GameObject InGameGroup;
    public GameObject GameOverGroup;

    public delegate void  GameEvents();
    public GameEvents StartEvent;
    public GameEvents StopEvent;

    [Space(5)]
    public ScreenState currentState;

    private void Awake()
    {
        scoreControl = GetComponent<ScoreController>();
        SwitchState(ScreenState.Menu);
    }

    void SwitchState(ScreenState newState)
    {
        switch (currentState)
        {
            case ScreenState.Menu:
                MenuGroup.SetActive(false);
                break;
            case ScreenState.InGame:
                InGameGroup.SetActive(false);
                break;
            case ScreenState.GameOver:
                GameOverGroup.SetActive(false);
                break;
        }

        switch (newState)
        {
            case ScreenState.Menu:
                MenuGroup.SetActive(true);
                break;
            case ScreenState.InGame:
                InGameGroup.SetActive(true);
                StartEvent();
                break;
            case ScreenState.GameOver:
                GameOverGroup.SetActive(true);
                break;
        }

        currentState = newState;
    }

    public void OpenMenu()
    {
        SwitchState(ScreenState.Menu);
    }
    public void OpenInGame()
    {
        SwitchState(ScreenState.InGame);
    }

    public void OpenGameOver()
    {
        SwitchState(ScreenState.GameOver);
    }

    public void StopGame()
    {
        StopEvent();
        SwitchState(ScreenState.Menu);
    }
}
