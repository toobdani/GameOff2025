using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AudioInstance_Prefab : MonoBehaviour
{
    [SerializeField] private float PauseTime = 2;
    [SerializeField] private float StartCount;
    void Start()
    {
        StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(PauseTime);
        Destroy(gameObject);
    }
}
