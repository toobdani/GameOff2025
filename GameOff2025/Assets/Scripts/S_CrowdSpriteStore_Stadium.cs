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
    [SerializeField] private GameObject CrowdSprites;



    public IEnumerator WaveUp(float waitTime, bool hold, float loopCount)
    {
        foreach(SpriteRenderer s in CrowdSprites.GetComponentsInChildren<SpriteRenderer>())
        {
            s.sprite = s.GetComponent<S_CrowdSpriteStore_Stadium>().HandsUp;
        }
        yield return new WaitForSecondsRealtime(waitTime);
        foreach (SpriteRenderer s in CrowdSprites.GetComponentsInChildren<SpriteRenderer>())
        {
            s.sprite = s.GetComponent<S_CrowdSpriteStore_Stadium>().HandsDown;
        }
        if(loopCount != 0)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            StartCoroutine(WaveUp(waitTime, hold, loopCount - 1));
        }
    }
}
