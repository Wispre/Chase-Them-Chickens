using UnityEngine;
using UnityEngine.AI;

public class ChickenMovement : MonoBehaviour
{
    public GameEvent OnChickenReachedHome;

    public Animator animator;

    private NavMeshAgent agent;

    private float maxDistance = 5f;

    private float maxWaitTime = 15f;
    private float minWaitTime = 1f;

    private float timer = 0f;
    private bool useNavMesh = true;

    private Vector3 endTween;
    private float distanceTween = 5f;

    private Vector3 target;
    public void GoToHouse(Vector3 target)
    {
        useNavMesh = false;
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
    }
    private void Update()
    {
        if (useNavMesh)
        {
            NavMeshCounter();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, endTween, distanceTween * Time.deltaTime);

            if (Vector3.Distance(transform.position, endTween) <= 1f)
            {
                gameObject.SetActive(false);
                OnChickenReachedHome.Raise();
            }
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

        if (Vector3.Distance(transform.position, target) <= 0f)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
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
