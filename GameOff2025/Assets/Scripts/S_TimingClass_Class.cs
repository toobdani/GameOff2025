using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_TimingClass_Class
{
    public S_TimingTypeEnum_Enum ControlType;
    public float BeatTiming;
    public float EndHold;
    public float ThirdTime;
    public float WarningCount = 4;
    public bool Ignore;
}
