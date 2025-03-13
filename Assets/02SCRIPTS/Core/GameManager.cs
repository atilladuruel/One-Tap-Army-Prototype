using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
    }
}
