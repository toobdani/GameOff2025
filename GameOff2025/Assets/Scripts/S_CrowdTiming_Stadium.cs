using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CrowdTiming_Stadium : MonoBehaviour
{
    public bool BeginAnimation;
    public bool DoneAnimation;
    public bool HandsUp;
    public bool HandsDown;
    [SerializeField] private GameObject Crowd;
    private int SpriteCount;


    private void Update()
    {
        if (BeginAnimation == true && DoneAnimation == false)
        {
            foreach (SpriteRenderer s in Crowd.GetComponentsInChildren<SpriteRenderer>())
            {
                s.sprite = s.GetComponent<S_CrowdSpriteStore_Stadium>().ReadySprite;
            }
            BeginAnimation = false;
            DoneAnimation = true;
        }
        if(HandsUp == true && DoneAnimation == true)
        {
            foreach (SpriteRenderer s in Crowd.GetComponentsInChildren<SpriteRenderer>())
            {
                s.sprite = s.GetComponent<S_CrowdSpriteStore_Stadium>().HandsUp;
            }
            HandsUp = false;
        }
        if(HandsDown == true && DoneAnimation == true)
        {
            Debug.LogError(gameObject.name);
            foreach (SpriteRenderer s in Crowd.GetComponentsInChildren<SpriteRenderer>())
            {
                s.sprite = s.GetComponent<S_CrowdSpriteStore_Stadium>().HandsDown;
            }
            HandsDown = false;
            DoneAnimation = false;
        }
    }
}
