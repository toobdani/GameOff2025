using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_SongManager_System : MonoBehaviour
{
    public S_SongStats_SerliazableObject SongStats;
    public AudioSource GameAudio;
    public int PlayType;

    [SerializeField] private GameObject[] GO;
    [SerializeField] private GameObject[] Hold;
    [SerializeField] private GameObject[] HoldOH;
    [SerializeField] private GameObject[] OH;
    [SerializeField] private GameObject[] Crowd;
    [SerializeField] private GameObject[] Scenes;

    [SerializeField] private S_TimingTypeEnum_Enum StartType;
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
    [SerializeField] private bool HoldEffects;

    [SerializeField] private List<float> TempTimings;
    [SerializeField] private List<float> HoldReleases;
    [SerializeField] private bool DontSpawn;
    [SerializeField] private S_Metronome_Audio Metronome;
    [SerializeField] private S_PerformanceStats_Stats StatStore;

    [SerializeField] private Animator[] CrowdAnimation;
    [SerializeField] private Animator[] PlayerAnimation;
    [SerializeField] private int RepeatCount;

    [SerializeField] private bool TutorialStadium;
    [SerializeField] private TextMeshProUGUI TutorialText;
 
    private GameObject HoldInstance;
    [SerializeField] private GameObject UICanvas;

    private void Start()
    {
        if (TutorialStadium == false)
        {
            switch (StartType)
            {
                case S_TimingTypeEnum_Enum.Stadium:
                    PlayType = 0;
                    Scenes[0].SetActive(true);
                    Scenes[1].SetActive(false);
                    break;
                case S_TimingTypeEnum_Enum.Concert:
                    PlayType = 1;
                    Scenes[0].SetActive(false);
                    Scenes[1].SetActive(true);
                    break;
            }
        }
        else
        {
            StartCoroutine(StadiumTutorial());
            GameAudio.volume = 0;
        }

        GameAudio.clip = SongStats.Song;
        GameAudio.Play();
        //Metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<S_Metronome_Audio>();
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
                tc.BeatTiming = SongStats.TempTimings[i];
                i++;
            }
        }

        if (PlayRhythm)
        {
            StoreRhytym();


            if (Scenes[0].activeSelf == false && Scenes[1].activeSelf == false && TutorialStadium == false)
            {
                if (TempTimings.Count == 0) return;
                if(GameAudio.time >= TempTimings[0])
                {
                    if (PlayType == 0) Scenes[0].SetActive(true);
                    else if (PlayType == 1) Scenes[1].SetActive(true);
                }
                return;
            }

            if(CrowdAnimation[PlayType].transform.parent.gameObject.activeSelf != false)
            {
                if (TempTimings.Count != 0 && CrowdAnimation[PlayType].GetBool("GettingReady") == true)
                {
                    if (GameAudio.time >= TempTimings[0] - TimesByBPM(0.4f) && HoldReleases.Count == 0)
                    {
                        CrowdAnimation[PlayType].SetBool("GettingReady", false);
                        CrowdAnimation[PlayType].Play("JumpingUp");
                        if (RepeatCount == 0)
                        {
                            CrowdAnimation[PlayType].SetBool("GettingReady", false);
                            CrowdAnimation[PlayType].Play("JumpingUp");
                            Instantiate(Crowd[PlayType], GameAudio.transform);
                        }
                        else
                        {
                            float gapTime = RepeatCount == 2 ? Metronome.BPMperSecond : Metronome.BPMperSecond / 3;
                            StartCoroutine(RepeatCrowd(RepeatCount, gapTime));
                        }
                    }
                    else if (GameAudio.time >= TempTimings[0] - TimesByBPM(0.4f) && HoldReleases.Count != 0)
                    {
                        CrowdAnimation[PlayType].SetBool("JumpBeat", true);
                        CrowdAnimation[PlayType].SetBool("GettingReady", false);
                        if (PlayType == 1 && HoldInstance == null) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        else Instantiate(Crowd[PlayType], GameAudio.transform);
                    }
                }
                else if (HoldReleases.Count != 0 && CrowdAnimation[PlayType].GetBool("JumpBeat") == true)
                {
                    if (GameAudio.time >= HoldReleases[0] - TimesByBPM(0.4f))
                    {
                        CrowdAnimation[PlayType].SetBool("JumpBeat", false);
                    }
                }
                else if (TempTimings.Count == 0 && HoldReleases.Count == 0 && (CrowdAnimation[PlayType].GetBool("JumpBeat") == true || CrowdAnimation[PlayType].GetBool("GettingReady") == true))
                {
                    CrowdAnimation[PlayType].SetBool("JumpBeat", false);
                    CrowdAnimation[PlayType].SetBool("GettingReady", false);
                    CrowdAnimation[PlayType].Play("OffScreen");
                }
            }

            

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(PlayType == 0) Instantiate(OH[PlayType], GameAudio.transform);
                PressTime = RoundedInput(GameAudio.time);

                if(PlayerAnimation[PlayType].transform.parent.gameObject.activeSelf != false)
                {
                    if (HoldReleases.Count == 0) PlayerAnimation[PlayType].Play("JumpOnce");
                    else PlayerAnimation[PlayType].Play("JumpStay");
                }

                if (TempTimings.Count == 0)
                {
                    if (HoldReleases.Count != 0)
                    {
                        if (PlayType == 0) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        HoldButton = true;
                    }
                    TextShow("Early", false);
                }
                else if ((PressTime > TempTimings[0] - TimesByBPM(0.8f) && PressTime < TempTimings[0] - TimesByBPM(0.3f)) || (PressTime < TempTimings[0] + TimesByBPM(0.8f) && PressTime > TempTimings[0] + TimesByBPM(0.3f)))
                {
                    if (HoldReleases.Count != 0)
                    {
                        if (PlayType == 0) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        HoldButton = true;
                    }
                    StatStore.AddPoints(1);
                    TextShow("Nearly", false);
                }
                else if (PressTime > TempTimings[0] - TimesByBPM(0.3f) && PressTime < TempTimings[0] + TimesByBPM(0.3f))
                {
                    StatStore.AddPoints(1.5f);
                    TextShow("Perfect", false);
                }
                else
                {
                    StatStore.AddPoints(0);
                    TextShow("Miss", false);
                    if (HoldReleases.Count != 0)
                    {
                        if (PlayType == 0) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        HoldButton = true;
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.Space) && HoldButton == true)
            {
                HoldButton = false;
                DontSpawn = false;
                PlayerAnimation[PlayType].Play("RegularLocation");


                Instantiate(OH[PlayType], GameAudio.transform);
                if (HoldInstance != null && PlayType == 0) Destroy(HoldInstance);
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
                    if (PlayType == 1)
                    {
                        Destroy(HoldInstance);
                    }
                }
                else if (GameAudio.time >= HoldReleases[0] - Metronome.BPMperSecond && GameAudio.time < HoldReleases[0] - TimesByBPM(0.9f) && HoldButton == true)
                {
                    if (DontSpawn == false)
                    {
                        DontSpawn = true;
                        Instantiate(GO[PlayType], GameAudio.transform);
                    }
                }
            }
            /*else if (HoldButton == true)
            {
                HoldButton = false;
                DontSpawn = false;
                if (HoldInstance != null) Destroy(HoldInstance);
                PlayerAnimation[PlayType].Play("RegularLocation");
                CrowdAnimation[PlayType].SetBool("JumpBeat", false);
            }*/

        }
    }

    private void StoreRhytym()
    {

        if (Timings.Count == 0) return;
        float f = 0;
        if (Timings[0].WarningCount == 0) f = 4;
        else f = Timings[0].WarningCount;

        if(Timings[0].ControlType == S_TimingTypeEnum_Enum.Stadium || Timings[0].ControlType == S_TimingTypeEnum_Enum.Concert)
        {
            if (GameAudio.time >= Timings[0].BeatTiming)
            {
                Scenes[0].SetActive(false);
                Scenes[1].SetActive(false);
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond * 4));
                switch (Timings[0].ControlType)
                {
                    case S_TimingTypeEnum_Enum.Stadium:
                        PlayType = 0;
                        break;
                    case S_TimingTypeEnum_Enum.Concert:
                        PlayType = 1;
                        break;
                }
                Timings.RemoveAt(0);
            }
        }
        else if (GameAudio.time >= Timings[0].BeatTiming - TimesByBPM(f))
        {
            CrowdAnimation[PlayType].SetBool("GettingReady", true);
            TempTimings.Add(Timings[0].BeatTiming);
            if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumHold)
            {
                StatStore.AddtoTotal(2);
                Instantiate(Hold[PlayType], GameAudio.transform);
                HoldReleases.Add(Timings[0].EndHold);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumTwo)
            {
                StatStore.AddtoTotal(2);
                StartCoroutine(PlayAudio(2, Metronome.BPMperSecond));
                RepeatCount = 2;
                TempTimings.Add(Timings[0].BeatTiming + Metronome.BPMperSecond);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumThree)
            {
                StatStore.AddtoTotal(3);
                StartCoroutine(PlayAudio(3, Metronome.BPMperSecond / 3));
                RepeatCount = 3;
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3));
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3 * 2));
            }
            else
            {
                StatStore.AddtoTotal(1);
                Instantiate(GO[PlayType], GameAudio.transform);
                if (TutorialStadium == true) StartCoroutine(CountdownUI());
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
        Instantiate(GO[PlayType], GameAudio.transform);
        yield return new WaitForSeconds(gap);
        if(loopAmount - 1 > 0) StartCoroutine(PlayAudio(loopAmount - 1, gap));
    }

    private IEnumerator RepeatCrowd(int loopAmount, float gap)
    {
        RepeatCount = 0;
        CrowdAnimation[PlayType].SetBool("GettingReady", false);
        CrowdAnimation[PlayType].Play("JumpingUp");
        Instantiate(Crowd[PlayType], GameAudio.transform);

        yield return new WaitForSeconds(gap);

        if (loopAmount - 1 > 0) StartCoroutine(RepeatCrowd(loopAmount - 1, gap));
    }

    private IEnumerator StadiumTutorial()
    {
        TutorialText.text = "Let's teach you how to crowd wave like a champ";
        yield return new WaitForSecondsRealtime(5);
        TutorialText.text = "When you hear this noise: ";
        yield return new WaitForSecondsRealtime(2.5f);
        Instantiate(GO[PlayType], GameAudio.transform);
        yield return new WaitForSecondsRealtime(2.5f);
        TutorialText.text = "That means a wave is coming. This sound will always play 4 beats before the wave. Press SPACE in time to get the perfect timing.";
        yield return new WaitForSecondsRealtime(5);
        TutorialText.text = "Let's practice, after hearing the note let's count down to get the timing right. When you hear the noise get ready";
        yield return new WaitForSecondsRealtime(20);
        yield return new WaitForSecondsRealtime(Metronome.BPMperSecond * 4);
        Debug.LogError(GameAudio.time);
    }
    private IEnumerator CountdownUI()
    {
        TutorialText.text = "4";
        yield return new WaitForSecondsRealtime(Metronome.BPMperSecond);
        TutorialText.text = "3";
        yield return new WaitForSecondsRealtime(Metronome.BPMperSecond);
        TutorialText.text = "2";
        yield return new WaitForSecondsRealtime(Metronome.BPMperSecond);
        TutorialText.text = "1";
        yield return new WaitForSecondsRealtime(Metronome.BPMperSecond);
        TutorialText.text = "";
    }
}
