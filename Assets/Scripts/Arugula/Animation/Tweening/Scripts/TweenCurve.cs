using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TweenCurve.asset", menuName = "Tween Curve")]
public class TweenCurve : ScriptableObject
{
    public AnimationCurve curve;

    [ContextMenu("Flip")]
    void Invert()
    {
        Keyframe[] original = curve.keys;

        foreach (var k in original)
        {
            //if (k.time < 0 || k.time > 1 || k.value < 0 || k.value > 1)
            //{
            //    Debug.LogWarning(k.ToString() + " out of bounds!  Ensure 0-1 range for Time and Value");
            //    return;
            //}
        }

        Keyframe[] flipped = new Keyframe[original.Length];

        for (int i = 0; i < flipped.Length; i++)
        {
            var k = original[i];
            k.time = 1f - k.time;

            k.inTangent = original[i].outTangent;
            k.outTangent = original[i].inTangent;
            k.value = 1f - k.value;
            flipped[i] = k;
        }

        curve.keys = flipped;
    }
}
