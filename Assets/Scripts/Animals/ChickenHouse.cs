using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenHouse : MonoBehaviour
{
    public ParticleSystem Particles;
    [Header("Audio")]
    public AudioSource Audio;
    public AudioClip DepositClip;

    public void ActivateParticleBurst()
    {
        Particles.Emit(1);
        Particles.Play();
    }

    public void PlayDepositSound()
    {
        Audio.PlayOneShot(DepositClip);
    }
}
