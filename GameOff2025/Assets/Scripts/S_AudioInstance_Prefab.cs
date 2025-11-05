using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AudioInstance_Prefab : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
