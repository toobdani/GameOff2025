using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SongManager_System : MonoBehaviour
{
    public S_SongStats_SerliazableObject SongStats;
    public AudioSource GameAudio;

    private void Start()
    {
        GameAudio.clip = SongStats.Song;
        GameAudio.Play();
    }
}
