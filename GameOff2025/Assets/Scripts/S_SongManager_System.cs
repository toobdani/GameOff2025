using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_SongManager_System : MonoBehaviour
{
    [Header("Public Variables")]
    public S_SongStats_SerliazableObject SongStats;
    public AudioSource GameAudio;
    public int PlayType;

    [Header("Audio")]
    [SerializeField] private GameObject[] GO;
    [SerializeField] private GameObject[] Hold;
    [SerializeField] private GameObject[] HoldOH;
    [SerializeField] private GameObject[] OH;
    [SerializeField] private GameObject[] Crowd;

    [Header("Activated Objects")]
    [SerializeField] private GameObject[] Scenes;
    [SerializeField] private GameObject StadiumCrowd;

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

    private GameObject HoldInstance;
    [SerializeField] private GameObject UICanvas;

    [Header("Metronome Variables")]
    [SerializeField] private S_Metronome_Audio Metronome;
    [SerializeField] private S_PerformanceStats_Stats StatStore;

    [Header("Animations")]
    [SerializeField] private Animator[] CrowdAnimation;
    [SerializeField] private Animator[] PlayerAnimation;

    [Header("Tutorial Variables")]
    [SerializeField] private bool TutorialStadium;
    [SerializeField] private TextMeshProUGUI TutorialText;
    private bool TutorialHold;
 

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
            StadiumCrowd.SetActive(true);
            TutorialText.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(StadiumTutorial());
            GameAudio.volume = 0;
            StadiumCrowd.SetActive(false);
            PlayType = 0;
            Scenes[0].SetActive(true);
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

            if(CrowdAnimation[PlayType].transform.parent.gameObject.activeSelf != false && CrowdAnimation[PlayType].gameObject.activeSelf != false)
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
                            if(PlayType == 0)Instantiate(Crowd[PlayType], GameAudio.transform);
                        }
                        else
                        {
                            float gapTime = RepeatCount == 2 ? Metronome.BPMperSecond : Metronome.BPMperSecond / 3;
                            if(PlayType == 0)StartCoroutine(RepeatCrowd(RepeatCount, gapTime));
                        }
                    }
                    else if (GameAudio.time >= TempTimings[0] - TimesByBPM(0.4f) && HoldReleases.Count != 0)
                    {
                        CrowdAnimation[PlayType].SetBool("JumpBeat", true);
                      
                        if(PlayType == 0) Instantiate(Crowd[PlayType], GameAudio.transform);
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
                Instantiate(OH[PlayType], GameAudio.transform);
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
                else if ((PressTime > TempTimings[0] - TimesByBPM(0.9f) && PressTime < TempTimings[0] - TimesByBPM(0.45f)) || (PressTime < TempTimings[0] + TimesByBPM(0.9f) && PressTime > TempTimings[0] + TimesByBPM(0.45f)))
                {
                    if (HoldReleases.Count != 0)
                    {
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
                    if (HoldReleases.Count != 0)
                    {
                        if (PlayType == 0) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                        HoldButton = true;
                    }
                }
                else
                {
                    if(StatStore.gameObject.activeSelf == true)StatStore.AddPoints(0);
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
                }
            }
            else if(Input.GetKeyUp(KeyCode.Space) && PlayerAnimation[0].name == "JumpStay") PlayerAnimation[PlayType].Play("RegularLocation");


            if (TempTimings.Count != 0)
            {
                if (GameAudio.time >= TempTimings[0] + TimesByBPM(1.2f))
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(0);
                    TempTimings.RemoveAt(0);
                }
            }
            if (HoldReleases.Count != 0)
            {
                if(GameAudio.time >= HoldReleases[0] && HoldInstance != null && PlayType == 1) Destroy(HoldInstance);
                if (GameAudio.time >= HoldReleases[0] + TimesByBPM(1.2f))
                {
                    if (StatStore.gameObject.activeSelf == true) StatStore.AddPoints(0);
                    HoldReleases.RemoveAt(0);
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
            if(TempTimings.Count == 0 && HoldReleases.Count != 0 && TutorialHold == true)
            {
                TutorialHold = false;
                StartCoroutine(CountdownUI(8));
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
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(2);
                if(PlayType == 0)Instantiate(Hold[PlayType], GameAudio.transform);
                if (PlayType == 1 && HoldInstance == null) HoldInstance = Instantiate(HoldOH[PlayType], GameAudio.transform);
                if (PlayType == 1)
                {
                    GameObject tempNote = Instantiate(ConcertNoteHold);
                    tempNote.GetComponent<S_HoldNote_Concert>().BeatTiming = f;
                    tempNote.GetComponent<S_HoldNote_Concert>().HoldLength = Timings[0].EndHold;
                    tempNote.GetComponent<S_HoldNote_Concert>().SetInstance(false);
                }
                HoldReleases.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond * Timings[0].EndHold));
                if (TutorialStadium == true && Timings[0].Ignore == false) StartCoroutine(CountdownUI(4));
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumTwo)
            {
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(2);
                StartCoroutine(PlayAudio(2, Metronome.BPMperSecond, false, Timings[0].Ignore, f));
                RepeatCount = 2;
                TempTimings.Add(Timings[0].BeatTiming + Metronome.BPMperSecond);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumThree)
            {
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(3);
                StartCoroutine(PlayAudio(3, Metronome.BPMperSecond / 3, false, Timings[0].Ignore, f));
                RepeatCount = 3;
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3));
                TempTimings.Add(Timings[0].BeatTiming + (Metronome.BPMperSecond / 3 * 2));
            }
            else
            {
                if (StatStore.gameObject.activeSelf == true) StatStore.AddtoTotal(1);
                Instantiate(GO[PlayType], GameAudio.transform);
                if(PlayType == 1)
                {
                    GameObject tempNote = Instantiate(ConcertNoteRegular);
                    tempNote.GetComponent<S_MusicNote_Concert>().BeatTiming = f;
                    tempNote.GetComponent<S_MusicNote_Concert>().InstantiateSetup();
                }
                if (TutorialStadium == true && Timings[0].Ignore == false) StartCoroutine(CountdownUI(4));
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
        float tempTime = timing / (Metronome.BPMperSecond/2);
        tempTime = Mathf.Round(tempTime);
        return tempTime * (Metronome.BPMperSecond/2);
    }

    private float RoundedInput(float timing)
    {
        return Mathf.Round(timing * 10) * 0.1f;
    }

    private IEnumerator PlayAudio(int loopAmount, float gap, bool justSound, bool ignore, float noteLength)
    {
        Instantiate(GO[PlayType], GameAudio.transform);
        if(PlayType == 1)
        {
            GameObject tempNote = Instantiate(ConcertNoteRegular);
            tempNote.GetComponent<S_MusicNote_Concert>().BeatTiming = noteLength;
            tempNote.GetComponent<S_MusicNote_Concert>().InstantiateSetup();
        }
        if (loopAmount - 1 <= 0 && TutorialStadium == true && ignore == false && justSound == false) StartCoroutine(CountdownUI(4));
        yield return new WaitForSeconds(gap);
        if (loopAmount - 1 > 0) StartCoroutine(PlayAudio(loopAmount - 1, gap, justSound, ignore, 0));
       
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
        StartCoroutine(PlayAudio(2, Metronome.BPMperSecond, true, false, 0));
        yield return new WaitForSecondsRealtime(3f);
        TutorialText.text = "That means there will be two waves in a row!\nPress space twice after 4 beats!\nLet's try that now";
        yield return new WaitForSecondsRealtime(11);
        TutorialText.text = "Great! And if you hear three noises...";
        yield return new WaitForSecondsRealtime(2.5f);
        StartCoroutine(PlayAudio(3, Metronome.BPMperSecond / 3, true, false, 0));
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
    private IEnumerator CountdownUI(int count)
    {
        TutorialText.text = "" + count;
        yield return new WaitForSecondsRealtime(Metronome.BPMperSecond);
        if (count - 1 > 0) StartCoroutine(CountdownUI(count - 1));
        else TutorialText.text = "";
    }
}
