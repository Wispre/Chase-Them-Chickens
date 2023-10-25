using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rooster : MonoBehaviour
{
    enum State
    {
        Idle,
        Anger,
        Charge,
        Recover
    }

    private NavMeshAgent agent;
    private float maxDistance = 5f;

    private float maxWaitTime = 15f;
    private float minWaitTime = 1f;

    private float timer = 0f;
    private State state = State.Idle;

    private Transform player;
    private Vector3 target;

    private float angerDuration = 1f;
    private float currentAngerDuration = 1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player.position;

        state = State.Anger;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Anger:
                Anger();
                break;

            case State.Charge:
                Charge();
                break;

            case State.Recover:
                Recover();
                break;
        }
    }

    private void Idle()
    {
        NavMeshCounter();
    }

    private void Anger()
    {
        if (currentAngerDuration <= 0f)
        {
            currentAngerDuration = angerDuration;
            PrepareToCharge();
        }
        else
        {
            currentAngerDuration -= Time.deltaTime;
            transform.LookAt(player);
            agent.speed = 20f;
        }
    }

    private void PrepareToCharge()
    {
        state = State.Charge;
        target = player.position;
        
    }
    private void Charge()
    {
        agent.destination = target;

        if (Vector3.Distance(transform.position, target) <= 1)
        {
            state = State.Anger;
        }
    }

    private void Recover()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            //collision.collider.gameObject.GetComponent<PlayerMovement>().GetHit();
        }

    }

    private void NavMeshCounter()
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
