using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class ChickenSpawner : MonoBehaviour
{
    public NavMeshAgent ChickenPrefab;
    public GameObject Parent;
    public Transform player;

    private float cameraLeft = 16f;
    private float cameraRight = 16f;
    private float cameraUp = 15f;
    private float cameraDown = 10f;

    private float landLeft = 18f;
    private float landRight = 25f;
    private float landUp = 14f;
    private float landDown = 11f;

    private WaitForSeconds delayPerSpawn = new WaitForSeconds(5);

    private void Start()
    {
        StartCoroutine(SpawnChickens());
    }

    IEnumerator SpawnChickens()
    {
        while (true)
        {
            var chicken = GrabInactiveChicken();

            if(chicken != null)
            {
                chicken.gameObject.SetActive(true);
                chicken.Warp(GrabSpawnPoint());
                chicken.transform.rotation = Utils.GetRandomRotationY();
                chicken.transform.parent = Parent.transform;
            }
            else
            {
                chicken = Instantiate(ChickenPrefab, Vector3.zero, Utils.GetRandomRotationY());
                chicken.Warp(GrabSpawnPoint());
                chicken.transform.parent = Parent.transform;
            }

            yield return delayPerSpawn;
        }
    }

    private NavMeshAgent GrabInactiveChicken()
    {
        for (int i = 0; i < Parent.transform.childCount; i++)
        {
            var child = Parent.transform.GetChild(i).gameObject;

            if (!child.activeInHierarchy && !child.GetComponent<ChickenMovement>().isCarried)
            {
                return child.GetComponent<NavMeshAgent>();
            }
        }

        return null;
    }

    private Vector3 GrabSpawnPoint1()
    {
        var x = Random.Range(-19f, 25f);
        var z = Random.Range(-11f,13f);

        return new Vector3(x, 0f, z);
    }

    private Vector3 GrabSpawnPoint2()
    {
        var spaceLeft = landLeft - cameraLeft - Mathf.Abs(player.position.x);
        var spaceRight = cameraRight - landRight - Mathf.Abs(player.position.x);
        var spaceUp = landUp - cameraUp - Mathf.Abs(player.position.z);
        var spaceDown = cameraDown - landDown - Mathf.Abs(player.position.z);

        print(spaceLeft);

        bool isLeftValid = spaceLeft >= 0;
        bool isRightValid = spaceRight > 0;
        bool isUpValid = spaceUp >= 0;
        bool isDownValid = spaceDown > 0;

        var xMin = 0f;
        var xMax = 0f;
        var zMin = 0f;
        var zMax = 0f;

        if (isLeftValid)
        {
            xMin = spaceLeft;

            print($"left: {isLeftValid}");
        }

        if (isRightValid)
        {
            xMax = spaceRight;
            print($"right: {isRightValid}");
        }

        if (isUpValid)
        {
            zMax = spaceUp;
            print($"up: {isUpValid}");
        }

        if(isDownValid)
        {
            zMin = spaceDown;
            print($"down: {isDownValid}");
        }

        var x = Random.Range(xMin, xMax);
        var z = Random.Range(zMin, zMax);

        if (isLeftValid)
        {
            x -= cameraLeft;
        }
        else if (isRightValid)
        {
            x += cameraRight;
        }

        if (isUpValid)
        {
            z += cameraUp;
        }
        else if(isDownValid)
        {
            z -= cameraDown;
        }

        //print($"{x} 0 {z}");

        return new Vector3(x, 0f, z);
    }

}
