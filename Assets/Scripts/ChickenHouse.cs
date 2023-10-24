using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenHouse : MonoBehaviour
{
    public ParticleSystem particles;

    public void ActivateParticleBurst()
    {
        particles.Emit(1);
        particles.Play();
    }
}
