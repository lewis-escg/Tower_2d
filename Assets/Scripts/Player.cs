using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
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
    public float MoveSequenceSpeed;
    public bool LockControls = false;
    public Animator AnimationController;
    public GameObject GameManager;


    private float horizontalInput;
    private bool isGrounded;
    private bool isSlopedGround;
    private Vector2 slopeNormal;
    private Vector2 slopeNormal_perpendicular;
    private float moveSpeed;
    private float jumpForce;
    private Vector3 originalScale;
    private List<GameObject> interactables = new();



    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;


    }

    // Update is called once per frame
    void Update()
    {
        // get horizontal input from player
        if (!LockControls) horizontalInput = Input.GetAxisRaw("Horizontal"); // get horizontal input if controls arent locked

        // check if player is grounded
        isGrounded = Physics2D.OverlapCircle(GroundCheck.transform.position, GroundCheckRadius, GroundLayer);
        // check for slope
        CheckForSlope();


        if (!isSlopedGround) // gravity stuff
        {
            // normal ground/air, normal gravity
            //RigidBody.AddForce(Physics.gravity * Gravity, ForceMode2D.Force); old gravity code
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, RigidBody.velocity.y + Physics.gravity.y * Gravity * Time.deltaTime);
        }
        else
        {
            // on slopes, pull the player into the slope (prevents sliding)
            //RigidBody.AddForce(-slopeNormal * Physics.gravity.magnitude * Gravity, ForceMode2D.Force); old gravity code
            RigidBody.velocity = new Vector2(RigidBody.velocity.x + (-slopeNormal.x * Physics.gravity.magnitude * Gravity * Time.deltaTime), RigidBody.velocity.y + (-slopeNormal.y * Physics.gravity.magnitude * Gravity * Time.deltaTime));
        }

        // update max speed
        moveSpeed = BaseMoveSpeed;
        jumpForce = BaseJumpForce;

        // update horizontal velocity
        float accelRate = (moveSpeed / AccelerationTime) * Time.deltaTime;
        if (isGrounded && isSlopedGround) // sloped movement
        {
            // target velocity
            Vector2 targetSlopeVelocity = -slopeNormal_perpendicular * horizontalInput * moveSpeed;

            // accelerate/move/etc
            RigidBody.velocity = Vector2.MoveTowards(RigidBody.velocity, targetSlopeVelocity, accelRate);
        }
        else // non-sloped movement
        {

            // target velocity
            Vector2 targetVelocity = new(horizontalInput * moveSpeed, 0); // 0 as a placeholder for y magnitude because we dont use it
                                                                            // get the accelerated velocity
            Vector2 horizontalVelocity = Vector2.MoveTowards(RigidBody.velocity, targetVelocity, accelRate);
            // clamp velocity (max move speed) and apply to rigidbody
            RigidBody.velocity = new Vector2(math.clamp(horizontalVelocity.x, -moveSpeed, moveSpeed), RigidBody.velocity.y);
        }


        // visuals (facing direction, animations, etc)
        if (RigidBody.velocity.x > 0.1f)
        {
            transform.localScale = originalScale;
            if (AnimationController.GetInteger("State") != 1) // if not walking
            {
                AnimationController.SetInteger("State", 1); // set state (prevent running trigger more than once and spamming the animation)
                AnimationController.SetTrigger("Walk"); // run walk animation
            }
        }
        else if (RigidBody.velocity.x < -0.1f)
        {
            transform.localScale = new Vector2(originalScale.x * -1, originalScale.y);
            if (AnimationController.GetInteger("State") != 1) // if not walking
            {
                AnimationController.SetInteger("State", 1); // set state
                AnimationController.SetTrigger("Walk"); // run walk animation
            }
        }
        else
        {
            if (AnimationController.GetInteger("State") != 0) // if not idle
            {
                AnimationController.SetInteger("State", 0); // set state
                AnimationController.SetTrigger("Idle"); // run idle animation
            }
        }



        // if jump is pressed and player is grounded and controls arent locked
        if (Input.GetButton("Jump") && isGrounded && !LockControls)
        {
            // run jump function
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E) && !LockControls) // when E is pressed and controls arent locked
        {
            GameObject closestInteractable = null;
            float distance = 100.0f; // compare to see which distance is least, so we start with a big one
            foreach (GameObject interactable in interactables) // for every available interactable
            {
                float distanceCheck = (transform.position - interactable.transform.position).magnitude; // get distance
                if (distanceCheck < distance && interactable.activeInHierarchy) // if distance is less than others so far and is active
                {
                    distance = distanceCheck; // set new distance
                    closestInteractable = interactable; // set new closest interactable
                }
            }
            if (closestInteractable != null)
            {
                if (closestInteractable.TryGetComponent<Interact>(out Interact interaction)) // if interactable exists and has interact component
                {
                    interaction.RunInteraction(this); // run interaction
                }
            }
        }

    }



    // triggers
    private void OnTriggerEnter2D(Collider2D collision) // enter trigger
    {
        switch (collision.tag)
        {
            case "Move": // move sequence
                {
                    LockControls = true;
                    Vector3 targetObjectPosition = collision.transform.Find("Target").position;
                    Vector3 target = new Vector3(targetObjectPosition.x, targetObjectPosition.y, gameObject.transform.position.z); // maintain our original z position
                    float distance = (collision.transform.position - target).magnitude;
                    float time = distance / moveSpeed;
                    GameManager.GetComponent<MoveToTarget>().Move(gameObject, target, time * (1/MoveSequenceSpeed), 67f);
                    break;
                }
            case "Interact": // interactable
                {
                    interactables.Add(collision.gameObject); // add to interactables list
                    break;
                }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) // exit trigger
    {
        switch (collision.tag)
        {
            case "Interact": // interactable
                {
                    if (interactables.Contains(collision.gameObject)) // if in interactables list
                    {
                        interactables.Remove(collision.gameObject); // remove from interactables list
                    }
                    break;
                }
        }
    }




    // methods

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
            RaycastHit2D rayResult = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, GroundCheckRadius + 1, GroundLayer);

            if (rayResult)
            {
                // get normal
                slopeNormal = rayResult.normal;
                // get perpendicular vector for applying forces and tings
                slopeNormal_perpendicular = Vector2.Perpendicular(slopeNormal).normalized;
                // if greater than theshold of 0.01, we consider the ground slanted
                isSlopedGround = math.abs(slopeNormal.x) > 0.01f;

                // show the slope of the surface the player is on
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
