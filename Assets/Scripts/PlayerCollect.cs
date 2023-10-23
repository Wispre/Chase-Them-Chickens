using System;
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

        for (int i = 0; i < rangeChecks.Length; i++)
        {
            Transform target = rangeChecks[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceTotarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceTotarget, obstructionMask))
                {
                    OnGrabChicken?.Invoke(rangeChecks[i].gameObject);
                    rangeChecks[i].gameObject.SetActive(false);
                }
            }
        }


    }
}
