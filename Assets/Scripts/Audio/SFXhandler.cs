using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXhandler : MonoBehaviour
{
    public AudioClip[] Screams;

    public AudioSource audioSource;

    public void PlayMinimaScream()
    {
        var chosenScream = Screams[Random.Range(0, Screams.Length)];

        audioSource.PlayOneShot(chosenScream);

    }
}
