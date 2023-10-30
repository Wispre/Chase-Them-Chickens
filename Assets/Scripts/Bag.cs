using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Bag : MonoBehaviour
{
    public GameEvent OnChickenReleased;

    public Transform Entrance;
    public Transform ThrowStart;

    private PlayerCollect playerCollect;
    private List<GameObject> chickens = new List<GameObject>();

    private WaitForSeconds delay = new WaitForSeconds(0.25f);

    public int GetCount()
    {
        return chickens.Count;
    }
    public void Release(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (chickens.Count <= 0) { return; }

            var chicken = chickens[0];
            chicken.GetComponent<ChickenMovement>().isCarried = false;

            chicken.SetActive(true);
            chicken.GetComponent<NavMeshAgent>().Warp(this.transform.position);
            chicken.transform.rotation = Utils.GetRandomRotationY();
            chickens.Remove(chicken);
            OnChickenReleased.Raise();
        }
    }

    public void ForceRelease(float percentage)
    {
        var amountToRelease = chickens.Count * percentage;

        for(int i = 0; i < amountToRelease; i++)
        {
            var chicken = chickens[0];
            chicken.SetActive(true);
            chicken.GetComponent<NavMeshAgent>().Warp(GetRandomSpotAroundPlayer(transform.position, 2f));

            var movementScript = chicken.GetComponent<ChickenMovement>();
            movementScript.Scare(transform.position);
            movementScript.isCarried = false;

            chickens.Remove(chicken);
            OnChickenReleased.Raise();
        }
    }

    private void Awake()
    {
        playerCollect = GetComponent<PlayerCollect>();
    }

    private void OnEnable()
    {
        playerCollect.OnGrabChicken += Increase;
    }

    private void OnDisable()
    {
        playerCollect.OnGrabChicken -= Increase;
    }

    private void Increase(GameObject chicken)
    {
        chickens.Add(chicken);
        chicken.GetComponent<ChickenMovement>().isCarried = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChickenHouse")
        {
            StartCoroutine(ReleaseChickens());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ChickenHouse")
        {
            StopAllCoroutines();
        }
    }

    IEnumerator ReleaseChickens()
    {
        while (chickens.Count > 0)
        {
            var currentChicken = chickens[0];
            currentChicken.SetActive(true);
            currentChicken.transform.position = ThrowStart.position;
            GiveRandomRotation(currentChicken);
            currentChicken.GetComponent<ChickenMovement>().GoToHouse(Entrance.position);
            chickens.Remove(currentChicken);
            yield return delay;
        }
    }

    private void GiveRandomRotation(GameObject chicken)
    {
        chicken.transform.rotation = Random.rotation;
    }

    private Vector3 GetRandomSpotAroundPlayer(Vector3 playerPos, float radius)
    {
        var spotInCircle = Random.insideUnitCircle.normalized * radius;

        return new Vector3(spotInCircle.x, 0f, spotInCircle.y) + playerPos;
    }

}
