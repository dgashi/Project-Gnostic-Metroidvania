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
    public string[] listOfDamagingTags;

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
    public int maxHealth;
    [SerializeField]
    private int currentHealth;
    public int maxMana;
    [SerializeField]
    private int currentMana;
    private HashSet<GameObject> hasBeenPickedUp;

    private PlatformController platformInFrontController;

    [SerializeField]
    private bool isColliderLying;
    [SerializeField]
    private bool isColliderUpright;
    [SerializeField]
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
    private bool hasTeleported = false;
    [SerializeField]
    private bool dontStopX = false;
    [SerializeField]
    private bool dontMoveX = false;
    [SerializeField]
    private bool isInvincible;

    public Transform defaultCheckPoint;
    public Transform lastCheckPoint;
    public GameObject sprite;
    public GameObject teleportSprite;
    public GameObject decoy;
    public GameObject block;
    private SpriteRenderer teleportSpriteRenderer;
    private Camera cam;
    private CameraController camController;
    public Image damageScreen;
    public Text healthText;
    public Text manaText;

    public override void Start()
    {
        base.Start();

        isColliderLying = false;

        currentHealth = maxHealth;

        cam = Camera.main;
        camController = cam.GetComponent<CameraController>();

        teleportSpriteRenderer = teleportSprite.GetComponent<SpriteRenderer>();
        teleportSpriteRenderer.enabled = false;

        lastCheckPoint = defaultCheckPoint;

        hasBeenPickedUp = new HashSet<GameObject>();

        healthText.text = "Health: " + currentHealth;
        manaText.text = "Mana: " + currentMana;
    }


    public override void Update()
    {
        if(Time.timeScale != 0)
        {
            base.Update();

            isCloseToGround = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - bc.size.y / 2), new Vector2(bc.size.x - checkInset * 2, groundCheckDistance * 2), 0f, whatIsPlatform);
            if (isColliderLying)
            {
                isCloseToWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * bc.size.y / 2, transform.position.y), new Vector2(wallCheckDistance * 2, bc.size.y - checkInset * 2), 0f, whatIsPlatform);
            }
            else
            {
                isCloseToWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * bc.size.x / 2, transform.position.y), new Vector2(wallCheckDistance * 2, bc.size.x - checkInset * 2), 0f, whatIsPlatform);
            }

            if (!isColliderLying)
            {
                isSqueezedLying = Physics2D.OverlapBox(new Vector2(transform.position.x + (bc.size.y / 2 - bc.size.x / 2) * direction, transform.position.y - (bc.size.y/2 - bc.size.x/2)), new Vector2(biggerDimension - 0.02f, smallerDimension - 0.02f), 0f, whatIsPlatform | whatIsDamage);
            }
            else if (!isColliderUpright)
            {
                isSqueezedUpright = Physics2D.OverlapBox(new Vector2(transform.position.x + (bc.size.x / 2 - bc.size.y / 2) * direction, transform.position.y + (bc.size.x / 2 - bc.size.y / 2)), new Vector2(smallerDimension - 0.02f, biggerDimension - 0.02f), 0f, whatIsPlatform | whatIsDamage);
            }

            //Reset variables when grounded
            if (isCloseToGround)
            {
                dontStopX = false;
                dontMoveX = false;
                hasAirJumped = false;
                hasTeleported = false;
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
                dontStopX = false;
                dontMoveX = false;
                hasAirJumped = false;
                hasTeleported = false;
            }

            //While grabbed
            if (isGrabbed)
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
                    isGrabbed = false;
                }
            }

            //Jump
            if (Input.GetButtonDown("X") && isCloseToGround)
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
            else if (Input.GetButtonDown("X") && !hasAirJumped && !isSliding && !disableAirJump)
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
                velocity.x = Mathf.Lerp(velocity.x, moveSpeed * direction * 3, Time.deltaTime * acceleration / 2);

                if (!isSqueezedLying)
                {
                    LayDownColliders();
                }
                else
                {
                    BunchUpColliders();
                }

                if (frontInput.isColliding && !isSqueezedUpright)
                {
                    isSliding = false;
                    velocity.x = 0;
                }
            }
            else if (!isColliderUpright)
            {
                sprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
                RaiseUpColliders();
            }

            //Teleport
            if (!isSliding && Input.GetButton("L1") && !hasTeleported)
            {
                for (float distance = 0; distance <= maxTeleportDistance; distance += 0.1f)
                {
                    isSqueezedTeleport = Physics2D.OverlapBox(new Vector2(transform.position.x + distance * direction, transform.position.y + bc.size.y / 2 - bc.size.x / 2), new Vector2(bc.size.x - 0.015f, bc.size.y - 0.015f), 0f, whatIsPlatform | whatIsDamage);

                    if (!isSqueezedTeleport)
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

            if (!isSliding && Input.GetButtonUp("L1") && !hasTeleported)
            {
                teleportSpriteRenderer.enabled = false;

                if (teleportDistance > 0)
                {
                    isGrabbed = false;
                    velocity.y = 0;
                    velocity.x = 0;
                    transform.position = new Vector3(transform.position.x + direction * teleportDistance, transform.position.y, transform.position.z);
                    hasTeleported = true;
                }
            }

            //Spawn decoy
            if (Input.GetButtonDown("L2"))
            {
                decoy.SetActive(false);
                decoy.transform.position = new Vector3(transform.position.x - 0.6f * direction, transform.position.y, transform.position.z);
                decoy.SetActive(true);
            }

            //Spawn stone block
            if (Input.GetButtonDown("R1") && Input.GetAxis("Vertical") < 0)
            {
                block.SetActive(false);
                block.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                block.SetActive(true);
            }

            if (((aboveInput.isColliding && belowInput.isColliding) || (frontInput.isColliding && backInput.isColliding)) && !isInvincible)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utilities.IsTagInList(collision.gameObject, listOfDamagingTags) && !isInvincible)
        {
            currentHealth -= 1;
            healthText.text = "Health: " + currentHealth;
            isInvincible = true;
            velocity = Vector2.zero;
            Time.timeScale = 0f;
            StartCoroutine(ResetAfterDamage());
        }

        if (collision.gameObject.CompareTag("Minor Checkpoint") && lastCheckPoint != collision.gameObject.transform)
        {
            lastCheckPoint = collision.gameObject.transform;
        }

        if (collision.gameObject.CompareTag("1 Mana") && currentMana < maxMana && !hasBeenPickedUp.Contains(collision.gameObject))
        {
            currentMana += 1;
            manaText.text = "Mana: " + currentMana;
            hasBeenPickedUp.Add(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }


    public void LayDownColliders()
    {
        if (!isColliderLying)
        {
            bc.size = new Vector2(biggerDimension, smallerDimension);
            transform.position -= new Vector3((bc.size.x / 2 - bc.size.y / 2) * -direction, bc.size.x / 2 - bc.size.y / 2, 0);
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
            transform.position += new Vector3((bc.size.x / 2 - bc.size.y / 2) * direction, bc.size.x / 2 - bc.size.y / 2, 0);
            bc.size = new Vector2(smallerDimension, biggerDimension);
            UpdateEdgeCollidersUpright();
            isColliderUpright = true;
            isColliderLying = false;
            isColliderBunched = false;
        }
    }


    public bool GetIsSliding()
    {
        return isSliding;
    }


    public float GetDirection()
    {
        return direction;
    }


    private void ResetToLastCheckpoint()
    {
        isSliding = false;
        isGrabbed = false;
        teleportSpriteRenderer.enabled = false;
        transform.position = lastCheckPoint.position;
        direction = Mathf.Sign(Input.GetAxis("Horizontal"));
        transform.localScale = new Vector3(direction, 1, 1);
        camController.SnapToPlayer();
        isInvincible = false;
    }


    private IEnumerator ResetAfterDamage()
    {
        float duration = 0.3f;
        float smoothness = 20f;
        float delay = 0.5f;
        float timeStep = 0;
        while (timeStep < duration + delay)
        {
            timeStep += duration / smoothness;
            damageScreen.color = Color.Lerp(Color.clear, Color.black, timeStep / duration);
            yield return Utilities.WaitForUnscaledSeconds(duration / smoothness);
        }
        ResetToLastCheckpoint();
        Time.timeScale = 1f;
        StartCoroutine(BlackToClear());
    }


    private IEnumerator BlackToClear()
    {
        float duration = 0.3f;
        float smoothness = 20f;
        float timeStep = 0;
        while (timeStep < duration)
        {
            timeStep += duration / smoothness;
            damageScreen.color = Color.Lerp(Color.black, Color.clear, timeStep / duration);
            yield return Utilities.WaitForUnscaledSeconds(duration / smoothness);
        }
    }
}
