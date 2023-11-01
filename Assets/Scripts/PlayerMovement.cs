using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public GameEvent OnHit;

    public LayerMask groundLayer;

    public PlayerCollect collect;
    public Bag bag;
    public Animator animator;

    private Camera cam;

    private float speed = 7f;
    private float speedPenalty = 0.05f;

    private Rigidbody rb;
    private Vector3 movementInput;
    private Vector3 mousePos;
    private Vector3 rotateDirection = Vector3.zero;

    private Coroutine Co_Slow;

    private bool inGrabPenalty = false;

    private bool isGrounded = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collect = GetComponent<PlayerCollect>();
        cam = Camera.main;
    }

    private void OnEnable()
    {
        collect.OnGrabChicken += SlowPlayer;
    }

    private void OnDisable()
    {
        collect.OnGrabChicken -= SlowPlayer;
    }

    public void GetHit()
    {
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);

        rb.velocity = new Vector3(0f,rb.velocity.y,0f);

        OnHit.Raise();
    }

    private float GetModifiedSpeed()
    {
        var originalSpeed = speed;

        if (inGrabPenalty)
        {
            originalSpeed *= 2;

        }

        var speedPercentage = (originalSpeed * speedPenalty);
        var modSpeed = originalSpeed - ( speedPercentage * bag.GetCount());
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

    public void GetMousePos(InputAction.CallbackContext ctx)
    {
        mousePos.x = ctx.ReadValue<Vector2>().x;
        mousePos.y = ctx.ReadValue<Vector2>().y;
    }

    private void Move()
    {
        Vector3 MoveVector = movementInput * GetModifiedSpeed();
        rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);
        
        if(MoveVector.magnitude >= 0.01f)
        {
            animator.SetBool("Run", true);
        }
        else{
            animator.SetBool("Run", false);
        }

    }

    private void Rotate()
    {
        Ray ray = cam.ScreenPointToRay(mousePos);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity ,groundLayer))
        {
            rotateDirection.x = hit.point.x;
            rotateDirection.z = hit.point.z;
            transform.LookAt(rotateDirection);
        } 
    }
    private void Update()
    {
        if (Pause.IsPaused) return;
        isGrounded = GroundCheck();

        if (isGrounded)
        {
            animator.SetBool("isGrounded", true);
            Rotate();
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded) return;
        Move();

    }

    private bool GroundCheck()
    {
        bool isGrounded;

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        return isGrounded;
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
