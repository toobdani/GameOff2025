using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class S_PerformanceStats_Stats : MonoBehaviour
{
    public float TotalPoints;
    public float CurrentPoints;
    public int TotalMisses;
    public int TotalGood;
    public int TotalPerfect;

    [SerializeField] private TextMeshProUGUI PerfectCount;
    [SerializeField] private TextMeshProUGUI GoodCount;
    [SerializeField] private TextMeshProUGUI MissCount;
    [SerializeField] private TextMeshProUGUI PerfectPercentage;
    // Update is called once per frame
    void Update()
    {
        PerfectCount.text = "" + TotalPerfect;
        GoodCount.text = "" + TotalGood;
        MissCount.text = "" + TotalMisses;
        if (TotalPoints == 0) return;
        PerfectPercentage.text = "" + Mathf.Round((((CurrentPoints / TotalPoints) * 100) * 10)) * 0.1 + "%";

    }

    public void AddtoTotal(int amount) => TotalPoints += (1.5f * amount);

    public void AddPoints(float points)
    {
        CurrentPoints += points;
        switch(points)
        {
            case 0:
                TotalMisses += 1;
                break;
            case 1:
                TotalGood += 1;
                break;
            case 1.5f:
                TotalPerfect += 1;
                break;
        }
    }
}
