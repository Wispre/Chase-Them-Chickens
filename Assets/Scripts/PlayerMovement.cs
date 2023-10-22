using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private float turnSmoothTime = 0.05f;
    private float turnSmoothVelocity;

    private Rigidbody rb;
    private Vector3 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void GetDirectionInput(InputAction.CallbackContext ctx)
    {
        movementInput.x = ctx.ReadValue<Vector2>().x;
        movementInput.z = ctx.ReadValue<Vector2>().y;
    }

    private void Move()
    {
        Vector3 MoveVector = movementInput * speed;
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
}
