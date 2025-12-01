using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Song Save Stats", menuName = "SongStats/Save Info", order = 1)]
public class S_SongScriptableObject_System : ScriptableObject
{
    public S_SongStats_SerliazableObject LevelSong;
    public float ConcertPoints;
    public float StadiumPoints;
    public bool Concert;
}
