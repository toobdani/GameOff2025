using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class S_MainMenuHandler : MonoBehaviour
{

    [SerializeField] private GameObject titleScreen;

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

}
