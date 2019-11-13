using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    PlayerController playerScript;
    public float cameraSpeed;
    public bool lerp = true;
    public bool follow = true;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerScript = player.GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (follow)
        {
            if (lerp)
            {
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * cameraSpeed), Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * cameraSpeed), transform.position.z);
            }
            else
            {
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            }
        }
    }
}
