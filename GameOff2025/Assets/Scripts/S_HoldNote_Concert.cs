using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HoldNote_Concert : MonoBehaviour
{
    public GameObject OtherNote;
    public float BeatTiming = 4;
    public float HoldLength;
    public GameObject LaserSpawnPoint;
    public bool SecondNote;

    [SerializeField] private Vector3 StartPos, EndPos;
    [SerializeField] private float LerpTime;
    [SerializeField] private float LerpAddition;
    [SerializeField] private float EndSpawnTime;
    [SerializeField] private LineRenderer HoldLaser;

    [SerializeField] private GameObject HoldPrefab;

    private float TimeWait;
    private int BeatCount;
    private GameObject SpawnedHold;
    private bool Started;
    private void FixedUpdate()
    {
        if (Started == false) return;

        if (gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == true)
        {
            gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
            BeatCount++;
            LerpTime += LerpAddition;
        }
        LerpTime = Mathf.Clamp(LerpTime, 0, 1);
        gameObject.transform.position = Vector3.Lerp(StartPos, EndPos, LerpTime);
        if (LerpTime >= 1)
        {
            if (TimeWait >= 1 && SecondNote == false)
            {
                foreach(SpriteRenderer s in gameObject.GetComponentsInChildren<SpriteRenderer>())
                {
                    s.color = new Color(s.color.r, s.color.g, s.color.b, 0);
                }
            }
            else if(TimeWait >= 1 && SecondNote == true)
            {
                Destroy(OtherNote);
                Destroy(gameObject);
            }
            else TimeWait += 0.1f;
        }

        if(BeatCount >= EndSpawnTime && HoldPrefab != null && SecondNote == false)
        {
            SpawnedHold = Instantiate(HoldPrefab);
            SpawnedHold.GetComponent<S_HoldNote_Concert>().SecondNote = true;
            SpawnedHold.GetComponent<S_HoldNote_Concert>().OtherNote = this.gameObject;
            SpawnedHold.GetComponent<S_HoldNote_Concert>().BeatTiming = BeatTiming;
            SpawnedHold.GetComponent<S_HoldNote_Concert>().SetInstance(true);
            HoldPrefab = null;
        }

        if (SecondNote == true) return;
        HoldLaser.SetPosition(0, LaserSpawnPoint.transform.position);
        HoldLaser.SetPosition(1, HoldPrefab == null ? SpawnedHold.transform.position : StartPos);

    }

    public void SetInstance(bool second)
    {
        gameObject.transform.position = StartPos;
        switch(second)
        {
            case true:
                LerpAddition = 1 / BeatTiming;
                HoldLaser.enabled = false;
                LerpTime = 0;
                break;

            case false:
                LerpAddition = 1 / BeatTiming;
                EndSpawnTime = HoldLength - 1;
                break;
        }
        foreach (SpriteRenderer s in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            s.color = new Color(s.color.r, s.color.g, s.color.b, 1);
        }
        Started = true;
    }
}
