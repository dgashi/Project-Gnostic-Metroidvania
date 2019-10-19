using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    public CollisionInfo collisions;
    
    private bool above, below, left, right;

    private void Update()
    {
        above = collisions.above;
        below = collisions.below;
        left = collisions.left;
        right = collisions.right;
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        MovingVerticalCollisions(ref velocity);
        MovingHorizontalCollisions(ref velocity);

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.position = Vector3.Lerp(transform.position, transform.position + velocity, 60);
        //transform.position += velocity;
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = (directionX == -1);
                collisions.right = (directionX == 1);
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

            rayOrigin += Vector2.right * (verticalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);
            }
        }
    }

    void MovingHorizontalCollisions(ref Vector3 velocity)
    {
        float rayLength = skinWidth * 3;

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < horizontalRayCount; j++)
            {
                Vector2 rayOrigin = (i == 0) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

                rayOrigin += Vector2.up * (horizontalRaySpacing * j);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * ((i == 0) ? -1 : 1), rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * ((i == 0) ? -1 : 1) * rayLength, Color.blue);

                /*if (hit && hit.collider.gameObject.GetComponent<PlatformController>() != null)
                {
                    PlatformController platformController = hit.collider.gameObject.GetComponent<PlatformController>();

                    if ((i == 0 && platformController.GetVelocity().x < 0) || (i == 1 && platformController.GetVelocity().x > 0))
                    {
                        velocity.x += (i == 0) ? skinWidth : -skinWidth;
                    }
                }*/
            }
        }
    }

    void MovingVerticalCollisions(ref Vector3 velocity)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < horizontalRayCount; j++)
            {
                float rayLength = -(velocity.y - skinWidth);

                Vector2 rayOrigin = (i == 0) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

                rayOrigin += Vector2.right * (verticalRaySpacing * j + velocity.x);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * ((i == 0) ? -1 : 1), rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * ((i == 0) ? -1 : 1) * rayLength, Color.blue);

                /*if (hit && hit.collider.gameObject.GetComponent<PlatformController>() != null)
                {
                    PlatformController platformController = hit.collider.gameObject.GetComponent<PlatformController>();

                    if (i == 0 && platformController.GetVelocity().y < 0)
                    {
                        platformController.AddPassenger(transform);
                    }
                }*/
            }
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public Vector3 velocityOld;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
        }
    }
}
