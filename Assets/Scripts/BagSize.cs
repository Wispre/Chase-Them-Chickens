using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagSize : MonoBehaviour, IWorkInGameplay
{
    private SkinnedMeshRenderer bagMesh;

    private float sizeChangeAmount = 20f;

    private int timesMediumIncreased = 0;
    private int timesSmallIncreased = 0;
    private int timesOverIncreased = 0;

    void Awake()
    {
        bagMesh = GetComponent<SkinnedMeshRenderer>();
    }

    public void MakeBagBigger()
    {
        var medium = bagMesh.GetBlendShapeWeight(1);
        var small = bagMesh.GetBlendShapeWeight(2);

        if (small > 0)
        {
            bagMesh.SetBlendShapeWeight(2, small - sizeChangeAmount);
            timesSmallIncreased++;
        }
        else if (medium > 0)
        {
            bagMesh.SetBlendShapeWeight(1, medium - sizeChangeAmount);
            timesMediumIncreased++;
        }
        else
        {
            timesOverIncreased++;
        }
    }

    public void MakeBagSmaller()
    {
        var medium = bagMesh.GetBlendShapeWeight(1);
        var small = bagMesh.GetBlendShapeWeight(2);

        if (timesOverIncreased > 0)
        {
            timesOverIncreased--;
        }
        else if (medium < 100)
        {
            bagMesh.SetBlendShapeWeight(1, medium + sizeChangeAmount);
            timesMediumIncreased--;
        }
        else if (small < 100)
        {
            bagMesh.SetBlendShapeWeight(2, small + sizeChangeAmount);
            timesSmallIncreased--;
        }
    }

    public void MakeBagSmallerInstant()
    {
        timesOverIncreased = 0;
        timesMediumIncreased = 0;
        timesSmallIncreased = 0;

        bagMesh.SetBlendShapeWeight(1, 100);
        bagMesh.SetBlendShapeWeight(2, 100);
    }

    public void StartWorking()
    {
        MakeBagSmallerInstant();
    }

    public void StopWorking()
    {
    }
}
