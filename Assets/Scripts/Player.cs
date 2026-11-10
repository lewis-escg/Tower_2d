using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D RigidBody;
    public GameObject GroundCheck;
    public float GroundCheckRadius = 0.25f;
    public LayerMask GroundLayer;
    public float Gravity;
    public float AccelerationTime;
    public float BaseMoveSpeed;
    public float BaseJumpForce;


    private float horizontalInput;
    private bool isGrounded;
    private bool isSlopedGround;
    private Vector2 slopeNormal;
    private Vector2 slopeNormal_perpendicular;
    private float moveSpeed;
    private float jumpForce;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get horizontal input from player
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // check if player is grounded
        isGrounded = Physics2D.OverlapCircle(GroundCheck.transform.position, GroundCheckRadius, GroundLayer);
        // check for slope
        CheckForSlope();

        if (!isSlopedGround)
        {
            RigidBody.AddForce(Physics.gravity, ForceMode2D.Force);
        }
        else
        {
            RigidBody.AddForce(-slopeNormal * Physics.gravity.magnitude * Gravity, ForceMode2D.Force);
        }

        // update max speed
        moveSpeed = BaseMoveSpeed;
        jumpForce = BaseJumpForce;

        // update horizontal velocity
        Vector2 newHorizontalVelocity = new(RigidBody.velocity.x + horizontalInput * (moveSpeed / AccelerationTime) * Time.deltaTime, 1);
        if (isGrounded && isSlopedGround)
        {
            Debug.Log(slopeNormal_perpendicular);
            newHorizontalVelocity = new Vector2(newHorizontalVelocity.x * slopeNormal_perpendicular.x, newHorizontalVelocity.x * slopeNormal_perpendicular.y);
        }
        RigidBody.velocity = new Vector2(math.clamp(newHorizontalVelocity.x, -moveSpeed, moveSpeed), RigidBody.velocity.y);

        // if jump is pressed and player is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // run jump function
            Jump();
        }
    }

    void Jump()
    {
        // set y velocity to jump force
        RigidBody.velocity = new Vector2(RigidBody.velocity.x, jumpForce);
    }

    void CheckForSlope()
    {
        if (!isGrounded)
        {
            isSlopedGround = false;
            return;
        }
        else
        {
            RaycastHit2D rayResult = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, GroundCheckRadius, GroundLayer);

            if (rayResult)
            {
                slopeNormal = rayResult.normal;
                slopeNormal_perpendicular = Vector2.Perpendicular(slopeNormal).normalized;
                isSlopedGround = math.abs(slopeNormal.x) > 0.01f;

                Debug.DrawRay(rayResult.point, slopeNormal, Color.green);
                Debug.DrawRay(rayResult.point, slopeNormal_perpendicular, Color.red);
            }
        }
    }



    // gizmos
    void OnDrawGizmosSelected()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GroundCheck.transform.position, GroundCheckRadius);
        }
    }
}
