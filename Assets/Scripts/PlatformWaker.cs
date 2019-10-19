using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformWaker : MonoBehaviour
{
    public Vector2 move;
    public float reverseMovementTime;

    void Start()
    {
        PlatformController pc = PlatformController.CreateInitialized(gameObject, move, reverseMovementTime);
    }
}
