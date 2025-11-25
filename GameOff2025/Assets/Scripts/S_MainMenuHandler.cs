using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class S_MainMenuHandler : MonoBehaviour
{

    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject settingsMenu;

    [Header("SubSettings")]
    [SerializeField] private GameObject gameplayMenu;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject displayMenu;

    private void Update()
    {
        if (titleScreen.activeSelf && Input.anyKey)
        {
            titleScreen.SetActive(false);
        }
    }

    public void ToggleTitle()
    {
        if (titleScreen.activeSelf)
        {
            titleScreen.SetActive(false);
        }
        else
        {
            titleScreen.SetActive(true);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ToggleSettings()
    {
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
        }
        else
        {
            settingsMenu.SetActive(true);
        }
    }

    public void SwitchSubsetting(GameObject subSetting)
    {
        switch (subSetting.name)
        {
            case "GameplayMenu":
                audioMenu.SetActive(false);
                displayMenu.SetActive(false);
                gameplayMenu.SetActive(true);
                break;

            case "AudioMenu":
                displayMenu.SetActive(false);
                gameplayMenu.SetActive(false);
                audioMenu.SetActive(true);
                break;

            case "DisplayMenu":
                gameplayMenu.SetActive(false);
                audioMenu.SetActive(false);
                displayMenu.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ToggleUI(GameObject ui)
    {
        if (ui.activeSelf)
        {
            ui.SetActive(false);
        }
        else
        {
            ui.SetActive(true);
        }
    }

}
