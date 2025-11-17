using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_SongManager_System : MonoBehaviour
{
    public S_SongStats_SerliazableObject SongStats;
    public AudioSource GameAudio;

    [SerializeField] private GameObject GO;
    [SerializeField] private GameObject Hold;
    [SerializeField] private GameObject HoldOH;
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
    [SerializeField] private S_PerformanceStats_Stats StatStore;

    private GameObject HoldInstance;
    [SerializeField] private GameObject UICanvas;

    private void Start()
    {
        //Metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<S_Metronome_Audio>();
        GameAudio.clip = SongStats.Song;
        GameAudio.Play();
        if (PlayRhythm == false) return;
        SetBeat();
    }

    private void Update()
    {
        if (SetTimings)
        {
            if (Input.GetKeyDown(KeyCode.Space)) SongStats.TempTimings.Add(RoundedBeat(GameAudio.time));
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
                PressTime = RoundedInput(GameAudio.time);


                if (TempTimings.Count == 0) TextShow("Early", false);
                else if ((PressTime > TempTimings[0] - TimesByBPM(0.8f) && PressTime < TempTimings[0] - TimesByBPM(0.3f)) || (PressTime < TempTimings[0] + TimesByBPM(0.8f) && PressTime > TempTimings[0] + TimesByBPM(0.3f)))
                {
                    if (HoldReleases.Count != 0)
                    {
                        HoldInstance = Instantiate(HoldOH, GameAudio.transform);
                        HoldButton = true;
                    }
                    StatStore.AddPoints(1);
                    TextShow("Nearly", false);
                }
                else if (PressTime > TempTimings[0] - TimesByBPM(0.3f) && PressTime < TempTimings[0] + TimesByBPM(0.3f))
                {
                    if (HoldReleases.Count != 0)
                    {
                        HoldInstance = Instantiate(HoldOH, GameAudio.transform);
                        HoldButton = true;
                    }
                    StatStore.AddPoints(1.5f);
                    TextShow("Perfect", false);
                }
                else
                {
                    StatStore.AddPoints(0);
                    TextShow("Miss", false);
                }
            }
            if (Input.GetKeyUp(KeyCode.Space) && HoldButton == true)
            {
                HoldButton = false;
                DontSpawn = false;

                Instantiate(OH, GameAudio.transform);
                if (HoldInstance != null) Destroy(HoldInstance);
                ReleaseTime = RoundedInput(GameAudio.time);

                if (HoldReleases.Count == 0) TextShow("Early", true);
                else if ((ReleaseTime > HoldReleases[0] - TimesByBPM(0.8f) && ReleaseTime < HoldReleases[0] - TimesByBPM(0.3f)) || (ReleaseTime < HoldReleases[0] + TimesByBPM(0.8f) && ReleaseTime > HoldReleases[0] + TimesByBPM(0.3f)))
                {
                    StatStore.AddPoints(1);
                    TextShow("Nearly", true);
                }
                else if (ReleaseTime > HoldReleases[0] - TimesByBPM(0.3f) && ReleaseTime < HoldReleases[0] + TimesByBPM(0.3f))
                {
                    StatStore.AddPoints(1.5f);
                    TextShow("Perfect", true);
                }
                else
                {
                    StatStore.AddPoints(0);
                    TextShow("Miss", true);
                }
            }


            if (TempTimings.Count != 0)
            {
                if (GameAudio.time >= TempTimings[0] + TimesByBPM(0.8f))
                {
                    StatStore.AddPoints(0);
                    TempTimings.RemoveAt(0);
                }
            }
            if (HoldReleases.Count != 0)
            {
                if (GameAudio.time >= HoldReleases[0] + TimesByBPM(0.8f))
                {
                    StatStore.AddPoints(0);
                    HoldReleases.RemoveAt(0);
                }
                else if (GameAudio.time >= HoldReleases[0] - Metronome.BPMperSecond && GameAudio.time < HoldReleases[0] - TimesByBPM(0.9f) && HoldButton == true)
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
                if (HoldInstance != null) Destroy(HoldInstance);
            }

        }
    }

    private void StoreRhytym()
    {

        if (Timings.Count == 0) return;
        float f = 0;
        if (Timings[0].WarningCount == 0) f = 4;
        else f = Timings[0].WarningCount;
        if (GameAudio.time >= Timings[0].BeatTiming - TimesByBPM(f))
        {
           
            TempTimings.Add(Timings[0].BeatTiming);
            if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumHold)
            {
                StatStore.AddtoTotal(2);
                Instantiate(Hold, GameAudio.transform);
                HoldReleases.Add(Timings[0].EndHold);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumTwo)
            {
                StatStore.AddtoTotal(2);
                StartCoroutine(PlayAudio(2, Metronome.BPMperSecond));
                TempTimings.Add(Timings[0].BeatTiming + Metronome.BPMperSecond);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumThree)
            {
                StatStore.AddtoTotal(3);
                StartCoroutine(PlayAudio(3, Metronome.BPMperSecond / 3));
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3));
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3 * 2));
            }
            else
            {
                StatStore.AddtoTotal(1);
                Instantiate(GO, GameAudio.transform);
            }
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

    private float TimesByBPM(float value)
    {
        return Metronome.BPMperSecond * value;
    }
    private void SetBeat()
    {
        foreach (S_TimingClass_Class tc in SongStats.Timings)
        {
            if (SetTimings == true)
            {
                Timings.Add(tc);
            }
            else
            {
                S_TimingClass_Class tempTime = tc;
                tempTime.BeatTiming = RoundedBeat(tempTime.BeatTiming);
                if (tempTime.ControlType == S_TimingTypeEnum_Enum.StadiumHold) tempTime.EndHold = RoundedBeat(tempTime.EndHold);
                Timings.Add(tempTime);
            }
        }
        if (SetTimings == true) SetTimings = false;
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

    private IEnumerator PlayAudio(int loopAmount, float gap)
    {
        Instantiate(GO, GameAudio.transform);
        yield return new WaitForSeconds(gap);
        if(loopAmount - 1 > 0)
        {
            StartCoroutine(PlayAudio(loopAmount - 1, gap));
        }
    }
}
