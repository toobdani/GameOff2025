using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Metronome_Audio : MonoBehaviour
{
    public float BPM = 89F;
    public float BPMperSecond => 60 / BPM;
    [SerializeField] private double NextBeat;
    [SerializeField] private AudioSource Audio;
    [SerializeField] private int Count;
    [SerializeField] private AudioClip MainMetronome;
    private bool CalledThird1;
    private bool CalledThird2;
    void Start()
    {
        Debug.Log(BPMperSecond);
        NextBeat = AudioSettings.dspTime + BPMperSecond;
        Audio = gameObject.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if(AudioSettings.dspTime > (NextBeat - (BPMperSecond/3)) && CalledThird1 == false)
        {
            CalledThird1 = true;
            if (FindObjectsOfType<S_HalfBeatSignal_System>() != null)
            {
                foreach (S_HalfBeatSignal_System bs in FindObjectsOfType<S_HalfBeatSignal_System>())
                {
                    bs.BeatChanged = true;
                }
            }
        }
        else if (AudioSettings.dspTime > (NextBeat - ((BPMperSecond / 3) * 2)) && CalledThird2 == false)
        {
            CalledThird2 = true;
            if (FindObjectsOfType<S_HalfBeatSignal_System>() != null)
            {
                foreach (S_HalfBeatSignal_System bs in FindObjectsOfType<S_HalfBeatSignal_System>())
                {
                    bs.BeatChanged = true;
                }
            }
        }
        if (AudioSettings.dspTime < NextBeat) return;
        Audio.Play();
        NextBeat += BPMperSecond;
        CalledThird1 = false;
        CalledThird2 = false;
        if(FindObjectsOfType<S_BeatSignal_System>() != null)
        {
            foreach(S_BeatSignal_System bs in FindObjectsOfType<S_BeatSignal_System>())
            {
                bs.BeatChanged = true;
            }
        }
        if (Count > 2 && Audio.clip != MainMetronome) Audio.clip = MainMetronome;
        Count++;
    }
}
