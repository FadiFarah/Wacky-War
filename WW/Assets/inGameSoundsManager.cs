using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inGameSoundsManager : MonoBehaviour
{
    public List<AudioSource> inGameSectionAudios;
    public void LeftFootStep()
    {
        inGameSectionAudios[0].Play();
    }
    public void RightFootStep()
    {
        inGameSectionAudios[1].Play();
    }
    public void LadderLeftFootStep()
    {
        inGameSectionAudios[2].Play();
    }
    public void LadderRightFootStep()
    {
        inGameSectionAudios[3].Play();
    }
    public void ShootSound()
    {
        inGameSectionAudios[4].Play();
    }
    public void SlideSound()
    {
        inGameSectionAudios[5].Play();
    }
    public void ZipLineSound()
    {
        inGameSectionAudios[6].Play();
    }
    public void DeathSound()
    {
        inGameSectionAudios[7].Play();
    }
    public void JumpSound()
    {
        inGameSectionAudios[8].Play();
    }
    public void HitPlayerSound()
    {
        inGameSectionAudios[9].Play();
    }
    public void HitObjectSound()
    {
        inGameSectionAudios[10].Play();
    }
}
