using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
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
}
