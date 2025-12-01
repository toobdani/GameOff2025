using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_VisualMetronome : MonoBehaviour
{
    [SerializeField] private S_BeatSignal_System beatSignal;
    [SerializeField] private S_Metronome_Audio metronomeAudio;
    [SerializeField] private GameObject visualMetronome;


    [SerializeField] private RectTransform metImage; 
    [SerializeField] private GameObject barStart;
    [SerializeField] private RectTransform barEnd;

    private float timer = 0f;
    private float duration;
    //private float speed;

    private void Awake()
    {
        duration = metronomeAudio.BPMperSecond * 4;

        //speed = Vector3.Distance(barStart.transform.position, barEnd.transform.position) / duration;

        metImage.position = barStart.transform.position;

        if (S_MainMenuHandler.Instance.GetVisualMet())
        {
            visualMetronome.SetActive(true);
        }
        else
        {
            visualMetronome.SetActive(false);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < duration / 4)
        {
            metImage.localPosition = Vector3.zero;
        }
        else if (timer < duration / 2)
        {
            metImage.localPosition = (barEnd.localPosition) / 3;
        }
        else if (timer < (duration / 4) * 3)
        {
            metImage.localPosition = ((barEnd.localPosition) / 3) * 2;
        }
        else if (timer < duration)
        {
            metImage.localPosition = barEnd.localPosition;
        }
        else
        {
            timer = 0f;
            metImage.localPosition = Vector3.zero;
        }
    }
}
