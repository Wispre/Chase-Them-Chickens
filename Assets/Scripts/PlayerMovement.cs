using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public PlayerCollect collect;
    public Counter counter;

    private float speed = 7f;
    private float speedPenalty = 0.05f;

    private float turnSmoothTime = 0.01f;
    private float turnSmoothVelocity;

    private Rigidbody rb;
    private Vector3 movementInput;

    private Coroutine Co_Slow;

    private bool inGrabPenalty = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collect = GetComponent<PlayerCollect>();
    }

    private void OnEnable()
    {
        collect.OnGrabChicken += SlowPlayer;
    }

    private void OnDisable()
    {
        collect.OnGrabChicken -= SlowPlayer;
    }

    private float GetModifiedSpeed()
    {
        var originalSpeed = speed;

        if (inGrabPenalty)
        {
            originalSpeed *= 2;

        }

        var speedPercentage = (originalSpeed * speedPenalty);
        var modSpeed = originalSpeed - ( speedPercentage * counter.GetCount());
        var lessThanSlowestSpeed = modSpeed < speed - speed*(1 - speedPenalty);

        if (lessThanSlowestSpeed)
        {
            modSpeed = speed - speed * (1 - speedPenalty);
        }
        return modSpeed;
    }

    public void GetDirectionInput(InputAction.CallbackContext ctx)
    {
        movementInput.x = ctx.ReadValue<Vector2>().x;
        movementInput.z = ctx.ReadValue<Vector2>().y;
    }

    private void Move()
    {
        Vector3 MoveVector = movementInput * GetModifiedSpeed();
        rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);
    }

    private void Rotate()
    {
        if (movementInput.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementInput.x, movementInput.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));
        }   
    }
    private void Update()
    {
        Move();
        Rotate();
    }

    private void SlowPlayer(GameObject chicken)
    {
        if(Co_Slow == null)
        {
            Co_Slow = StartCoroutine(Co_SlowingPlayer());
        }
    }

    IEnumerator Co_SlowingPlayer()
    {
        inGrabPenalty = true;
        var speedTemp = speed;
        speed = speedTemp * 0.5f;

        yield return new WaitForSeconds(0.5f);

        speed = speedTemp;
        Co_Slow = null;
        inGrabPenalty = false;
    }
}
