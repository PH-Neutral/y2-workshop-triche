using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    /*
    public GameObject informations;
    public GameObject commands;
    public GameObject crd;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick_Game()
    {
        SceneManager.LoadScene(1); //Go to scene 1
    }

    public void OnClick_Informations()  //Show game's commands
    {
        informations.SetActive(true);
    }

    public void OnClick_Exit()//exit game
    {
        Application.Quit();
    }

    public void OnClick_Credits() //Show credits
    {
        crd.SetActive(true);
    }

    public void OnClick_Commands()  //Show game's commands
    {
        commands.SetActive(true);
    }

    public void OnClick_Menu()
    {
        commands.SetActive(false);
        crd.SetActive(false);
        informations.SetActive(false);
    }
    */
    ////////////////////////////////////////////////////////////////////

    public void LoadScene(int sceneNumber)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneNumber);
    }
    public void OnClick_Exit()
    {
        Application.Quit();
    }
    public void Restart()
    {
        Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    public void CallPause()
    {
        UIManager.Instance.PauseGame();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        MusicManager.instance.musicVolume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("sfxVolume", volume);
        AudioManager.instance.sfxVolume = volume;
    }
}
