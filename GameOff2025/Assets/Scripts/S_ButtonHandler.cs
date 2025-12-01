using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_ButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject envismetCheck;

    private void OnEnable()
    {
        if (S_MainMenuHandler.Instance.GetVisualMet())
        {
            envismetCheck.SetActive(true);
        }
        else
        {
            envismetCheck.SetActive(false);
        }
        
    }

    public void ToggleVismetImg()
    {
        S_MainMenuHandler.Instance.ToggleVisualMet();

        if (envismetCheck.activeSelf)
        {
            envismetCheck.SetActive(false);
        }
        else
        {
            envismetCheck.SetActive(true);
        }
    }
}
