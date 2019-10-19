using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour
{
    public float jumpHeight = 4f;
    public float timeToJumpApex = 0.4f;
    public float fallingGravityMultiplier = 1.5f;
    public float lowJumpGravityMultiplier = 2f;

    public float moveSpeed = 6f;
    private float gravity;
    private float jumpVelocity;

    Vector3 velocity; 

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        Debug.Log("Gravity: " + gravity + "\nJump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("X") && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        if (input.x < -0.5f || input.x > 0.5f)
        {
            velocity.x = Mathf.Sign(input.x) * moveSpeed;
        }
        else if (input.x != 0f)
        {
            velocity.x = Mathf.Sign(input.x) * moveSpeed / 4;
        }
        else
        {
            velocity.x = 0f;
        }

        if (velocity.y > 0f)
        {
            velocity.y += gravity * Time.deltaTime;
            if (!Input.GetButton("X"))
            {
                velocity.y += gravity * lowJumpGravityMultiplier * Time.deltaTime;
            }
        }
        else
        {
            velocity.y += gravity * fallingGravityMultiplier * Time.deltaTime;
        }
            
        controller.Move(velocity * Time.deltaTime);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
