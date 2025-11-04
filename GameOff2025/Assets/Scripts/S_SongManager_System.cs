using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_SongManager_System : MonoBehaviour
{
    public S_SongStats_SerliazableObject SongStats;
    public AudioSource GameAudio;

    [SerializeField] private GameObject GO;
    [SerializeField] private GameObject OH;
    [SerializeField] private bool SetTimings;
    [SerializeField] private bool PutTimings;
    [SerializeField] private bool PlayRhythm;
    [SerializeField] private float PressTime;
    [SerializeField] private List<float> Timings;
    [SerializeField] private bool ButtonPressed;

    [SerializeField] private List<float> TempTimings;


    [SerializeField] private GameObject TimingImage;

    private void Start()
    {
        GameAudio.clip = SongStats.Song;
        GameAudio.Play();
        if (PlayRhythm == false) return;
        foreach (S_TimingClass_Class tc in SongStats.Timings)
        {
            Timings.Add(tc.BeatTiming);
        }
    }

    private void Update()
    {
        if (SetTimings)
        {
            if (Input.GetKeyDown(KeyCode.Space)) SongStats.TempTimings.Add(GameAudio.time);
        }
        if(PutTimings)
        {
            PutTimings = false;
            int i = 0;
            foreach (S_TimingClass_Class tc in SongStats.Timings)
            {
                Debug.Log(i);
                tc.BeatTiming = SongStats.TempTimings[i];
                i++;
            }
        }

        if (PlayRhythm)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(OH, GameAudio.transform);
                PressTime = GameAudio.time;
                ButtonPressed = true;
            }

            DoRhythm();
        }
    }

    private void DoRhythm()
    {

        if (TempTimings.Count != 0)
        {
            if (GameAudio.time >= TempTimings[0] + 0.8 && ButtonPressed == false)
            {
                TempTimings.RemoveAt(0);
            }
        }

        if (ButtonPressed)
        {
            ButtonPressed = false;
            if (TempTimings.Count == 0) StartCoroutine(TextShow("Early"));
            else if ((PressTime > TempTimings[0] - 0.8 && PressTime < TempTimings[0] - 0.3) || (PressTime < TempTimings[0] + 0.8 && PressTime > TempTimings[0] + 0.3))
            {
                StartCoroutine(TextShow("Nearly"));
            }
            else if(PressTime > TempTimings[0] - 0.3 && PressTime < TempTimings[0] + 0.3)
            {
                StartCoroutine(TextShow("Perfect"));
            }
            else
            {
                StartCoroutine(TextShow("Miss"));
            }
        }

        if (Timings.Count == 0) return;
        if (GameAudio.time >= Timings[0] - 1.5)
        {
            Instantiate(GO, GameAudio.transform);
            TempTimings.Add(Timings[0]);
            Timings.RemoveAt(0);
        }
    }

    private IEnumerator TextShow(string text)
    {
        TimingImage.SetActive(true);
        switch(text)
        {
            case "Nearly":
                TimingImage.GetComponent<Image>().color = Color.yellow;
                TempTimings.RemoveAt(0);
                break;
            case "Early":
                TimingImage.GetComponent<Image>().color = Color.black;
                break;
            case "Perfect":
                TimingImage.GetComponent<Image>().color = Color.green;
                TempTimings.RemoveAt(0);
                break;
            case "Miss":
                TimingImage.GetComponent<Image>().color = Color.red;
                     TempTimings.RemoveAt(0);
                break;
        }
  

        yield return new WaitForSeconds(0.3f);
        TimingImage.SetActive(false);
    }
}
