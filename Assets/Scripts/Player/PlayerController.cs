using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : FallingObjectController
{
    public bool disableAirJump;
    public float moveSpeed;
    public float acceleration;
    public float jumpForce;
    public Vector2 wallJumpModifier;
    public LayerMask whatIsDamage;

    public float maxTeleportDistance;
    [SerializeField]
    private float teleportDistance;

    public float playerCenterOffset;
    public float smallerDimension;
    public float biggerDimension;

    public float squeezeTimer = 0.1f;

    public float groundCheckDistance;
    public float wallCheckDistance;
    public float checkInset;

    [SerializeField]
    private float direction = 1;

    private PlatformController platformInFrontController;

    private PlayerStates states;

    public GameObject sprite;
    public GameObject teleportSprite;
    private SpriteRenderer teleportSpriteRenderer;
    private Camera cam;
    private CameraController camController;

    public override void Start()
    {
        base.Start();

        cam = Camera.main;
        camController = cam.GetComponent<CameraController>();

        states = GetComponent<PlayerStates>();

        states.isColliderLying = false;

        teleportSpriteRenderer = teleportSprite.GetComponent<SpriteRenderer>();
        teleportSpriteRenderer.enabled = false;
    }


    public override void Update()
    {
        if(Time.timeScale != 0)
        {
            base.Update();

            states.isCloseToGround = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - bc.size.y / 2), new Vector2(bc.size.x - checkInset * 2, groundCheckDistance * 2), 0f, whatIsPlatform);

            if (states.isColliderLying)
            {
                states.isCloseToWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * bc.size.y / 2, transform.position.y), new Vector2(wallCheckDistance * 2, bc.size.y - checkInset * 2), 0f, whatIsPlatform);
            }
            else
            {
                states.isCloseToWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * bc.size.x / 2, transform.position.y), new Vector2(wallCheckDistance * 2, bc.size.x - checkInset * 2), 0f, whatIsPlatform);
            }

            if (!states.isColliderLying)
            {
                states.isSqueezedLying = Physics2D.OverlapBox(new Vector2(transform.position.x + (bc.size.y / 2 - bc.size.x / 2) * direction, transform.position.y - (bc.size.y/2 - bc.size.x/2)), new Vector2(biggerDimension - 0.02f, smallerDimension - 0.02f), 0f, whatIsPlatform | whatIsDamage);
            }
            else if (!states.isColliderUpright)
            {
                states.isSqueezedUpright = Physics2D.OverlapBox(new Vector2(transform.position.x + (bc.size.x / 2 - bc.size.y / 2) * direction, transform.position.y + (bc.size.x / 2 - bc.size.y / 2)), new Vector2(smallerDimension - 0.02f, biggerDimension - 0.02f), 0f, whatIsPlatform | whatIsDamage);
            }

            //Reset variables when grounded
            if (states.isCloseToGround)
            {
                states.dontStopX = false;
                states.dontMoveX = false;
                states.hasAirJumped = false;
                states.hasTeleported = false;
            }

            //Move left and right
            if (!states.isSliding && !states.isGrabbed)
            {
                if (Input.GetAxis("Horizontal") != 0 && !states.dontMoveX)
                {
                    direction = Mathf.Sign(Input.GetAxis("Horizontal"));
                    velocity.x = Mathf.Lerp(velocity.x, moveSpeed * Input.GetAxis("Horizontal"), Time.deltaTime * acceleration);
                    transform.localScale = new Vector3(direction, 1, 1);
                }
                else if (!states.dontStopX)
                {
                    velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * acceleration);
                    ///velocity.x = 0;
                }
            }

            //Grab Wall
            if (Input.GetButton("R1") && states.isCloseToWall && !states.isSliding && !states.isGrabbed && !belowInput.isColliding && states.isColliderUpright)
            {
                states.isGrabbed = true;
                states.dontStopX = false;
                states.dontMoveX = false;
                states.hasAirJumped = false;
                states.hasTeleported = false;
            }

            //While grabbed
            if (states.isGrabbed)
            {

                velocity.y = 0;

                if (frontInput.isColliding)
                {
                    velocity.x = 0;

                    if (frontInput.collidingPlatformControllers.Count != 0)
                    {
                        frontInput.collidingPlatformControllers[belowInput.collidingPlatformControllers.Count - 1].AddPassengerRB(rb);
                    }
                }
                else
                {
                    velocity.x = direction * 100;
                }

                if (Input.GetButtonUp("R1"))
                {
                    states.isGrabbed = false;
                }
            }

            //Jump
            if (Input.GetButtonDown("X") && states.isCloseToGround)
            {
                if (states.isSliding)
                {
                    velocity.y = jumpForce / 3 * 2;
                }
                else
                {
                    velocity.y = jumpForce;
                }
            }
            else if (Input.GetButtonDown("X") && states.isGrabbed)
            {
                direction = -direction;
                transform.localScale = new Vector3(direction, 1, 1);
                velocity = new Vector2(moveSpeed * direction, jumpForce) * wallJumpModifier;
                states.dontStopX = true;
                states.dontMoveX = true;
                states.isGrabbed = false;
            }
            else if (Input.GetButtonDown("X") && !states.hasAirJumped && !states.isSliding && !disableAirJump)
            {
                velocity.y = jumpForce;
                states.hasAirJumped = true;
                states.dontStopX = false;
                states.dontMoveX = false;
            }

            //Initiate slide
            if (Input.GetButtonDown("R2") && states.isCloseToGround && !states.isSliding && !states.isGrabbed)
            {
                if (!states.isSqueezedLying)
                {
                    LayDownColliders();
                }
                else
                {
                    BunchUpColliders();
                }

                sprite.transform.localRotation = Quaternion.Euler(0, 0, 90f);
                states.isSliding = true;
            }

            //While sliding
            if (states.isSliding)
            {
                velocity.x = Mathf.Lerp(velocity.x, moveSpeed * direction * 3, Time.deltaTime * acceleration / 2);

                if (!states.isSqueezedLying)
                {
                    LayDownColliders();
                }
                else
                {
                    BunchUpColliders();
                }

                if (frontInput.isColliding && !states.isSqueezedUpright)
                {
                    states.isSliding = false;
                    velocity.x = 0;
                }
            }
            else if (!states.isColliderUpright)
            {
                sprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
                RaiseUpColliders();
            }

            //Teleport
            if (!states.isSliding && Input.GetButton("L1") && !states.hasTeleported)
            {
                for (float distance = 0; distance <= maxTeleportDistance; distance += 0.1f)
                {
                    states.isSqueezedTeleport = Physics2D.OverlapBox(new Vector2(transform.position.x + distance * direction, transform.position.y + bc.size.y / 2 - bc.size.x / 2), new Vector2(bc.size.x - 0.015f, bc.size.y - 0.015f), 0f, whatIsPlatform | whatIsDamage);

                    if (!states.isSqueezedTeleport)
                    {
                        teleportDistance = distance;
                    }
                }

                teleportSprite.transform.localPosition = new Vector3(teleportDistance, 0, 0);

                if (teleportDistance > 0)
                {
                    teleportSpriteRenderer.enabled = true;
                }
                else
                {
                    teleportSpriteRenderer.enabled = false;
                }
            }

            if (!states.isSliding && Input.GetButtonUp("L1") && !states.hasTeleported)
            {
                teleportSpriteRenderer.enabled = false;

                if (teleportDistance > 0)
                {
                    states.isGrabbed = false;
                    velocity.y = 0;
                    velocity.x = 0;
                    transform.position = new Vector3(transform.position.x + direction * teleportDistance, transform.position.y, transform.position.z);
                    states.hasTeleported = true;
                }
            }

            if (((aboveInput.isColliding && belowInput.isColliding) || (frontInput.isColliding && backInput.isColliding)) && !states.isInvincible)
            {
                squeezeTimer -= Time.deltaTime;

                if (squeezeTimer <= 0)
                {
                    Debug.Log("Player was squeezed");
                }
            }
            else
            {
                squeezeTimer = 0.1f;
            }
        }
    }


    public void LayDownColliders()
    {
        if (!states.isColliderLying)
        {
            bc.size = new Vector2(biggerDimension, smallerDimension);
            transform.position -= new Vector3((bc.size.x / 2 - bc.size.y / 2) * -direction, bc.size.x / 2 - bc.size.y / 2, 0);
            UpdateEdgeCollidersLying();
            states.isColliderUpright = false;
            states.isColliderLying = true;
            states.isColliderBunched = false;
        }
    }


    public void BunchUpColliders()
    {
        if (!states.isColliderBunched)
        {
            bc.size = new Vector2(smallerDimension, smallerDimension);
            UpdateEdgeCollidersLying();
            states.isColliderUpright = false;
            states.isColliderLying = false;
            states.isColliderBunched = true;
        }
    }


    public void RaiseUpColliders()
    {
        if (!states.isColliderUpright)
        {
            transform.position += new Vector3((bc.size.x / 2 - bc.size.y / 2) * direction, bc.size.x / 2 - bc.size.y / 2, 0);
            bc.size = new Vector2(smallerDimension, biggerDimension);
            UpdateEdgeCollidersUpright();
            states.isColliderUpright = true;
            states.isColliderLying = false;
            states.isColliderBunched = false;
        }
    }


    public bool GetIsSliding()
    {
        return states.isSliding;
    }


    public float GetDirection()
    {
        return direction;
    }

}
