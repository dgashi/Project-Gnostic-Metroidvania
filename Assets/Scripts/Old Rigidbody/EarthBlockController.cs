using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBlockController : MonoBehaviour
{
    public GameObject[] arrayOfEarthBlocks;
    public bool isGrounded;
    public float groundCheckDistance;
    public LayerMask whatIsPlatform;
    private BoxCollider2D bc;
    private Rigidbody2D rb;

    public bool isMovingY;
    private Vector2 lastPosition;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        arrayOfEarthBlocks = GameObject.FindGameObjectsWithTag("Earth Block");
        for (int i = 0; i < arrayOfEarthBlocks.Length-1; i++)
        {
            Destroy(arrayOfEarthBlocks[i].gameObject);
        }
    }

    
    void Update()
    {
        isGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - bc.size.y / 2), new Vector2(bc.size.x - 0.1f, groundCheckDistance * 2), 0f, whatIsPlatform);

        if (isGrounded)
        {
            gameObject.layer = 8;
        }
        else
        {
            gameObject.layer = 10;
        }

        CheckMovement();
        if (!isMovingY && isGrounded)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }


    private void CheckMovement()
    {
        if (!Mathf.Approximately(transform.position.y, lastPosition.y))
        {
            isMovingY = true;
        }
        else
        {
            isMovingY = false;
        }

        lastPosition = transform.position;
    }
}
