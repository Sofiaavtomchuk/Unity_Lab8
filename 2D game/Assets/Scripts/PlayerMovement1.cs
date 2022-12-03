using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement1 : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float jumpHeight = 450f;
    public bool isJumping;
    private float horizontalMove;
    private bool isFacingRight = true;

    //dashing
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;


    Vector2 movementVector;
    Rigidbody2D rbody;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) 
        {
            return;
        }
        Vector2 playerVelocity = new Vector2(movementVector.x * movementSpeed, rbody.velocity.y);
        rbody.velocity = playerVelocity;
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        Flip();
    }

    private void OnMove(InputValue value)
    {
        if (isDashing)
        {
            return;
        }
        horizontalMove = Input.GetAxisRaw("Horizontal") * movementSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        movementVector = value.Get<Vector2>();
        Debug.Log(movementVector);
    } 
    private void OnJump(InputValue value)
    {
        if (isDashing)
        {
            return;
        }
        if (value.isPressed && isJumping == false)
        {
            rbody.AddForce(new Vector2(rbody.velocity.x, jumpHeight));
            animator.SetBool("IsJumping", true);
            Debug.Log("jumpHeight");
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isJumping = true;
            }
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMove < 0f || !isFacingRight && horizontalMove > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rbody.gravityScale;
        rbody.gravityScale = 0f;
        rbody.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rbody.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("fef");
        Destroy(collision.gameObject);
    }
}
