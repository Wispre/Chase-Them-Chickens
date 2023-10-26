using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChickenMovement : MonoBehaviour
{
    public GameEvent OnChickenReachedHome;

    public Animator animator;

    enum State
    {
        casual,
        fear,
        droppedInCoop
    }

    private Coroutine Co_fear;

    private State state = State.casual;

    private NavMeshAgent agent;

    private float maxDistance = 5f;

    private float maxWaitTime = 15f;
    private float minWaitTime = 1f;

    private float timer = 0f;
    private bool useNavMesh = true;

    private Vector3 endTween;
    private float distanceTween = 5f;
    private WaitForSeconds randomDelay = new WaitForSeconds(0.5f);

    private Vector3 target;
    public void GoToHouse(Vector3 target)
    {
        state = State.droppedInCoop;
        endTween = target;
        agent.enabled = false;

        if (transform.position == endTween)
        {
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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

    public void InFear()
    {
        if (Co_fear == null) return;

        state = State.fear;
        Co_fear = StartCoroutine(ScareChicken());
    }

    IEnumerator ScareChicken()
    {
        yield return null;
        Co_fear = null;
    }

    private void CoopDeposit()
    {
        transform.position = Vector3.MoveTowards(transform.position, endTween, distanceTween * Time.deltaTime);

        if (Vector3.Distance(transform.position, endTween) <= 1f)
        {
            gameObject.SetActive(false);
            OnChickenReachedHome.Raise();
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
