using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rooster : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip PreparingToCharge;
    public Animator animator;
    public ParticleSystem dustParticles;

    enum State
    {
        Idle,
        Anger,
        Charge,
        Recover
    }

    [Range(0,1)]
    private float percentToRelease = 1f; 

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

    private Vector3 restingSpot;

    private bool hasNotScreamed = true;
    private bool isCharging = false;

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

        if (agent.velocity.magnitude <= 0f)
        {
            animator.SetBool("isWalking", false);
            if (!isCharging)
            {
                StopDustParticles();
            }

        }
        else
        {
            animator.SetBool("isWalking", true);

            if (isCharging)
            {
                PlayDustParticles();
            }
            
        }
    }

    private void Idle()
    {
        NavMeshCounter();
    }

    private void Anger()
    {
        if (hasNotScreamed)
        {
            sfxSource.PlayOneShot(PreparingToCharge);
            hasNotScreamed = false;
            animator.SetTrigger("trigScared");
        }

        if (currentAngerDuration <= 0f)
        {
            currentAngerDuration = angerDuration;
            PrepareToCharge();
            hasNotScreamed = true;
        }
        else
        {
            currentAngerDuration -= Time.deltaTime;
            transform.LookAt(player);
            //agent.speed = 20f;
        }
    }

    private void PrepareToCharge()
    {
        state = State.Charge;
        target = player.position;
        
    }
    private void Charge()
    {
        isCharging = true;
        agent.destination = target;

        if (Vector3.Distance(transform.position, target) <= 1)
        {
            state = State.Anger;
            isCharging = false;
        }
    }

    private void PlayDustParticles()
    {
        if (!dustParticles.isEmitting)
        {
            dustParticles.Play();
        }
    }

    private void StopDustParticles()
    {
        if(dustParticles.isEmitting)
        {
            dustParticles.Stop();
        }
    }


    private void Recover()
    {
        restingSpot.y = 0f;
        if(Vector3.Distance(transform.position, restingSpot) <= 0.1f)
        {
            state = State.Anger;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            FindRestingSpot(collision.transform.position);
            state = State.Recover;
            PunishPlayer(collision.collider.gameObject);
        }

    }

    private void PunishPlayer(GameObject player)
    {
        player.transform.parent.GetComponent<PlayerMovement>().GetHit();
        player.transform.parent.GetComponent<Bag>().ForceRelease(percentToRelease);
    }

    private void FindRestingSpot(Vector3 playerSpot)
    {
        var direction = (transform.position - playerSpot).normalized;
        restingSpot = direction * 10;
        agent.destination = restingSpot;
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
