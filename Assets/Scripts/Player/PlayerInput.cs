using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    PlayerStates states;
    PlayerSpawnDecoy spawnDecoy;
    PlayerSpawnBlock spawnBlock;
    PlayerTeleport teleport;
    Movement movement;

    public Vector2 playerInput;

    void Start()
    {
        states = GetComponent<PlayerStates>();
        spawnDecoy = GetComponent<PlayerSpawnDecoy>();
        spawnBlock = GetComponent<PlayerSpawnBlock>();
        teleport = GetComponent<PlayerTeleport>();
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (!states.isSliding && !states.isGrabbed)
            {
                movement.MoveX(Input.GetAxis("Horizontal"), true);
            }
            
            if (Input.GetButton("R1"))
            {
                //Try to grab wall
            }
            
            if (Input.GetButtonDown("X"))
            {
                movement.MoveY(1.7f, false);
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
