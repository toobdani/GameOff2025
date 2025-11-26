using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MusicNote_Concert : MonoBehaviour
{
    public float BeatTiming;
    [SerializeField] private Vector3 StartPos, EndPos;
    [SerializeField] private float LerpTime;
    [SerializeField] private float LerpAddition;

    private float TimeWait;
    private bool Started;

    public void InstantiateSetup()
    {
        gameObject.transform.position = StartPos;
        LerpAddition = 1 / BeatTiming;
        Started = true;
    }
    private void FixedUpdate()
    {
        if (Started == false) return;
        if(gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == true)
        {
            gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
            LerpTime += LerpAddition;
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
