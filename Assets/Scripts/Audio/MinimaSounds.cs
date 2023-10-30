using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaSounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] Screams;
    public AudioClip[] Chicken;

    public void PlayMinimaScream()
    {
        var chosenScream = Screams[Random.Range(0, Screams.Length)];

        audioSource.clip = chosenScream;
        audioSource.Play();
    }

    public void PlayMinimaChicken()
    {
        var soundChance = Random.Range(0, 100);

        if (soundChance < 20)
        {
            if (!audioSource.isPlaying)
            {
                var chosenChicken = Chicken[Random.Range(0, Chicken.Length)];
                audioSource.clip = chosenChicken;
                audioSource.Play();
            }
        }
    }


}
