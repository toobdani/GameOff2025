using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private Image Battery;
    [SerializeField] private Sprite[] BatteryImages;
    // Update is called once per frame
    void Update()
    {
        PerfectCount.text = "" + TotalPerfect;
        GoodCount.text = "" + TotalGood;
        MissCount.text = "" + TotalMisses;
        if (TotalPoints == 0) return;
        PerfectPercentage.text = "" + CurrentPoints;
        int i = 0;
        double percentage = Mathf.Round((((CurrentPoints / TotalPoints) * 100) * 10)) * 0.1;
        if (TotalPoints == 0) i = 0;
        else
        {
            if (percentage >= 0 && percentage < 20) i = 0;
            else if (percentage >= 20 && percentage < 45) i = 1;
            else if (percentage >= 45 && percentage < 80) i = 2;
            else if (percentage >= 80 && percentage <= 100) i = 3;
        }

        Battery.sprite = BatteryImages[i];


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
