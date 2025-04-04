//v2.0
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{

    [SerializeField] Rigidbody2D rbody;
    [SerializeField] Animator anim;

    // movement variables
    private float horizInput;                       // for storing horiz input
    private float moveSpeed = 4.5f;                 // horizontal speed (units per sec)
    private float toNewtons = 100f;                 // for conversion to newtons

    // jump vars
    private float jumpHeight = 3.0f;                // jump height in units
    private float jumpTimeToApex = 0.375f;          // jump time
    private float initialJumpVelocity;              // upward (Y) velocity at the start of a jump
    bool jumpPressed = false;                       // true if jump pressed but not yet processed
    int jumpMax = 2;                                // max # of jumps player can do
    int jumpsAvailable = 0;                         // current jumps available to player

    // grounded vars
    bool isGrounded = false;
    [SerializeField] Transform groundCheckPoint;    // draw a circle around this to check ground
    [SerializeField] LayerMask groundLayerMask;     // a layer for all ground items
    float groundCheckRadius = 0.3f;                 // radius of ground check circle

    // direction vars
    bool facingRight = true;                        // true if player facing right

    void Start()
    {
        // given a desired jumpHeight and jumpTime, calculate gravity (same formulas as 3D)
        float gravity = (-2 * jumpHeight) / Mathf.Pow(jumpTimeToApex, 2);
        // -42 / -9.81 == 4
        rbody.gravityScale = gravity / Physics2D.gravity.y;

        // calculate jump velocity (upward motion)
        initialJumpVelocity = (2.0f * jumpHeight) / jumpTimeToApex;
    }

    void Flip()
    {
        // flip the player to face the opposite direction
        facingRight = !facingRight;
        transform.Rotate(Vector3.up * 180f);
    }

    void Update()
    {
        horizInput = Input.GetAxis("Horizontal");   // read (and store) horizontal input

        // determine running
        bool isRunning = Mathf.Abs(horizInput) > 0.01; ;
        anim.SetBool("isRunning", isRunning);

        // determine grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayerMask) &&
            rbody.linearVelocity.y < 0.01;
        anim.SetBool("isGrounded", isGrounded);

        // reset jumps
        if (isGrounded)
        {
            jumpsAvailable = jumpMax;
        }

        // detect jump
        if (Input.GetButtonDown("Jump") && jumpsAvailable > 0)
        {
            jumpPressed = true;
        }

        // if we're facing in the opposite direction of movement, Flip the player!
        if ((!facingRight && horizInput > 0.01) || (facingRight && horizInput < -0.01))
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // We're moving via Rigidbody physics, so use FixedUpdate
        float xVel = horizInput * moveSpeed * toNewtons * Time.deltaTime;   // determine new x velocity
        float yVel = rbody.linearVelocity.y;                                // use existing y velocity

        if (jumpPressed)
        {
            // process jump
            yVel = initialJumpVelocity;                 // overwrite y velocity for jump
            jumpsAvailable--;
            jumpPressed = false;
            anim.SetTrigger("jump");
        }

        Vector3 movement = new Vector2(xVel, yVel);     // determine move direction
        rbody.linearVelocity = movement;                // move!
    }

    private void OnDrawGizmos()
    {
        // draw the ground check sphere
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
