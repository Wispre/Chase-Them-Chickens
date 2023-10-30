using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Quaternion GetRandomRotationY()
    {
        return Quaternion.Euler(0f, Random.Range(0, 360), 0f);
    }
}
