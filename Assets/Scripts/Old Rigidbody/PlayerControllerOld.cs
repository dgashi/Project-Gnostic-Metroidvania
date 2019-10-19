using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerControllerOld : MonoBehaviour
{
    public float currentSpeed;
    public float speed;
    public float jumpForce;
    public float maxTeleportDistance;
    public float teleportDistance;
    public int currentMana;
    private int maxMana = 4;
    public float direction;

    public float playerDimensionX;
    public float playerDimensionY;
    private float playerCenterOffset;

    public GameObject earthPrefab;

    private Rigidbody2D rb;
    private GameObject sprite;
    private BoxCollider2D bc;
    
    public float groundCheckDistance;
    public float wallCheckDistance;

    public LayerMask whatIsPlatform;

    private bool hasAirJumped;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public bool isGrounded;
    public bool isWall;
    public bool isSqueezedUpright;
    public bool isSqueezedLying;
    public bool isSqueezedTeleport;
    public bool isSqueezedTeleportMax;
    public bool isGrabbed;
    public bool isSliding;
    public bool isMovingX;
    public bool isMovingY;
    private bool isManaSpent;
    public bool disableMoveX;
    public bool disableStopX;
    public bool disableDoubleJump;

    private float slideStartTimer;
    
    private Vector3 lastPosition;
    
    //Input variables:
    private float xAxis;
    private float yAxis;
    private bool jumpButton;
    private bool jumpButtonHeld;
    private bool grabButton;
    private bool grabButtonReleased;
    private bool grabButtonHeld;
    private bool teleportButton;
    private bool teleportButtonReleased;
    private bool teleportButtonHeld;
    public bool elementButton;
    private bool elementButtonReleased;
    private bool elementButtonHeld;
    private bool slideButton;
    private bool slideButtonHeld;

    void Start()
    {
        hasAirJumped = false;
        isSliding = false;
        disableMoveX = false;
        direction = 1;
        currentMana = maxMana;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        slideStartTimer = 0.1f;
        playerDimensionX = 0.5f;
        playerDimensionY = 1.2f;
        teleportDistance = maxTeleportDistance;
    }

    
    private void FixedUpdate()
    {
        playerCenterOffset = playerDimensionY / 2 - playerDimensionX / 2;

        Teleport();
        GrabWall();
        CreateBox();
        Jump();
        BetterJump();
        Slide();
        Resize();
        MoveHorizontally();
        Flip();
        CheckMovement();
    }


    void Update()
    {
        currentSpeed = rb.velocity.x;

        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");

        jumpButton = Input.GetButtonDown("X");
        jumpButtonHeld = Input.GetButton("X");

        grabButton = Input.GetButtonDown("L1");
        grabButtonReleased = Input.GetButtonUp("L1");
        grabButtonHeld = Input.GetButton("L1");

        teleportButton = Input.GetButtonDown("L2");
        teleportButtonReleased = Input.GetButtonUp("L2");
        teleportButtonHeld = Input.GetButton("L2");

        elementButton = Input.GetButtonDown("R1");
        elementButtonReleased = Input.GetButtonUp("R1");
        elementButtonHeld = Input.GetButton("R1");

        slideButton = Input.GetButtonDown("R2");
        slideButtonHeld = Input.GetButton("R2");
    }


    void MoveHorizontally()
    {
        if ((xAxis > 0.5f || xAxis < -0.5f) && !disableMoveX)
        {
            direction = xAxis / Mathf.Abs(xAxis);
            rb.velocity = new Vector2(speed * direction, rb.velocity.y);
        }
        else if (xAxis != 0 && !disableMoveX)
        {
            direction = xAxis / Mathf.Abs(xAxis);
            rb.velocity = new Vector2(speed / 4 * direction, rb.velocity.y);
        }
        else if (!disableStopX)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }


    void Flip()
    {
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
    }
    

    void Jump()
    {
        isGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - playerDimensionX / 2), new Vector2(playerDimensionX - 0.1f, groundCheckDistance * 2), 0f, whatIsPlatform);

        if (isGrounded)
        {
            disableStopX = false;
            hasAirJumped = false;
            if (!isSliding)
            {
                disableMoveX = false;
            }
        }

        if (jumpButton && (isGrounded))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (jumpButton && isGrabbed)
        {
            disableStopX = true;
            disableMoveX = true;
            direction = -direction;
            rb.velocity = new Vector2(direction * speed, jumpForce);
        }
        else if (jumpButton && !hasAirJumped && !isSliding && !disableDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentMana--;
            hasAirJumped = true;
            disableStopX = false;
            disableMoveX = false;
        }

        
    }


    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpButtonHeld)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }


    void GrabWall()
    {
        
        isWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * playerDimensionX / 2, transform.position.y), new Vector2(wallCheckDistance * 2, playerDimensionX - 0.1f), 0f, whatIsPlatform);

        if (isWall && !isGrounded && grabButtonHeld)
        {
            isGrabbed = true;
            SpendMana(1);
        }
        else
        {
            isGrabbed = false;
            isManaSpent = false;
        }

        if (isGrabbed)
        {
            disableStopX = true;
            disableMoveX = true;
            hasAirJumped = false;

            rb.gravityScale = 0;

            rb.velocity = new Vector2(direction * speed, 0);
        }
        else
        {
            rb.gravityScale = 1;
            //disableMoveX = false; //this should be coupled to a timer
        }
    }


    void Slide()
    {
        if (isGrounded && slideButton && !isSliding)
        {
            currentMana--;
            isSliding = true;
        }
        if (isSliding)
        {
            slideStartTimer = slideStartTimer - Time.deltaTime;
            //sprite.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            disableMoveX = true;
            disableStopX = true;
            rb.velocity = new Vector2(direction * speed * 2, rb.velocity.y);

            if (slideStartTimer <= 0f && !isMovingX)
            {
                isSliding = false;
                //sprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
                slideStartTimer = 0.05f;
                disableMoveX = false;
                disableStopX = false;
            }
        }
    }

    private void Teleport()
    {

        for (int distance = 0; distance/10f <= maxTeleportDistance; distance++)
        {
            isSqueezedTeleport = Physics2D.OverlapBox(new Vector2(transform.position.x + distance* 0.1f * direction, transform.position.y + playerCenterOffset), new Vector2(playerDimensionX - 0.1f, playerDimensionY - 0.1f), 0f, whatIsPlatform);
            if (!isSqueezedTeleport)
            {
                teleportDistance = distance * 0.1f;
            }
        }

        if (!isSliding && teleportButtonReleased)
        {
            transform.position = new Vector3(transform.position.x + direction * teleportDistance, transform.position.y, transform.position.z);
            currentMana--;
        }
    }

    
    private void CreateBox()
    {
        if (yAxis < 0f && elementButton)
        {
            //PrefabUtility.InstantiatePrefab(Instantiate(earthPrefab, new Vector3(transform.position.x, transform.position.y - playerDimensionX / 2 - 0.25f, 0f), Quaternion.Euler(0, 0, 0)));
        }
    }


    private void SpendMana(int amount)
    {
        if (!isManaSpent)
        {
            currentMana = currentMana - amount;
        }
        isManaSpent = true;
    }


    private void Resize()
    {
        isSqueezedUpright = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + playerCenterOffset), new Vector2(playerDimensionX - 0.1f, playerDimensionY - 0.1f), 0f, whatIsPlatform);
        isSqueezedLying = Physics2D.OverlapBox(new Vector2(transform.position.x - direction * playerCenterOffset, transform.position.y), new Vector2(playerDimensionY - 0.1f, playerDimensionX - 0.1f), 0f, whatIsPlatform);

        if (isSliding && !isSqueezedLying)
        {
            bc.size = new Vector2(playerDimensionY, playerDimensionX);
            bc.offset = new Vector2(-playerCenterOffset, 0f);
        }
        else if (!isSliding && !isSqueezedUpright)
        {
            bc.size = new Vector2(playerDimensionX, playerDimensionY);
            bc.offset = new Vector2(0f, playerCenterOffset);
        }
        else
        {
            bc.size = new Vector2(playerDimensionX, playerDimensionX);
            bc.offset = new Vector2(0f, 0f);
        }
    }


    private void CheckMovement()
    {
        if (!Mathf.Approximately(transform.position.x, lastPosition.x))
        {
            isMovingX = true;
        }
        else
        {
            isMovingX = false;
        }

        if (!Mathf.Approximately(transform.position.y, lastPosition.y))
        {
            isMovingY = true;
        }
        else
        {
            isMovingY = false;
        }

        lastPosition = transform.position;
    }
}
