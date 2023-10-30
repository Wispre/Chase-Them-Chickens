using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Quaternion GetRandomRotationY()
    {
        return Quaternion.Euler(0f, Random.Range(0, 360), 0f);
    }

    public static Vector3 GetPointAroundNoY(Vector3 target, float radius)
    {
        var circlePoint = Random.insideUnitCircle.normalized * radius;
        var pointOffset = new Vector3(circlePoint.x, 0f, circlePoint.y);
        pointOffset.x += target.x;
        pointOffset.z += target.z;

        return pointOffset;
    }
}
