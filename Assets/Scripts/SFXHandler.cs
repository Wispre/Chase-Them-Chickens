using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXHandler : MonoBehaviour
{

    public AudioClip[] screams;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMinimaScream()
    {
        var chosenClip = screams[Random.Range(0, screams.Length)];

        audioSource.PlayOneShot(chosenClip);
    }
}
