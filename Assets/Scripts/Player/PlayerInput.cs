using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    PlayerStates states;
    PlayerSpawnDecoy spawnDecoy;
    PlayerSpawnBlock spawnBlock;
    PlayerTeleport teleport;
    PlayerWallGrab wallGrab;
    PlayerSlide slide;
    Movement movement;

    public float dontGrabWallDelay;
    float dontGrabWallTimer;

    void Start()
    {
        states = GetComponent<PlayerStates>();
        spawnDecoy = GetComponent<PlayerSpawnDecoy>();
        spawnBlock = GetComponent<PlayerSpawnBlock>();
        teleport = GetComponent<PlayerTeleport>();
        wallGrab = GetComponent<PlayerWallGrab>();
        slide = GetComponent<PlayerSlide>();
        movement = GetComponent<Movement>();

        dontGrabWallTimer = 0;
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (!states.dontMoveX && !states.isSliding && !states.isGrabbed)
            {
                movement.SetVelocityX(Input.GetAxis("Horizontal"), true);
            }

            if (Input.GetButtonDown("X") && states.isCloseToGround)
            {
                if (states.isSliding)
                {
                    movement.SetVelocityY(1.7f / 2f, false);
                }
                else
                {
                    movement.SetVelocityY(1.7f, false);
                }
            }
            else if (Input.GetButtonDown("X") && states.isGrabbed)
            {
                states.dontMoveX = true;
                states.isGrabbed = false;
                movement.SetVelocityX(-states.direction, false);
                movement.SetVelocityY(1.7f, false);
                dontGrabWallTimer = dontGrabWallDelay;
            }
            else if (Input.GetButtonDown("X") && !states.hasAirJumped && !states.isSliding)
            {
                movement.SetVelocityY(1.7f, false);
                states.hasAirJumped = true;
                states.dontMoveX = false;
            }

            if (dontGrabWallTimer <= 0)
            {
                if (Input.GetButton("R1") && states.isTouchingWallInFront && !states.isSliding && !states.isGrabbed && !states.isTouchingGround && states.isColliderUpright)
                {
                    wallGrab.GrabWall();
                }
            }
            else
            {
                dontGrabWallTimer -= Time.unscaledDeltaTime;
            }

            if (Input.GetButtonUp("R1") && states.isGrabbed)
            {
                states.isGrabbed = false;
            }

            if (Input.GetButtonDown("R2") && states.isCloseToGround && !states.isSliding && !states.isGrabbed)
            {
                slide.Slide();
            }
            
            if (Input.GetButtonDown("L1") && !states.isSliding && !states.hasTeleported && !states.isPreparingTeleport)
            {
                teleport.PrepareTeleport();
            }
            
            if (Input.GetButtonDown("R1") && Input.GetAxis("Vertical") < 0)
            {
                spawnBlock.SpawnBlock();
            }

            if (Input.GetButtonDown("L2"))
            {
                spawnDecoy.SpawnDecoy();
            }
        }
    }
}
