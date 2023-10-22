using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChickenMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    private float maxDistance = 5f;

    private float maxWaitTime = 15f;
    private float minWaitTime = 1f;

    private float timer = 0f;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetNewTimer();
    }
    private void Update()
    {
        if (timer > 0f) 
        {
            timer -= Time.deltaTime;
        }
        else
        {
            GetTarget();
            SetNewTimer();
        }
    }

    private void GetTarget()
    {
        float currentPosX = transform.position.y;
        float currentPosZ = transform.position.z;

        float getTargetX = Random.Range(currentPosX - maxDistance, currentPosX + maxDistance);
        float getTargetZ = Random.Range(currentPosZ - maxDistance, currentPosZ + maxDistance);

        agent.destination = new Vector3(getTargetX, 0f, getTargetZ);
    }

    private void SetNewTimer()
    {
        timer = Random.Range(minWaitTime, maxWaitTime);
    }
}
