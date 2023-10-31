using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rooster : MonoBehaviour, IWorkInGameplay
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

    private Coroutine Co_Scream;
    private WaitForSeconds screamCD = new WaitForSeconds(20);

    private bool roosterNotAngered = true;

    private bool inGameplay = true;

    public void AngerRooster()
    {
        if (roosterNotAngered && Vector3.Distance(player.transform.position, transform.position) <= 6)
        {
            state = State.Anger;
            roosterNotAngered = false;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player.position;

        state = State.Idle;
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
            StopDustParticles();


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
            Co_Scream = StartCoroutine(ScreamCooldown());
            hasNotScreamed = false;
            animator.SetTrigger("trigScared");
        }

        if (currentAngerDuration <= 0f)
        {
            currentAngerDuration = angerDuration;
            PrepareToCharge();
        }
        else if(agent.velocity.magnitude <= 0)
        {
            currentAngerDuration -= Time.deltaTime;
            transform.LookAt(player);
            agent.speed = 10f;
            agent.acceleration = 80f;
        }
    }

    IEnumerator ScreamCooldown()
    {
        sfxSource.PlayOneShot(PreparingToCharge);

        yield return screamCD;
        hasNotScreamed = true;
        Co_Scream = null;
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

        if (agent.velocity.magnitude <= 0)
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
        if(agent.velocity.magnitude <= 0)
        {
            state = State.Anger;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!inGameplay) { return; }

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

    public void StartWorking()
    {
        inGameplay = true;
    }

    public void StopWorking()
    {
        inGameplay = false;
        state = State.Idle;
        roosterNotAngered = true;
        agent.speed = 3.5f;
        agent.acceleration = 8f;
        roosterNotAngered = true;
        timer = 0f;
        currentAngerDuration = 1f;

    }
}
