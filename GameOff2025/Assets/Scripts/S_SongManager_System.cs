using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_SongManager_System : MonoBehaviour
{
    public S_SongStats_SerliazableObject SongStats;
    public AudioSource GameAudio;

    [SerializeField] private GameObject GO;
    [SerializeField] private GameObject GOGO;
    [SerializeField] private GameObject GOGOGO;
    [SerializeField] private GameObject Hold;
    [SerializeField] private GameObject OH;

    [SerializeField] private GameObject Perfect;
    [SerializeField] private GameObject Good;
    [SerializeField] private GameObject Miss;
    [SerializeField] private bool SetTimings;
    [SerializeField] private bool PutTimings;
    [SerializeField] private bool PlayRhythm;
    [SerializeField] private float PressTime;
    [SerializeField] private float ReleaseTime;
    [SerializeField] private List<S_TimingClass_Class> Timings;
    [SerializeField] private bool HoldButton;

    [SerializeField] private List<float> TempTimings;
    [SerializeField] private List<float> HoldReleases;
    [SerializeField] private bool DontSpawn;
    [SerializeField] private S_Metronome_Audio Metronome;


    [SerializeField] private GameObject UICanvas;

    private void Start()
    {
        Metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<S_Metronome_Audio>();
        GameAudio.clip = SongStats.Song;
        GameAudio.Play();
        if (PlayRhythm == false) return;
        SetBeat();
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
            StoreRhytym();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(OH, GameAudio.transform);
                PressTime = RoundedBeat(GameAudio.time);


                if (TempTimings.Count == 0) TextShow("Early", false);
                else if ((PressTime > TempTimings[0] - 0.8 && PressTime < TempTimings[0] - 0.3) || (PressTime < TempTimings[0] + 0.8 && PressTime > TempTimings[0] + 0.3))
                {
                    if (HoldReleases.Count != 0) HoldButton = true;
                    TextShow("Nearly", false);
                }
                else if (PressTime > TempTimings[0] - 0.3 && PressTime < TempTimings[0] + 0.3)
                {
                    if (HoldReleases.Count != 0) HoldButton = true;
                    TextShow("Perfect", false);
                }
                else
                {
                    TextShow("Miss", false);
                }
            }
            if (Input.GetKeyUp(KeyCode.Space) && HoldButton == true)
            {
                HoldButton = false;
                DontSpawn = false;

                Instantiate(OH, GameAudio.transform);
                ReleaseTime = RoundedBeat(GameAudio.time);

                if (HoldReleases.Count == 0) TextShow("Early", true);
                else if ((ReleaseTime > HoldReleases[0] - 0.8 && ReleaseTime < HoldReleases[0] - 0.3) || (ReleaseTime < HoldReleases[0] + 0.8 && ReleaseTime > HoldReleases[0] + 0.3))
                {
                    TextShow("Nearly", true);
                }
                else if (ReleaseTime > HoldReleases[0] - 0.3 && ReleaseTime < HoldReleases[0] + 0.3)
                {
                    TextShow("Perfect", true);
                }
                else
                {
                    TextShow("Miss", true);
                }
            }


            if (TempTimings.Count != 0)
            {
                if (GameAudio.time >= TempTimings[0] + 0.8)
                {
                    TempTimings.RemoveAt(0);
                }
            }
            if (HoldReleases.Count != 0)
            {
                if (GameAudio.time >= HoldReleases[0] + 0.8)
                {
                    HoldReleases.RemoveAt(0);
                }
                else if (GameAudio.time >= HoldReleases[0] - 1.5 && GameAudio.time < HoldReleases[0] - 1.4 && HoldButton == true)
                {
                    if (DontSpawn == false)
                    {
                        DontSpawn = true;
                        Instantiate(GO, GameAudio.transform);
                    }
                }
            }
            else if (HoldButton == true)
            {
                HoldButton = false;
                DontSpawn = false;
            }

        }
    }

    private void StoreRhytym()
    {

        if (Timings.Count == 0) return;
        if (GameAudio.time >= Timings[0].BeatTiming - 1.5)
        {
           
            TempTimings.Add(Timings[0].BeatTiming);
            if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumHold)
            {
                Instantiate(Hold, GameAudio.transform);
                HoldReleases.Add(Timings[0].EndHold);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumTwo)
            {
                Instantiate(GOGO, GameAudio.transform);
                TempTimings.Add(Timings[0].BeatTiming + Metronome.BPMperSecond);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumThree)
            {
                Instantiate(GOGOGO, GameAudio.transform);
                TempTimings.Add(Timings[0].BeatTiming + Metronome.BPMperSecond);
                TempTimings.Add(Timings[1].BeatTiming + (2 * Metronome.BPMperSecond));
            }
            else Instantiate(GO, GameAudio.transform);
            Timings.RemoveAt(0);
        }
    }

    private void TextShow(string text, bool isHold)
    {
        switch(text)
        {
            case "Nearly":
                Instantiate(Good, UICanvas.transform);
                if (isHold == false) TempTimings.RemoveAt(0);
                else HoldReleases.RemoveAt(0);
                break;
            case "Early":
                //UICanvas.GetComponent<Image>().color = Color.black;
                break;
            case "Perfect":
                Instantiate(Perfect, UICanvas.transform);
                if (isHold == false) TempTimings.RemoveAt(0);
                else HoldReleases.RemoveAt(0);

                break;
            case "Miss":
                Instantiate(Miss, UICanvas.transform);
                if (isHold == false) TempTimings.RemoveAt(0);
                else HoldReleases.RemoveAt(0);
                break;
        }
  
    }

    private void SetBeat()
    {
        foreach (S_TimingClass_Class tc in SongStats.Timings)
        {
            S_TimingClass_Class tempTime = tc;
            tempTime.BeatTiming = RoundedBeat(tempTime.BeatTiming);
            if (tempTime.ControlType == S_TimingTypeEnum_Enum.StadiumHold) tempTime.EndHold = RoundedBeat(tempTime.EndHold);
            Timings.Add(tc);
        }
    }

    private float RoundedBeat(float timing)
    {
        float tempTime = timing / Metronome.BPMperSecond;
        tempTime = Mathf.Round(tempTime);
        return tempTime * Metronome.BPMperSecond;
    }

    private float RoundedInput(float timing)
    {
        return Mathf.Round(timing * 10) * 0.1f;
    }
}
