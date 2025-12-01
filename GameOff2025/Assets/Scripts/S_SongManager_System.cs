using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class S_SongManager_System : MonoBehaviour
{
    [Header("Public Variables")]
    public S_SongStats_SerliazableObject SongStats;
    public AudioSource GameAudio;
    public int PlayType;
    public S_SongScriptableObject_System PStats;

    [Header("Audio")]
    [SerializeField] private GameObject[] GO;
    [SerializeField] private GameObject[] Hold;
    [SerializeField] private GameObject[] HoldOH;
    [SerializeField] private GameObject[] OH;
    [SerializeField] private GameObject[] Crowd;

    [Header("Activated Objects")]
    [SerializeField] private GameObject[] Scenes;
    [SerializeField] private GameObject StadiumCrowd;
    [SerializeField] private GameObject Endscreen;
    [SerializeField] private GameObject FadetoBlack;

    [Header("Instantiated Objects and UI")]
    [SerializeField] private GameObject Perfect;
    [SerializeField] private GameObject Good;
    [SerializeField] private GameObject Miss;
    [SerializeField] private GameObject ConcertNoteRegular;
    [SerializeField] private GameObject ConcertNoteHold;

    [Header("System Variables")]
    [SerializeField] private S_TimingTypeEnum_Enum StartType;
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
    [SerializeField] private float SongTime;
    [SerializeField] private bool DontSpawn;
    [SerializeField] private int RepeatCount;
    [SerializeField] private bool FadeRed;
    [SerializeField] private float RedLerp;
    [SerializeField] private float RedCount;
    [SerializeField] private bool BackDown;
    [SerializeField] private Color[] NoteColours;
    private bool ColourSwap;
    private bool WooOnce;
    private List<float> CheckTimes;
    private bool End;

    private GameObject HoldInstance;
    [SerializeField] private GameObject UICanvas;

    [Header("Metronome Variables")]
    [SerializeField] private S_Metronome_Audio Metronome;
    [SerializeField] private S_PerformanceStats_Stats StatStore;

    [Header("Animations")]
    [SerializeField] private Animator[] CrowdAnimation;
    [SerializeField] private Animator[] PlayerAnimation;
    [SerializeField] private GameObject ConcertPlayerParent;

    [Header("Tutorial Variables")]
    [SerializeField] private bool TutorialStadium;
    [SerializeField] private TextMeshProUGUI TutorialText;
    private bool TutorialHold;
    [SerializeField] private bool TutorialConcert;
    private float HoldTime;
 

    private void Start()
    {
        Endscreen.SetActive(false);
        FadetoBlack.GetComponent<Image>().color = new Color(Color.black.r, Color.black.g, Color.black.b, 0);
        SongStats = PStats.LevelSong;
        if (PStats.Concert == true) StartType = S_TimingTypeEnum_Enum.Concert;
        else StartType = S_TimingTypeEnum_Enum.Stadium;

        CheckTimes = new List<float>();
        if (TutorialStadium == false && TutorialConcert == false)
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
            StadiumCrowd.SetActive(true);
            TutorialText.gameObject.SetActive(false);
            
        }
        else if(TutorialStadium == true)
        {
            StartCoroutine(StadiumTutorial());
            GameAudio.volume = 0;
            StadiumCrowd.SetActive(false);
            PlayType = 0;
            Scenes[0].SetActive(true);
            TutorialHold = true;
        }
        else if(TutorialConcert == true)
        {
            StartCoroutine(ConcertTutorial());
            GameAudio.volume = 0;
            PlayType = 1;
            Scenes[1].SetActive(true);
            TutorialHold = true;
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

            if(Timings.Count == 0 && End == false)
            {
                End = true;
                StartCoroutine(EndScreen());
            }

            if (TempTimings.Count == 0 && HoldTime != 0) HoldTime = 0;

            if (Scenes[0].activeSelf == false && Scenes[1].activeSelf == false && TutorialStadium == false && TutorialConcert == false)
            {
                if (TempTimings.Count == 0) return;
                if(GameAudio.time >= TempTimings[0])
                {
                    if (PlayType == 0) Scenes[0].SetActive(true);
                    else if (PlayType == 1) Scenes[1].SetActive(true);
                }
                return;
            }

            if(CrowdAnimation[PlayType].transform.parent.gameObject.activeSelf != false && CrowdAnimation[PlayType].gameObject.activeSelf != false)
            {
                if (CheckTimes.Count != 0)
                {
                    if (GameAudio.time >= CheckTimes[0])
                    {
                        CheckTimes.Remove(CheckTimes[0]);
                        if (CheckTimes.Count == 0) CrowdAnimation[PlayType].SetBool("GettingReady", false);
                        CrowdAnimation[PlayType].Play("JumpingUp");
                        Instantiate(Crowd[PlayType], GameAudio.transform);
                    }
                }
                if (TempTimings.Count != 0 && CrowdAnimation[PlayType].GetBool("GettingReady") == true)
                {
                    if (GameAudio.time >= TempTimings[0] - TimesByBPM(0.4f) && GameAudio.time <= TempTimings[0] - TimesByBPM(0.3f) && HoldReleases.Count != 0 && CheckIfHold(TempTimings.Count == 0 ? 0 : TempTimings[0], false) && WooOnce == false)
                    {
                        CrowdAnimation[PlayType].SetBool("JumpBeat", true);
                        WooOnce = true;
                        if (PlayType == 0) Instantiate(Crowd[PlayType], GameAudio.transform);
                    }
                    else if (GameAudio.time > TempTimings[0] - TimesByBPM(0.3f) && WooOnce == true) WooOnce = false;
                }
                else if (HoldReleases.Count != 0 && CrowdAnimation[PlayType].GetBool("JumpBeat") == true && TempTimings.Count == 0 && PlayType == 0)
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
                Instantiate(OH[PlayType], GameAudio.transform);
                PressTime = RoundedInput(GameAudio.time);

                if(PlayerAnimation[PlayType].transform.parent.gameObject.activeSelf != false)
                {
                    if(PlayType == 1)
                    {
                        if (ConcertPlayerParent.transform.localScale.x > 0) ConcertPlayerParent.transform.localScale = new Vector3(-ConcertPlayerParent.transform.localScale.x, ConcertPlayerParent.transform.localScale.y, ConcertPlayerParent.transform.localScale.z);
                        else ConcertPlayerParent.transform.localScale = new Vector3(ConcertPlayerParent.transform.localScale.x * -1, ConcertPlayerParent.transform.localScale.y, ConcertPlayerParent.transform.localScale.z);
                        
                    }
                    if (HoldReleases.Count == 0) PlayerAnimation[PlayType].Play("JumpOnce");
                    else if (HoldReleases.Count != 0 && CheckIfHold(TempTimings.Count == 0 ? 0 : TempTimings[0], true))
                    {
                        PlayerAnimation[PlayType].Play("JumpStay");
                    }
                }

                if (TempTimings.Count == 0)
                {
                    if (HoldReleases.Count != 0)
                    {
                        if (PlayType == 0) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        HoldButton = true;
                    }
                    TextShow("Early", false);
                    return;
                }
                if(PressTime < TempTimings[0] - TimesByBPM(2))
                {
                    TextShow("Early", false);
                    return;
                }

                if ((PressTime > TempTimings[0] - TimesByBPM(0.9f) && PressTime < TempTimings[0] - TimesByBPM(0.45f)) || (PressTime < TempTimings[0] + TimesByBPM(0.9f) && PressTime > TempTimings[0] + TimesByBPM(0.45f)))
                {
                    if (HoldReleases.Count != 0 && CheckIfHold(TempTimings.Count == 0 ? 0 : TempTimings[0], true))
                    {
                        Debug.LogError("On Nearly");
                        if (PlayType == 0) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        HoldButton = true;
                    }
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(1);
                    TextShow("Nearly", false);
                }
                else if (PressTime > TempTimings[0] - TimesByBPM(0.45f) && PressTime < TempTimings[0] + TimesByBPM(0.45f))
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(1.5f);
                    TextShow("Perfect", false);
                    if (HoldReleases.Count != 0 && CheckIfHold(TempTimings.Count == 0 ? 0 : TempTimings[0], true))
                    {
                        Debug.LogError("On Perfect");
                        if (PlayType == 0) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        HoldButton = true;
                    }
                }
                else
                {
                    if(StatStore.gameObject.activeSelf == true)StatStore.AddPoints(0);
                    TextShow("Miss", false);
                    if (PlayType == 1) FadeRed = true;
                    if (HoldReleases.Count != 0 && CheckIfHold(TempTimings.Count == 0 ? 0 : TempTimings[0], true))
                    {
                        Debug.LogError("On Miss");
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
                if (HoldInstance != null) Destroy(HoldInstance);
                ReleaseTime = RoundedInput(GameAudio.time);

                if (HoldReleases.Count == 0) TextShow("Early", true);
                else if ((ReleaseTime > HoldReleases[0] - TimesByBPM(0.9f) && ReleaseTime < HoldReleases[0] - TimesByBPM(0.45f)) || (ReleaseTime < HoldReleases[0] + TimesByBPM(0.9f) && ReleaseTime > HoldReleases[0] + TimesByBPM(0.45f)))
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(1);
                    TextShow("Nearly", true);
                }
                else if (ReleaseTime > HoldReleases[0] - TimesByBPM(0.45f) && ReleaseTime < HoldReleases[0] + TimesByBPM(0.45f))
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(1.5f);
                    TextShow("Perfect", true);
                }
                else
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(0);
                    TextShow("Miss", true);
                    if (PlayType == 1) FadeRed = true;
                }
            }
            else if(Input.GetKeyUp(KeyCode.Space) && PlayerAnimation[PlayType].name == "JumpStay") PlayerAnimation[PlayType].Play("RegularLocation");
            


            if (TempTimings.Count != 0)
            {
                if (GameAudio.time >= TempTimings[0] + TimesByBPM(1))
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(0);
                    if (PlayType == 1) FadeRed = true;
                    TempTimings.RemoveAt(0);
                }
            }
            if (HoldReleases.Count != 0)
            {
                if (GameAudio.time >= HoldReleases[0] - Metronome.BPMperSecond && GameAudio.time < HoldReleases[0] - TimesByBPM(0.9f) && HoldButton == true)
                {
                    if (DontSpawn == false)
                    {
                        DontSpawn = true;
                        if(PlayType == 0)Instantiate(GO[PlayType], GameAudio.transform);
                    }
                }
                else if (GameAudio.time >= HoldReleases[0] && HoldInstance != null && PlayType == 1) Destroy(HoldInstance);
                else if (GameAudio.time >= HoldReleases[0] + TimesByBPM(0.5f))
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(0);
                    if (PlayType == 1) FadeRed = true;
                    HoldReleases.RemoveAt(0);
                    PlayerAnimation[PlayType].Play("RegularLocation");
                }
            }
            if(TempTimings.Count == 0 && HoldReleases.Count != 0 && (TutorialStadium == true || TutorialConcert == true) && TutorialHold == true)
            {
                TutorialHold = false;
                if(TutorialStadium)StartCoroutine(CountdownUI(8));
                else if(TutorialConcert) StartCoroutine(CountdownUI(4));
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

    private void FixedUpdate()
    {
        SongTime = GameAudio.time;
        if (FadeRed == false) return;
        if (PlayType == 0) return;
        RedLerp += BackDown ? -0.1f : 0.1f;
        RedLerp = Mathf.Clamp(RedLerp, 0, 1);
        PlayerAnimation[1].gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, RedLerp);

        if (BackDown == false && RedLerp >= 1)
        {
            if (RedCount == 1) BackDown = true;
            else RedCount += 0.2f;
        }
        else if(BackDown == true && RedLerp <= 0)
        {
            BackDown = false;
            FadeRed = false;
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
            if (PlayType == 0)
            {
                CrowdAnimation[PlayType].SetBool("GettingReady", true);
            }
            TempTimings.Add(Timings[0].BeatTiming);
            HoldTime = Timings[0].BeatTiming;
            if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumHold)
            {
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(2);
                if (PlayType == 0)
                {
                    Instantiate(Hold[PlayType], GameAudio.transform);
                    StartCoroutine(CrowdAnimation[PlayType].GetComponent<S_CrowdSpriteStore_Stadium>().WaveUp(Timings[0].EndHold, true, 0));
                }
                if (PlayType == 1 && HoldInstance == null) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                if (PlayType == 1)
                {
                    CrowdAnimation[PlayType].SetBool("JumpBeat", true);
                    GameObject tempNote = Instantiate(ConcertNoteHold);
                    tempNote.GetComponent<S_HoldNote_Concert>().BeatTiming = f;
                    tempNote.GetComponent<S_HoldNote_Concert>().HoldLength = Timings[0].EndHold;
                    tempNote.GetComponent<S_HoldNote_Concert>().SetInstance(false);
                }
                HoldReleases.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond * Timings[0].EndHold));
                if ((TutorialStadium == true || TutorialConcert == true) && Timings[0].Ignore == false) StartCoroutine(CountdownUI(4));
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumTwo)
            {
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(2);
                if (PlayType == 0)
                {
                    StartCoroutine(CrowdAnimation[PlayType].GetComponent<S_CrowdSpriteStore_Stadium>().WaveUp(Metronome.BPMperSecond / 2, true, 1));
                    CheckTimes.Add(Timings[0].BeatTiming);
                    CheckTimes.Add(Timings[0].BeatTiming + Metronome.BPMperSecond);
                }
                StartCoroutine(PlayAudio(2, Metronome.BPMperSecond, false, Timings[0].Ignore, f, false));
                TempTimings.Add(Timings[0].BeatTiming + Metronome.BPMperSecond);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumThree)
            {
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(3);
                if (PlayType == 0)
                {
                    StartCoroutine(CrowdAnimation[PlayType].GetComponent<S_CrowdSpriteStore_Stadium>().WaveUp((Metronome.BPMperSecond / 3) / 2, true, 2));
                    CheckTimes.Add(Timings[0].BeatTiming);
                    CheckTimes.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3));
                    CheckTimes.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3 * 2));
                }
                StartCoroutine(PlayAudio(3, Metronome.BPMperSecond / 3, false, Timings[0].Ignore, f, true));
                RepeatCount = 3;
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3));
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3 * 2));
            }
            else
            {
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(1);
                Instantiate(GO[PlayType], GameAudio.transform);
                if (PlayType == 0)
                {
                    if(Timings.Count != 0)CheckTimes.Add(Timings[0].BeatTiming);
                    StartCoroutine(CrowdAnimation[PlayType].GetComponent<S_CrowdSpriteStore_Stadium>().WaveUp(Metronome.BPMperSecond / 2, true, 0));
                }
                if (PlayType == 1)
                {
                    CrowdAnimation[PlayType].SetBool("GettingReady", true);
                    ColourSwap = !(ColourSwap);
                    GameObject tempNote = Instantiate(ConcertNoteRegular);
                    tempNote.GetComponent<S_MusicNote_Concert>().BeatTiming = f;
                    tempNote.AddComponent<S_HalfBeatSignal_System>();
                    tempNote.GetComponent<S_MusicNote_Concert>().InstantiateSetup(ColourReturn(), ColourSwap ? 0.5f : -0.5f);
                }
                if ((TutorialStadium == true || TutorialConcert == true) && Timings[0].Ignore == false) StartCoroutine(CountdownUI(4));
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
                Timings.Add(tempTime);
            }
        }
    }

    private float RoundedBeat(float timing)
    {
        float tempTime = timing / (Metronome.BPMperSecond);
        tempTime = Mathf.Round(tempTime);
        return tempTime * (Metronome.BPMperSecond);
    }

    private float RoundedInput(float timing)
    {
        return Mathf.Round(timing * 10) * 0.1f;
    }

    private IEnumerator PlayAudio(int loopAmount, float gap, bool justSound, bool ignore, float noteLength, bool threeNote)
    {
        Instantiate(GO[PlayType], GameAudio.transform);
        if(PlayType == 1)
        {
            CrowdAnimation[PlayType].SetBool("GettingReady", true);
            ColourSwap = !(ColourSwap);
            GameObject tempNote = Instantiate(ConcertNoteRegular);
            tempNote.GetComponent<S_MusicNote_Concert>().BeatTiming = noteLength;
            if(threeNote)
            {
                tempNote.AddComponent<S_HalfBeatSignal_System>();
            }
            else tempNote.AddComponent<S_HalfBeatSignal_System>();
            tempNote.GetComponent<S_MusicNote_Concert>().InstantiateSetup(ColourReturn(), ColourSwap ? 0.5f : -0.5f);
            tempNote = null;
        }
        if (loopAmount - 1 <= 0 && (TutorialStadium == true || TutorialConcert == true) && ignore == false && justSound == false) StartCoroutine(CountdownUI(4));
        yield return new WaitForSeconds(gap);
        if (loopAmount - 1 > 0) StartCoroutine(PlayAudio(loopAmount - 1, gap, justSound, ignore, noteLength, threeNote));
       
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
        TutorialText.text = "So you want to crowd wave like a champ?";
        yield return new WaitForSecondsRealtime(5);
        TutorialText.text = "You can jump up at any time by pressing SPACE!\nGo on, try it";
        yield return new WaitForSecondsRealtime(5);
        TutorialText.text = "Great! But to really master Crowd Waves means you need to time it with those around you.";
        yield return new WaitForSecondsRealtime(2.5f);
        StadiumCrowd.SetActive(true);
        yield return new WaitForSecondsRealtime(2.5f);
        TutorialText.text = "When you hear this noise: ";
        yield return new WaitForSecondsRealtime(2.5f);
        Instantiate(GO[PlayType], GameAudio.transform);
        yield return new WaitForSecondsRealtime(2.5f);
        TutorialText.text = "That means everyone's getting ready to wave!\nAfter 4 beats press SPACE to join in.";
        yield return new WaitForSecondsRealtime(5);
        TutorialText.text = "Let's practice, after hearing the note let's count down to get the timing right.\nWhen you hear the noise get ready";
        yield return new WaitForSecondsRealtime(8);
        TutorialText.text = "Good Job!";
        yield return new WaitForSecondsRealtime(2.5f);
        TutorialText.text = "Let's Practise again,  but this time without the countdown";
        yield return new WaitForSecondsRealtime(7);
        TutorialText.text = "Fantastic!\nNow, if you hear two of those noises in a row...";
        yield return new WaitForSecondsRealtime(2.5f);
        StartCoroutine(PlayAudio(2, Metronome.BPMperSecond, true, false, 0, false));
        yield return new WaitForSecondsRealtime(3f);
        TutorialText.text = "That means there will be two waves in a row!\nPress space twice after 4 beats!\nLet's try that now";
        yield return new WaitForSecondsRealtime(11);
        TutorialText.text = "Great! And if you hear three noises...";
        yield return new WaitForSecondsRealtime(2.5f);
        StartCoroutine(PlayAudio(3, Metronome.BPMperSecond / 3, true, false, 0, true));
        yield return new WaitForSecondsRealtime(3f);
        TutorialText.text = "That means there will be three waves.\nWhen this happens you need to press SPACE three times in quick succession";
        yield return new WaitForSecondsRealtime(9);
        TutorialText.text = "Alright, lets test you: do a two wave then three wave one after the other.\nWITH NO COUNTDOWN!";
        yield return new WaitForSecondsRealtime(12);
        TutorialText.text = "Perfect! Now there is only one last thing to learn.\nIf you hear this noise...";
        yield return new WaitForSecondsRealtime(2.5f);
        Instantiate(Hold[PlayType], GameAudio.transform);
        yield return new WaitForSecondsRealtime(2.5f);
        TutorialText.text = "That means a Hold Wave is coming after 4 beats.\nWhen this happens keeps holding down SPACE until you hear the Wave Sound";
        yield return new WaitForSecondsRealtime(5f);
        TutorialText.text = "To test this, let's do a Hold Wave that lasts 8 beats.\n";
        yield return new WaitForSecondsRealtime(10f);
        TutorialText.text = "Great, now let's try out a Hold Wave with no countdown, and without knowing how long it will be";
        yield return new WaitForSecondsRealtime(10f);
        TutorialText.text = "Alright, I think you're ready for the actual thing.\nHave fun";
    }

    private IEnumerator ConcertTutorial()
    {
        TutorialText.text = "Meet Lucky*Star, famous Pop Idol!\nYou're lucky enough to see her live!";
        yield return new WaitForSecondsRealtime(5);
        TutorialText.text = "Lucky*Star loves to sing! The only issue is she doesn't sound good";
        yield return new WaitForSecondsRealtime(2.5f);
        Instantiate(GO[PlayType], GameAudio.transform);
        ColourSwap = !(ColourSwap);
        GameObject tempNote = Instantiate(ConcertNoteRegular);
        tempNote.GetComponent<S_MusicNote_Concert>().BeatTiming = 4;
        tempNote.GetComponent<S_MusicNote_Concert>().InstantiateSetup(ColourReturn(), ColourSwap ? 0.5f : -0.5f);
        yield return new WaitForSecondsRealtime(2.5f);
        TutorialText.text = "Listening to her is physically painful.\nWhen she sings you need to dodge out the way by pressing SPACE.\nTry pressing SPACE now!";
        yield return new WaitForSeconds(5f);
        TutorialText.text = "Oh she's getting ready to Sing!\nWhen she does the note will hit your ears after 4 beats, so try to dodge before then.";
        yield return new WaitForSeconds(6f);
        TutorialText.text = "Phew! You did well. Sometimes she can sing two notes in a row.\nWhen this happens make sure to dodge both a beat away from eachother.";
        yield return new WaitForSeconds(8f);
        TutorialText.text = "Oh no! Looks like she's going to sing three notes quickly!\nDODGE!!!";
        yield return new WaitForSeconds(8f);
        TutorialText.text = "Now that you know the basics of dodging, she's getting ready to sing again, but there won't be any countdown to follow!";
        yield return new WaitForSeconds(12f);
        TutorialText.text = "Sometimes Lucky*Star will hold a note, when this happens you will  need to hold space down until she's finished";
        yield return new WaitForSeconds(8f);
        TutorialText.text = "I think you're prepared to go to the real concert!";

    }
    private IEnumerator CountdownUI(int count)
    {
        TutorialText.text = "" + count;
        yield return new WaitForSecondsRealtime(Metronome.BPMperSecond);
        if (count - 1 > 0) StartCoroutine(CountdownUI(count - 1));
        else TutorialText.text = "";
    }

    private Color ColourReturn()
    {
        if (ColourSwap == false) return NoteColours[0];
        else return NoteColours[1];
    }

    private bool CheckIfHold(float time, bool press)
    {
        if (TempTimings.Count == 0) return false;
        if (time == HoldTime)
        {
            //EditorApplication.isPaused = true;
            if (press == true) return true;
            if (GameAudio.time >= time - TimesByBPM(2)) return true;
        }
        return false;
    }

    private IEnumerator ResetWoo()
    {
        yield return new WaitForSecondsRealtime(Metronome.BPM / 4);
        WooOnce = false;
    }

    private IEnumerator EndScreen()
    {
        FadetoBlack.GetComponent<Image>().color = new Color(Color.black.r, Color.black.g, Color.black.b, FadetoBlack.GetComponent<Image>().color.a + 0.01f);
        GameAudio.volume = GameAudio.volume -= 0.01f;
        yield return new WaitForSeconds(0.01f);
        if (GameAudio.volume > 0.5f) StartCoroutine(EndScreen());
        else
        {
            Endscreen.SetActive(true);
            Endscreen.GetComponentInChildren<S_RankCalc_Rank>().ShowRank(StatStore.Percentage);
            yield return new WaitForSeconds(10f);
            SceneManager.LoadScene(0);
        }
    }
}
