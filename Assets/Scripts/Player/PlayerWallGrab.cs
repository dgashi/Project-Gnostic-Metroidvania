using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallGrab : MonoBehaviour
{
    PlayerStates states;
    Movement movement;
    Rigidbody2D rb;

    EdgeInput frontColliderInput;

    void Start()
    {
        states = GetComponent<PlayerStates>();
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();

        foreach (EdgeInput i in GetComponentsInChildren<EdgeInput>())
        {
            if (i.colliderPosition == EdgeInput.Direction.Front)
            {
                frontColliderInput = i;
            }
        }
    }

    public void GrabWall()
    {
        states.isGrabbed = true;
        
        states.dontMoveX = false;
        states.hasAirJumped = false;
        states.hasTeleported = false;

        states.isAffectedByGravity = false;
        movement.SetVelocityX(0, false);
        movement.SetVelocityY(0, false);

        frontColliderInput.SetConnectToMovingPlatforms(true);

        StartCoroutine(WhileGrabbed());
    }

    IEnumerator WhileGrabbed()
    {
        while (states.isGrabbed)
        {
            if (!states.isTouchingWallInFront)
            {
                states.isGrabbed = false;
            }
            
            yield return null;
        }

        frontColliderInput.SetConnectToMovingPlatforms(false);
        states.isAffectedByGravity = true;
    }
}
