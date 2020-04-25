using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    PlayerStates states;
    PlayerSpawnDecoy spawnDecoy;
    PlayerSpawnBlock spawnBlock;
    PlayerTeleport teleport;

    public Vector2 playerInput;

    void Start()
    {
        states = GetComponent<PlayerStates>();
        spawnDecoy = GetComponent<PlayerSpawnDecoy>();
        spawnBlock = GetComponent<PlayerSpawnBlock>();
        teleport = GetComponent<PlayerTeleport>();
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");

            if (!states.isSliding && !states.isGrabbed)
            {
                if (Input.GetAxis("Horizontal") != 0)
                {
                    //Try to move player right or left
                }
                else if (!states.dontStopX)
                {
                    //Try to stop player
                }
            }
            
            if (Input.GetButton("R1"))
            {
                //Try to grab wall
            }
            
            if (Input.GetButtonDown("X"))
            {
                //Try to jump
            }
            
            if (Input.GetButtonDown("R2"))
            {
                //Try to initiate slide
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
