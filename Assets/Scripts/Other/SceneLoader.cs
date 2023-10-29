using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    //private variables
    private PlayerSkill playerSkill;
    private int currentSceneIndex;

    private void Start()
    {
        //connections
        playerSkill = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkill>();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        //open player skills
        if (currentSceneIndex >= 1)
        {
            playerSkill.openW = true;
            playerSkill.transform.GetChild(0).GetChild(0).GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            playerSkill.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
        }

        if (currentSceneIndex >= 2)
        {
            playerSkill.openE = true;
            playerSkill.transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            playerSkill.transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(true);
        }
    }

    public static void LoadNextScene()
    {
        //load next scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public static void LoadStartScene()
    {
        //load start scene
        SceneManager.LoadScene(0);
    }

    public static void RestartScene()
    {
        //restart scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}