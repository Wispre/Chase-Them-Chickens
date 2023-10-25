using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCollect : MonoBehaviour
{
    public Action<GameObject> OnGrabChicken;

    public float radius;
    [Range(0,360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    private List<GameObject> closeChickens = new List<GameObject>();

    public void Catch(InputAction.CallbackContext ctx)
    {
        if (ctx.started) 
        {
            LookForChicken();
        }
    }

    private void LookForChicken()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length == 0) return;

        closeChickens.Clear();

        for (int i = 0; i < rangeChecks.Length; i++)
        {
            Transform target = rangeChecks[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceTotarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceTotarget, obstructionMask))
                {
                    closeChickens.Add(rangeChecks[i].gameObject);
                }
            }
        }
        var chicken = GrabClosestChicken();
        OnGrabChicken?.Invoke(chicken);
        chicken.SetActive(false);
    }

    private GameObject GrabClosestChicken()
    {
        int index = 0;
        float lastDistance;

        lastDistance = Vector3.Distance(transform.position, closeChickens[0].transform.position);

        for (int i = 1; i < closeChickens.Count; i++)
        {
            var currentDistance = Vector3.Distance(transform.position, closeChickens[i].transform.position);


            if (currentDistance < lastDistance)
            {
                index = i;
                lastDistance = currentDistance;
            }
        }

        return closeChickens[index];
    }
}
