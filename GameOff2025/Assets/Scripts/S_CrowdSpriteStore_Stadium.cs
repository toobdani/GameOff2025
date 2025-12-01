using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CrowdSpriteStore_Stadium : MonoBehaviour
{
    public Sprite ReadySprite;
    public Sprite HandsUp;
    public Sprite HandsDown;

    public bool StartedWave;
    private int BeatCount;
    [SerializeField] private S_CrowdTiming_Stadium[] Crowds;

    private void Update()
    {
        if (gameObject.GetComponent<S_BeatSignal_System>() == null) return;
        if (StartedWave == false && gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == false)
        {
            gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
        }
        if (StartedWave == false) return;
        if (gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == false) return;

        switch(BeatCount)
        {
            case 1:
                Crowds[0].HandsUp = true;
                Crowds[1].BeginAnimation = true;
                break;
            case 2:
                Crowds[0].HandsDown = true;
                Crowds[1].HandsUp = true;
                Crowds[2].BeginAnimation = true;
                break;
            case 3:
                Crowds[1].HandsDown = true;
                Crowds[2].HandsUp = true;
                Crowds[3].BeginAnimation = true;
                break;
            case 4:
                Crowds[2].HandsDown = true;
                Crowds[3].DoneAnimation = false;
                break;
        }
        gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
        BeatCount++;

        if(BeatCount == 4)
        {
            StartedWave = false;
            BeatCount = 0;
        }
    }

    public void StartWaves()
    {
        Crowds[0].BeginAnimation = true;
        BeatCount = 1;
        StartedWave = true;
    }
}
