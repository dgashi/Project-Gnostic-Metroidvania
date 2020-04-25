using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    PlayerStates playerStates;
    public float cameraSpeed;
    public float aheadDistance;
    public float aheadSpeedMultiplier;
    public bool lerp = true;
    public bool follow = true;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerStates = player.GetComponent<PlayerStates>();
    }

    void FixedUpdate()
    {
        if (follow)
        {
            if (playerStates.isSliding)
            {
                if (lerp)
                {
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x + aheadDistance * playerStates.direction, Time.deltaTime * cameraSpeed * aheadSpeedMultiplier), Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * cameraSpeed * aheadSpeedMultiplier), transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(player.transform.position.x + aheadDistance * playerStates.direction, player.transform.position.y, transform.position.z);
                }
            }
            else
            {
                if (lerp)
                {
                    //Lerp both x- and y-positions
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * cameraSpeed), Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * cameraSpeed), transform.position.z);
                    
                    //Only lerp x-position
                    //transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * cameraSpeed), player.transform.position.y, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
                }
            }
        }
    }

    public void SnapToPlayer()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
