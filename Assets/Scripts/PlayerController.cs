using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GroundedCharacterController
{
    public float moveSpeed;
    public float acceleration;
    public float jumpForce;
    public Vector2 wallJumpModifier;

    public float maxTeleportDistance;
    [SerializeField]
    private float teleportDistance;

    public float playerCenterOffset;
    public float smallerDimension;
    public float biggerDimension;

    public float slideStartTimer = 0.1f;

    public float groundCheckDistance;
    public float wallCheckDistance;

    [SerializeField]
    private float direction = 1;

    private PlatformController platformInFrontController;

    private bool isColliderLying;
    private bool isColliderUpright;
    private bool isColliderBunched;
    [SerializeField]
    private bool isGrabbed = false;
    [SerializeField]
    private bool isSliding = false;
    [SerializeField]
    private bool isCloseToGround;
    [SerializeField]
    private bool isCloseToWall;
    [SerializeField]
    private bool isSqueezedUpright;
    [SerializeField]
    private bool isSqueezedLying;
    [SerializeField]
    private bool isSqueezedTeleport = false;
    [SerializeField]
    private bool hasAirJumped = false;
    [SerializeField]
    private bool dontStopX = false;
    [SerializeField]
    private bool dontMoveX = false;

    public GameObject sprite;


    public override void Start()
    {
        base.Start();

        bc.offset = new Vector2(0, bc.size.y / 2 - bc.size.x / 2);
        UpdateEdgeCollidersUpright();

        isColliderLying = false;
    }


    public override void Update()
    {
        base.Update();

        playerCenterOffset = -(biggerDimension - smallerDimension) / 2;

        isCloseToGround = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - bc.size.x / 2), new Vector2(bc.size.x - 0.1f, groundCheckDistance * 2), 0f, whatIsPlatform);
        isCloseToWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * bc.size.x / 2, transform.position.y), new Vector2(wallCheckDistance * 2, bc.size.x - 0.1f), 0f, whatIsPlatform);

        isSqueezedLying = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * playerCenterOffset, transform.position.y), new Vector2(biggerDimension - 0.02f, smallerDimension - 0.02f), 0f, whatIsPlatform);
        isSqueezedUpright = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - playerCenterOffset), new Vector2(smallerDimension - 0.02f, biggerDimension - 0.02f), 0f, whatIsPlatform);
        
        //Reset variables when grounded
        if(belowInput.isColliding)
        {
            dontStopX = false;
            dontMoveX = false;
            hasAirJumped = false;
        }

        //Move left and right
        if (!isSliding && !isGrabbed)
        {
            if (Input.GetAxis("Horizontal") != 0 && !dontMoveX)
            {
                direction = Mathf.Sign(Input.GetAxis("Horizontal"));
                velocity.x = Mathf.Lerp(velocity.x, moveSpeed * direction, Time.deltaTime * acceleration);
                transform.localScale = new Vector3(direction, 1, 1);
            }
            else if (!dontStopX)
            {
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * acceleration);
            }
        }

        //Grab Wall
        if (Input.GetButton("R1") && isCloseToWall && !isSliding && !isGrabbed && !belowInput.isColliding && isColliderUpright)
        {
            isGrabbed = true;
        }

        //While grabbed
        if (isGrabbed)
        {
            dontStopX = false;
            dontMoveX = false;

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
                isGrabbed = false;
            }
        }

        //Jump
        if (Input.GetButtonDown("X") && belowInput.isColliding)
        {
            if (isSliding)
            {
                velocity.y = jumpForce / 3 * 2;
            }
            else
            {
                velocity.y = jumpForce;
            }
        }
        else if (Input.GetButtonDown("X") && isGrabbed)
        {
            direction = -direction;
            transform.localScale = new Vector3(direction, 1, 1);
            velocity = new Vector2(moveSpeed * direction, jumpForce) * wallJumpModifier;
            dontStopX = true;
            dontMoveX = true;
            isGrabbed = false;
        }
        else if (Input.GetButtonDown("X") && !hasAirJumped && !isSliding)
        {
            velocity.y = jumpForce;
            hasAirJumped = true;
            dontStopX = false;
            dontMoveX = false;
        }

        //Initiate slide
        if (Input.GetButtonDown("R2") && isCloseToGround && !isSliding && !isGrabbed)
        {
            if (!isSqueezedLying)
            {
                LayDownColliders();
            }
            else
            {
                BunchUpColliders();
            }

            sprite.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            isSliding = true;
        }

        //While sliding
        if (isSliding)
        {
            slideStartTimer = slideStartTimer - Time.deltaTime;

            velocity.x = Mathf.Lerp(velocity.x, moveSpeed * direction * 3, Time.deltaTime * acceleration / 2);

            if (!isSqueezedLying)
            {
                LayDownColliders();
            }
            else
            {
                BunchUpColliders();
            }

            if (frontInput.isColliding && slideStartTimer <= 0)
            {
                isSliding = false;
                velocity.x = 0;
                slideStartTimer = 0.1f;
            }
        }
        else if(!isSqueezedUpright && !isColliderUpright)
        {
            sprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
            RaiseUpColliders();
        }

        //Teleport
        if (!isSliding && Input.GetButton("L1"))
        {
            for (int distance = 0; distance / 10f <= maxTeleportDistance; distance++)
            {
                isSqueezedTeleport = Physics2D.OverlapBox(new Vector2(transform.position.x + distance * 0.1f * direction, transform.position.y + bc.size.y / 2 - bc.size.x / 2), new Vector2(bc.size.x - 0.1f, bc.size.y - 0.1f), 0f, whatIsPlatform);
                if (!isSqueezedTeleport)
                {
                    teleportDistance = distance * 0.1f;
                }
            }
        }

        if (!isSliding && Input.GetButtonUp("L1"))
        {
            isGrabbed = false;
            velocity.y = 0;
            velocity.x = 0;
            transform.position = new Vector3(transform.position.x + direction * teleportDistance, transform.position.y, transform.position.z);
        }
    }


    public void LayDownColliders()
    {
        if (!isColliderLying)
        {
            bc.size = new Vector2(biggerDimension, smallerDimension);
            bc.offset = new Vector2(-(bc.size.x / 2 - bc.size.y / 2), 0);
            UpdateEdgeCollidersLying();
            isColliderUpright = false;
            isColliderLying = true;
            isColliderBunched = false;
        }
    }


    public void BunchUpColliders()
    {
        if (!isColliderBunched)
        {
            bc.size = new Vector2(smallerDimension, smallerDimension);
            bc.offset = new Vector2(0, 0);
            UpdateEdgeCollidersLying();
            isColliderUpright = false;
            isColliderLying = false;
            isColliderBunched = true;
        }
    }


    public void RaiseUpColliders()
    {
        if (!isColliderUpright)
        {
            bc.size = new Vector2(smallerDimension, biggerDimension);
            bc.offset = new Vector2(0, bc.size.y / 2 - bc.size.x / 2);
            UpdateEdgeCollidersUpright();
            isColliderUpright = true;
            isColliderLying = false;
            isColliderBunched = false;
        }
    }
}
