using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSoundsManager : MonoBehaviour
{
    public List<AudioSource> musicSectionAudios;
    public void UIClick1()
    {
        musicSectionAudios[0].Play();
    }
    public void UIClick2()
    {
        musicSectionAudios[1].Play();
    }
    public void inGameMusic()
    {
        musicSectionAudios[2].Play();
    }
}
