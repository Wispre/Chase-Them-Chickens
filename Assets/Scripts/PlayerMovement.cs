using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

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

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(movementInput) * speed;
        rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);
    }

    private void Update()
    {
        MovePlayer();
    }
}
