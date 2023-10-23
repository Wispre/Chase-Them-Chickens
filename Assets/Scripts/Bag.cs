using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bag : MonoBehaviour
{
    private PlayerCollect playerCollect;
    private List<GameObject> chickens = new List<GameObject>();

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

}
