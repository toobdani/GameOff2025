using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongStats", menuName = "SongStats/Song Stats", order = 0)]
public class S_SongStats_SerliazableObject : ScriptableObject
{
    public AudioClip Song;
    public S_TimingClass_Class[] Timings;

    public List<float> TempTimings;
}
