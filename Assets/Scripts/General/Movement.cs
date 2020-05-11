using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector2 velocity;

    public float moveSpeed;
    public float acceleration;
    public float gravity;

    private States states;
    private Rigidbody2D rb;

    EdgeInput belowColliderInput;

    void Start()
    {
        states = GetComponent<States>();
        rb = GetComponent<Rigidbody2D>();

        foreach (EdgeInput i in GetComponentsInChildren<EdgeInput>())
        {
            if (i.colliderPosition == EdgeInput.Direction.Below)
            {
                belowColliderInput = i;
            }
        }
    }

    void Update()
    {
        if (states.isAffectedByGravity)
        {
            //Make bottom edge collider connect to moving platforms
            if (!belowColliderInput.GetConnectToMovingPlatforms())
            {
                belowColliderInput.SetConnectToMovingPlatforms(true);
            }

            //Apply gravity;
            if (velocity.y <= 0)
            {
                velocity.y += -gravity * 3 * Time.deltaTime;
            }
            else
            {
                velocity.y += -gravity * Time.deltaTime;
            }
        }
        else
        {
            //Make bottom edge collider not connect to moving platforms
            belowColliderInput.SetConnectToMovingPlatforms(false);
        }

        //Stop moving vertically if hitting ceiling or ground
        if (velocity.y > 0 && states.isTouchingCeiling)
        {
            velocity.y = 0;
        }

        if (velocity.y < 0 && states.isTouchingGround)
        {
            velocity.y = 0;
        }
    }

    private void LateUpdate()
    {
        rb.velocity = velocity;
    }

    public void SetVelocityX(float modifier, bool accelerate)
    {
        if (modifier != 0)
        {
            states.direction = Mathf.Sign(modifier);
            transform.localScale = new Vector3(states.direction, 1, 1);
        }

        if (accelerate)
        {
            velocity.x = Mathf.Lerp(velocity.x, moveSpeed * modifier, Time.deltaTime * acceleration);
        }
        else
        {
            velocity.x = moveSpeed * modifier;
        }
    }

    public void SetVelocityY(float modifier, bool accelerate)
    {
        if (accelerate)
        {
            velocity.y = Mathf.Lerp(velocity.y, moveSpeed * modifier, Time.deltaTime * acceleration);
        }
        else
        {
            velocity.y = moveSpeed * modifier;
        }
    }

    public void SetVelocity(float xModifier, float yModifier, bool accelerate)
    {
        if (xModifier != 0)
        {
            states.direction = Mathf.Sign(xModifier);
            transform.localScale = new Vector3(states.direction, 1, 1);
        }

        if (accelerate)
        {
            velocity = Vector2.Lerp(velocity, new Vector2(moveSpeed * xModifier, moveSpeed * yModifier), Time.deltaTime * acceleration);
        }
        else
        {
            velocity = new Vector2(moveSpeed * xModifier, moveSpeed * yModifier);
        }
    }
}
