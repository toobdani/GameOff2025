using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CrowdTiming_Stadium : MonoBehaviour
{
    public bool BeginAnimation;
    [SerializeField] private SpriteRenderer[] Crowd;
    [SerializeField]private Sprite CrowdReady;
    private int SpriteCount;

    private void Update()
    {
        if (BeginAnimation == false) return;
        if (gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == false) return;
        gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
        Crowd[SpriteCount].sprite = CrowdReady;
        SpriteCount++;
        if (SpriteCount <= 3) return;
        SpriteCount = 0;
        BeginAnimation = false;
    }
}
