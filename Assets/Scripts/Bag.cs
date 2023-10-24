using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bag : MonoBehaviour
{
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

            chicken.SetActive(true);
            chicken.transform.position = this.transform.position;
            chickens.Remove(chicken);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChickenHouse")
        {
            StartCoroutine(ReleaseChickens(other.transform.parent.position));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ChickenHouse")
        {
            StopAllCoroutines();
        }
    }

    IEnumerator ReleaseChickens(Vector3 target)
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
        var x = Random.Range(0f, 360f);
        var y = Random.Range(0f, 360f);
        var z = Random.Range(0f, 360f);

        chicken.transform.rotation = Quaternion.Euler(x, y, z);
    }

}
