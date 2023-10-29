using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChickenMovement : MonoBehaviour
{
    public GameEvent OnChickenReachedHome;
    public GameEvent OnChickenDeposit;

    public Animator animator;

    public enum State
    {
        casual,
        fear,
        droppedInCoop
    }

    public bool isCarried;

    public State state { get; private set; } = State.casual;

    private NavMeshAgent agent;

    private float maxDistance = 5f;

    private float maxWaitTime = 15f;
    private float minWaitTime = 1f;

    private float timer = 0f;

    private Vector3 endTween;
    private float distanceTween = 5f;
    private WaitForSeconds randomDelay = new WaitForSeconds(0.5f);

    private Vector3 target;
    private Vector3 scaryTarget;
    public void GoToHouse(Vector3 target)
    {
        state = State.droppedInCoop;
        endTween = target;
        agent.enabled = false;
    }

    void OnEnable()
    {
        state = State.casual;
        agent.enabled = true;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        transform.rotation = Utils.GetRandomRotationY();
    }

    private void Start()
    {
        SetNewTimer();

        StartCoroutine(RandomUpdater());
    }
    private void Update()
    {
        switch (state)
        {
            case State.casual:
                NavMeshCounter();
                break;

            case State.fear:
                InFear();
                break;

            case State.droppedInCoop:
                CoopDeposit();
                break;
        }
    }

    public void Scare(Vector3 scaryTarget)
    {
        state = State.fear;
        this.scaryTarget = scaryTarget;
        agent.destination = (transform.position - scaryTarget) * 5;
        animator.SetBool("isWalking", true);
        animator.SetTrigger("trigScared");
    }

    private void InFear()
    {
        if (agent.velocity.magnitude <= 0)
        {
            state = State.casual;
        }
    }


    private void CoopDeposit()
    {
        transform.position = Vector3.MoveTowards(transform.position, endTween, distanceTween * Time.deltaTime);

        if (Vector3.Distance(transform.position, endTween) <= 1f)
        {
            gameObject.SetActive(false);
            OnChickenReachedHome.Raise();
            isCarried = false;
            OnChickenDeposit.Raise();
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

        if (agent.velocity.magnitude <= 0f)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }

    IEnumerator RandomUpdater()
    {
        while (true)
        {
            yield return randomDelay;
            animator.SetInteger("random", Random.Range(0, 101));
        }
       
    }

    private void GetTarget()
    {
        float currentPosX = transform.position.y;
        float currentPosZ = transform.position.z;

        float getTargetX = Random.Range(currentPosX - maxDistance, currentPosX + maxDistance);
        float getTargetZ = Random.Range(currentPosZ - maxDistance, currentPosZ + maxDistance);

        target = new Vector3(getTargetX, 0f, getTargetZ);

        agent.destination = target;
    }

    private void SetNewTimer()
    {
        timer = Random.Range(minWaitTime, maxWaitTime);
    }
}
