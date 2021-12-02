using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject commands;
    public GameObject pausePanel;
    public static bool gameIsPaused;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
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
