using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ChickenSpawner : MonoBehaviour
{
    public NavMeshAgent ChickenPrefab;
    public GameObject Parent;

    private WaitForSeconds delayPerSpawn = new WaitForSeconds(5);
    private void Start()
    {
        StartCoroutine(SpawnChickens());
    }

    IEnumerator SpawnChickens()
    {
        while (true)
        {
            var chicken = Instantiate(ChickenPrefab, Vector3.zero, Utils.GetRandomRotationY());
            chicken.Warp(GrabSpawnPoint());
            chicken.transform.parent = Parent.transform;
            yield return delayPerSpawn;
        }
    }

    private Vector3 GrabSpawnPoint()
    {
        var x = Random.Range(-19f, 25f);
        var z = Random.Range(-11f,13f);

        return new Vector3(x, 0f, z);
    }
}
