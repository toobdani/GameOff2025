using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_VolumeHandler : MonoBehaviour
{
    private void Awake()
    {
        if (this.name == "MusicVolSlider")
        {
            this.GetComponent<Slider>().value = S_MainMenuHandler.Instance.GetMainVolume();
        }
        else if (true)
        {
            this.GetComponent<Slider>().value = S_MainMenuHandler.Instance.GetMetronomeVolume();
        }
        
    }

    public void SetVolume()
    {
        if (this.name == "MusicVolSlider")
        {
            S_MainMenuHandler.Instance.SetMainVolume(this.GetComponent<Slider>().value);
        }
        else if (this.name == "MetronomeVolSlider")
        {
            S_MainMenuHandler.Instance.SetMetronomeVolume(this.GetComponent<Slider>().value);
        }
        
    }
}
