using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject commands;
    public GameObject pausePanel;
    [SerializeField] GameObject loosePanel, winPanel;
    public static bool gameIsPaused;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
    }
    public void Loose()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
        loosePanel.SetActive(true);
    }
    public void Win()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
        winPanel.SetActive(true);
    }
    void PauseGame()
    {
        if (gameIsPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
    }

    public void OnClick_Restart()
    {
        SceneManager.LoadScene(1); //Go to scene 1
    }

    public void OnClick_Commands()  //Show game's commands
    {
        commands.SetActive(true);
    }

    public void OnClick_Menu()
    {
        SceneManager.LoadScene(0); //Go to scene 1
    }


    public void OnClick_Pause()
    {
        commands.SetActive(false);
    }
}
