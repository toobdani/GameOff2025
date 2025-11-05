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
    [SerializeField] private bool SetTimings;
    [SerializeField] private bool PutTimings;
    [SerializeField] private bool PlayRhythm;
    [SerializeField] private float PressTime;
    [SerializeField] private List<S_TimingClass_Class> Timings;
    [SerializeField] private bool HoldButton;

    [SerializeField] private List<float> TempTimings;
    [SerializeField] private List<float> HoldReleases;
    [SerializeField] private bool DontSpawn;


    [SerializeField] private GameObject TimingImage;

    private void Start()
    {
        GameAudio.clip = SongStats.Song;
        GameAudio.Play();
        if (PlayRhythm == false) return;
        foreach (S_TimingClass_Class tc in SongStats.Timings)
        {
            Timings.Add(tc);
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
            StoreRhytym();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(OH, GameAudio.transform);
                PressTime = GameAudio.time;


                if (TempTimings.Count == 0) StartCoroutine(TextShow("Early", false));
                else if ((PressTime > TempTimings[0] - 0.8 && PressTime < TempTimings[0] - 0.3) || (PressTime < TempTimings[0] + 0.8 && PressTime > TempTimings[0] + 0.3))
                {
                    if (HoldReleases.Count != 0) HoldButton = true;
                    StartCoroutine(TextShow("Nearly", false));
                }
                else if (PressTime > TempTimings[0] - 0.3 && PressTime < TempTimings[0] + 0.3)
                {
                    if (HoldReleases.Count != 0) HoldButton = true;
                    StartCoroutine(TextShow("Perfect", false));
                }
                else
                {
                    StartCoroutine(TextShow("Miss", false));
                }
            }
            if (Input.GetKeyUp(KeyCode.Space) && HoldButton == true)
            {
                HoldButton = false;
                DontSpawn = false;
                if (HoldReleases.Count == 0) StartCoroutine(TextShow("Early", true));
                else if ((PressTime > HoldReleases[0] - 0.8 && PressTime < HoldReleases[0] - 0.3) || (PressTime < HoldReleases[0] + 0.8 && PressTime > HoldReleases[0] + 0.3))
                {
                    StartCoroutine(TextShow("Nearly", true));
                }
                else if (PressTime > HoldReleases[0] - 0.3 && PressTime < HoldReleases[0] + 0.3)
                {
                    StartCoroutine(TextShow("Perfect", true));
                }
                else
                {
                    StartCoroutine(TextShow("Miss", true));
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
                TempTimings.Add(Timings[0].BeatTiming + 0.17067f);
            }
            else if (Timings[0].ControlType == S_TimingTypeEnum_Enum.StadiumThree)
            {
                Instantiate(GOGOGO, GameAudio.transform);
                TempTimings.Add(Timings[0].BeatTiming + 0.2f);
                TempTimings.Add(Timings[1].BeatTiming + 0.4f);
            }
            else Instantiate(GO, GameAudio.transform);
            Timings.RemoveAt(0);
        }
    }

    private IEnumerator TextShow(string text, bool isHold)
    {
        TimingImage.SetActive(true);
        switch(text)
        {
            case "Nearly":
                TimingImage.GetComponent<Image>().color = Color.yellow;
                if (isHold == false) TempTimings.RemoveAt(0);
                else HoldReleases.RemoveAt(0);
                break;
            case "Early":
                TimingImage.GetComponent<Image>().color = Color.black;
                break;
            case "Perfect":
                TimingImage.GetComponent<Image>().color = Color.green;
                if (isHold == false) TempTimings.RemoveAt(0);
                else HoldReleases.RemoveAt(0);

                break;
            case "Miss":
                TimingImage.GetComponent<Image>().color = Color.red;
                if (isHold == false) TempTimings.RemoveAt(0);
                else HoldReleases.RemoveAt(0);
                break;
        }
  

        yield return new WaitForSeconds(0.05f);
        TimingImage.SetActive(false);
    }
}
