using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class ChickenSpawner : MonoBehaviour
{
    public NavMeshAgent ChickenPrefab;
    public GameObject ChickenHolder;
    public Transform player;

    private float cameraLeft = 16f;
    private float cameraRight = 16f;
    private float cameraUp = 15f;
    private float cameraDown = 10f;

    private float landLeft = 18f;
    private float landRight = 25f;
    private float landUp = 14f;
    private float landDown = 11f;

    private int minChickensAvailable = 15;
    private int chickensAvailable = 0;

    private WaitForSeconds delayPerSpawn = new WaitForSeconds(0.1f);

    public void SpawnChicken()
    {
        if (chickensAvailable >= minChickensAvailable) return;

        var chicken = GrabInactiveChicken();

        if (chicken != null)
        {
            chicken.gameObject.SetActive(true);
            chicken.Warp(GrabSpawnPoint());
            chicken.transform.rotation = Utils.GetRandomRotationY();
            chicken.transform.parent = ChickenHolder.transform;
        }
        else
        {
            chicken = Instantiate(ChickenPrefab, Vector3.zero, Utils.GetRandomRotationY());
            chicken.Warp(GrabSpawnPoint());
            chicken.transform.parent = ChickenHolder.transform;
        }

        IncreaseChickensAvailableCount();
    }

    public void IncreaseChickensAvailableCount()
    {
        chickensAvailable++;
    }

    public void DecreaseChickensAvailableCount()
    {
        chickensAvailable--;
    }

    private void Start()
    {
        chickensAvailable = ChickenHolder.transform.childCount;

        StartCoroutine(SpawnChickens());
    }

    IEnumerator SpawnChickens()
    {
        while (chickensAvailable < minChickensAvailable)
        {
            SpawnChicken();
            chickensAvailable++;
            yield return delayPerSpawn;
        }
    }

    private NavMeshAgent GrabInactiveChicken()
    {
        for (int i = 0; i < ChickenHolder.transform.childCount; i++)
        {
            var child = ChickenHolder.transform.GetChild(i).gameObject;

            if (!child.activeInHierarchy && !child.GetComponent<ChickenMovement>().isCarried)
            {
                return child.GetComponent<NavMeshAgent>();
            }
        }

        return null;
    }

    private Vector3 GrabSpawnPoint()
    {
        var spaceLeft = landLeft - cameraLeft + player.position.x;
        var spaceRight = landRight - cameraRight - player.position.x;
        var spaceUp = landUp - cameraUp - player.position.z;
        var spaceDown = landDown - cameraDown + player.position.z;

        bool isLeftValid = spaceLeft > 0;
        bool isRightValid = spaceRight > 0;
        bool isUpValid = spaceUp > 0;
        bool isDownValid = spaceDown > 0;

        var xMin = 0f;
        var xMax = 0f;
        var zMin = 0f;
        var zMax = 0f;

        if (isLeftValid)
        {
            xMin = spaceLeft;
        }

        if (isRightValid)
        {
            xMax = spaceRight;
        }

        if (isUpValid)
        {
            zMax = spaceUp;
        }

        if(isDownValid)
        {
            zMin = spaceDown;
        }

        var x = Random.Range(-xMin, xMax);
        var z = Random.Range(-zMin, zMax);

        if (isLeftValid)
        {
            x -= cameraLeft - player.position.x;
        }
        else if (isRightValid)
        {
            x += cameraRight + player.position.x;
        }

        if (isUpValid)
        {
            z += cameraUp + player.position.z;
        }
        else if(isDownValid)
        {
            z -= cameraDown - player.position.z;
        }

        return new Vector3(x, 0f, z);
    }

}
