using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Metronome_Audio : MonoBehaviour
{
    public float BPM = 89F;
    [SerializeField] private float BPMperSecond;
    [SerializeField] private double NextBeat;
    [SerializeField] private AudioSource Audio;
    [SerializeField] private int Count;
    [SerializeField] private AudioClip MainMetronome;
    void Start()
    {
        BPMperSecond = 60 / BPM;
        NextBeat = AudioSettings.dspTime + BPMperSecond;
        Audio = gameObject.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (AudioSettings.dspTime < NextBeat) return;
        Audio.Play();
        NextBeat += BPMperSecond;
        if (Count > 2 && Audio.clip != MainMetronome) Audio.clip = MainMetronome;
        Count++;
    }
}
