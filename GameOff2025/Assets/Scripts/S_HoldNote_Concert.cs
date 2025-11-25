using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HoldNote_Concert : MonoBehaviour
{
    public float BeatTiming;
    public GameObject LaserSpawnPoint;
    public bool SecondNote;

    [SerializeField] private Vector3 StartPos, EndPos;
    [SerializeField] private float LerpTime;
    [SerializeField] private float LerpAddition;
    [SerializeField] private LineRenderer HoldLaser;

    [SerializeField] private GameObject HoldPrefab;

    private float TimeWait;

    private void Awake()
    {
        switch(SecondNote)
        {
            case true:
                LerpAddition = 1 / BeatTiming;
                break;

            case false:
                LerpAddition = 1 / BeatTiming;
                LerpAddition = 1 - (BeatTiming * 4);
                break;
        }
       
    }
    private void FixedUpdate()
    {
        if (gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == true)
        {
            gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
            LerpTime += LerpAddition;
        }
        LerpTime = Mathf.Clamp(LerpTime, 0, 1);
        gameObject.transform.position = Vector3.Lerp(StartPos, EndPos, LerpTime);
        if (LerpTime >= 1)
        {
            if (TimeWait >= 1)
            {
                foreach(SpriteRenderer s in gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    s.color = new Color(1, 1, 1, 0);
                }
            }
            else TimeWait += 0.1f;
        }

        if (HoldLaser == null) return;
        HoldLaser.SetPosition(0, LaserSpawnPoint.transform.position);
        HoldLaser.SetPosition(1, StartPos);

    }
}
