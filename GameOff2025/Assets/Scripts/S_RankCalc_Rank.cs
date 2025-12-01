using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RankCalc_Rank : MonoBehaviour
{
    public GameObject P;
    public GameObject S;
    public GameObject A;
    public GameObject B;
    public GameObject C;
    public GameObject F;
    public void ShowRank(double percentage)
    {
        P.SetActive(false);
        S.SetActive(false);
        A.SetActive(false);
        B.SetActive(false);
        C.SetActive(false);
        if (percentage == 100) P.SetActive(true);
        else if (percentage <= 99 && percentage >= 80) S.SetActive(true);
        else if (percentage <= 79 && percentage >= 65) A.SetActive(true);
        else if (percentage <= 64 && percentage >= 50) B.SetActive(true);
        else if (percentage <= 49 && percentage >= 25) C.SetActive(true);
        else if (percentage <= 24) F.SetActive(true);
    }
}
