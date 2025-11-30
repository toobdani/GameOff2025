using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CallAnimationBool_System : MonoBehaviour
{
    public void RemoveAnimationBool()
    {
        gameObject.GetComponent<Animator>().SetBool("GettingReady", false);
    }
}
