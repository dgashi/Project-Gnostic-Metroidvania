using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : States
{
    public bool isGrabbed = false;
    public bool isSliding = false;
    public bool isCloseToGround;
    public bool isCloseToWall;
    public bool isSqueezedTeleport = false;
    public bool hasAirJumped = false;
    public bool hasTeleported = false;
    public bool dontMoveX = false;
    public bool isPreparingTeleport;
    public bool isPreparingPossession = false;
    public bool isPossessing = false;

    public float groundCheckDistance;
    public float wallCheckDistance;
    public float checkInset;

    private BoxCollider2D bc;

    private void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        isCloseToGround = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - bc.size.y / 2), new Vector2(bc.size.x - checkInset * 2, groundCheckDistance * 2), 0f, whatIsPlatform);

        if (isColliderLying)
        {
            isCloseToWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * bc.size.y / 2, transform.position.y), new Vector2(wallCheckDistance * 2, bc.size.y - checkInset * 2), 0f, whatIsPlatform);
        }
        else
        {
            isCloseToWall = Physics2D.OverlapBox(new Vector2(transform.position.x + direction * bc.size.x / 2, transform.position.y), new Vector2(wallCheckDistance * 2, bc.size.x - checkInset * 2), 0f, whatIsPlatform);
        }

        if (isCloseToGround)
        {
            dontMoveX = false;
            hasAirJumped = false;
            hasTeleported = false;
        }
    }
}
