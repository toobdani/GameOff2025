using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class S_MainMenuHandler : MonoBehaviour
{
    public static S_MainMenuHandler Instance;


    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider metronomeSlider;

    [Header("SubSettings")]
    [SerializeField] private GameObject gameplayMenu;
    [SerializeField] private GameObject envismetImg;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject displayMenu;

    [Header("PlayerSettings")]
    [SerializeField] private float musicVolume;
    [SerializeField] private float metronomeVolume;
    [SerializeField] private AudioSource mainAudio;
    [SerializeField] private AudioSource metronomeAudio;
    [SerializeField] private bool VisualmetEnabled;
    [SerializeField] private bool isConcert;
    [SerializeField] private bool isStadium;

    private Vector3 buttonScale;
    private Quaternion buttonRotation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainAudio = GameObject.Find("Main Camera")?.GetComponent<AudioSource>();
        metronomeAudio = GameObject.Find("Metronome")?.GetComponent<AudioSource>();

        if (mainAudio != null)
        {
            mainAudio.volume = (musicVolume / 100);
        }
        if (metronomeAudio != null)
        {
            metronomeAudio.volume = (metronomeVolume / 100);
        }
    }

    private void Update()
    {
        if (titleScreen != null)
        {
            if (titleScreen.activeSelf && Input.anyKey)
            {
                titleScreen.SetActive(false);
                mainMenu.SetActive(true);
            }
        }
        
    }

    public void ToggleTitle()
    {
        if (titleScreen.activeSelf)
        {
            titleScreen.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            titleScreen.SetActive(true);
            mainMenu.SetActive(false);
        }
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }
    public void StartConcert()
    {
        isConcert = true;
        isStadium = false;
        SceneManager.LoadScene(1);
    }
    public void StartStadium()
    {
        isStadium = true;
        isConcert = false;
        SceneManager.LoadScene(1);
    }

    public void ToggleSettings()
    {
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }
    }

    public void ToggleCredits()
    {
        if (creditsMenu.activeSelf)
        {
            creditsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            creditsMenu.SetActive(true);
            mainMenu.SetActive(false);
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

    public void ButtonHover(RectTransform _button)
    {
        buttonScale = _button.localScale;
        buttonRotation = _button.localRotation;
        _button.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        _button.localRotation = Quaternion.Euler(0, 0, -5f);
    }
    public void ButtonExit(RectTransform _button)
    {
        _button.localScale = buttonScale;
        _button.localRotation = buttonRotation;
    }

    public void SetMainVolume(float _value)
    {
        musicVolume = _value;
    }
    public float GetMainVolume()
    {
        return musicVolume;
    }

    public void SetMetronomeVolume(float _value)
    {
        metronomeVolume = _value;
    }
    public float GetMetronomeVolume()
    {
        return metronomeVolume;
    }

    public void ToggleVisualMet()
    {
        VisualmetEnabled = !VisualmetEnabled;
    }
    public bool GetVisualMet()
    {
        return VisualmetEnabled;
    }
}
