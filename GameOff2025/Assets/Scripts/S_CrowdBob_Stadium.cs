using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CrowdBob_Stadium : MonoBehaviour
{
    private bool PutUp;
    private float OriginalY;
    private void Start()
    {
        OriginalY = gameObject.transform.position.y;
    }
    private void FixedUpdate()
    {
        if (gameObject.GetComponent<S_BeatSignal_System>().BeatChanged == false) return;
        gameObject.GetComponent<S_BeatSignal_System>().BeatChanged = false;
        PutUp = !(PutUp);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, PutUp ? OriginalY + 0.05f : OriginalY, gameObject.transform.position.z) ;
    }
}
