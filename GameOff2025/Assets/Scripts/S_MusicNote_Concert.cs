using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MusicNote_Concert : MonoBehaviour
{
    public float BeatTiming;
    //public float BPS;
    //public float StartTime;
    //public AudioSource GameAudio;

    [SerializeField] private Vector3 StartPos, EndPos;
    [SerializeField] private float LerpTime;
    [SerializeField] private float LerpAddition;

    private float TimeWait;
    private bool Started;
    private float NextBeat;
    private int BeatCount;

    public void InstantiateSetup(Color startColor, float xChange)
    {
        //if (GameAudio != null) NextBeat = StartTime + BPS;
        foreach (SpriteRenderer s in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            s.color = startColor;
        }
        StartPos = new Vector3(StartPos.x + xChange, StartPos.y, StartPos.z);
        LerpTime = 0;
        gameObject.transform.position = StartPos;
        LerpAddition = gameObject.GetComponent<S_HalfBeatSignal_System>() == null ? 1 / (BeatTiming): 1 / (BeatTiming * 3);
        Started = true;
    }
    private void FixedUpdate()
    {
        if(Started == false && gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == true)
        {
            gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
        }
        if (Started == false) return;
        if (gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == true && gameObject.GetComponent<S_HalfBeatSignal_System>() == null)
        {
            gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
            LerpTime += LerpAddition;
        }
        else if (gameObject.GetComponent<S_HalfBeatSignal_System>() != null)
        {
            if(gameObject.GetComponent<S_HalfBeatSignal_System>().BeatChanged == true)
            {
                gameObject.GetComponent<S_HalfBeatSignal_System>().BeatChanged = false;
                LerpTime += LerpAddition;
            }
            if(gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == true)
            {
                gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
                LerpTime += LerpAddition;
            }
        }
        LerpTime = Mathf.Clamp(LerpTime, 0, 1);
        gameObject.transform.position = Vector3.Lerp(StartPos, EndPos, LerpTime);
        if(LerpTime >= 1)
        {
            if (TimeWait >= 1) Destroy(gameObject);
            else TimeWait += 0.1f;
        }
    }
}
