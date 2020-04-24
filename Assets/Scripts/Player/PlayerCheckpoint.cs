using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    public Transform defaultCheckPoint;
    public Transform lastCheckPoint;

    private void Start()
    {
        lastCheckPoint = defaultCheckPoint;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minor Checkpoint") && lastCheckPoint != collision.gameObject.transform)
        {
            lastCheckPoint = collision.gameObject.transform;
        }
    }
}
