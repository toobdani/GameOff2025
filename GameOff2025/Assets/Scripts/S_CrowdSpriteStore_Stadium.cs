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
            case 0:
                Crowds[0].HandsUp = true;
                Crowds[0].DoneAnimation = true;
                break;
            case 1:
                Crowds[1].HandsUp = true;
                Crowds[1].DoneAnimation = true;
                break;
            case 2:
                Crowds[0].HandsDown = true;
                Crowds[2].HandsUp = true;
                Crowds[2].DoneAnimation = true;
                Crowds[3].HandsUp = true;
                Crowds[3].DoneAnimation = true;
                break;
            case 3:
                Crowds[1].HandsDown = true;
                Crowds[4].BeginAnimation = true;
                break;
            case 4:
                Crowds[2].HandsDown = true;
                Crowds[3].HandsDown = true;
                Crowds[4].DoneAnimation = false;
                break;
        }
        gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
       

        if(BeatCount == 4)
        {
            StartedWave = false;
            BeatCount = 0;
        }
        else BeatCount++;
    }

    public void StartWaves()
    {
        Crowds[0].HandsUp = true;
        Crowds[0].DoneAnimation = true;


        StartedWave = true;
    }
}
