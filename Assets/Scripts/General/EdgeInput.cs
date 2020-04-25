using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeInput : MonoBehaviour
{
    public bool isColliding;
    public List<GameObject> collidingObjects;
    public List<PlatformController> collidingPlatformControllers;
    public LayerMask whatIsPlatform;

    public enum Direction
    {
        Above,
        Below,
        Front,
        Back
    }

    public Direction colliderPosition;

    [SerializeField]
    private Rigidbody2D parentrb;
    [SerializeField]
    private States parentStates;

    private void Start()
    {
        parentrb = transform.parent.GetComponent<Rigidbody2D>();
        parentStates = transform.parent.GetComponent<States>();
    }

    private void OnDisable()
    {
        isColliding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If colliding object is in the "Ground" layer:
        if (Utilities.IsInLayerMask(collision.gameObject.layer, whatIsPlatform))
        {
            isColliding = true;

            if (parentStates)
            {
                switch (colliderPosition)
                {
                    case Direction.Above:
                        parentStates.isTouchingCeiling = true;
                        break;
                    case Direction.Below:
                        parentStates.isTouchingGround = true;
                        break;
                    case Direction.Front:
                        parentStates.isTouchingWallInFront = true;
                        break;
                    case Direction.Back:
                        parentStates.isTouchingWallBehind = true;
                        break;
                }
            }
            
            //Save colliding object to list of all currently colliding objects
            collidingObjects.Add(collision.gameObject);

            //Save the MovingPlatformController of colliding object if it has one
            if(collision.gameObject.GetComponent<PlatformController>() != null)
            {
                collidingPlatformControllers.Add(collision.gameObject.GetComponent<PlatformController>());

                if (colliderPosition == Direction.Below && parentStates.isAffectedByGravity)
                {
                    collision.gameObject.GetComponent<PlatformController>().AddPassengerRB(parentrb);
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        //If colliding object is in the "Ground" layer:
        if (Utilities.IsInLayerMask(collision.gameObject.layer, whatIsPlatform))
        {
            //Remove exiting object from list of all currently colliding objects
            collidingObjects.Remove(collision.gameObject);

            //Forget the MovingPlatformController of colliding object if it has one
            if (collision.gameObject.GetComponent<PlatformController>() != null)
            {
                collidingPlatformControllers.Remove(collision.gameObject.GetComponent<PlatformController>());

                if (colliderPosition == Direction.Below && parentStates.isAffectedByGravity)
                {
                    collision.gameObject.GetComponent<PlatformController>().RemovePassengerRB(parentrb);
                }
            }

            //If the list is empty, show that nothing is colliding
            if (collidingObjects.Count == 0)
            {
                isColliding = false;

                if (parentStates)
                {
                    switch (colliderPosition)
                    {
                        case Direction.Above:
                            parentStates.isTouchingCeiling = false;
                            break;
                        case Direction.Below:
                            parentStates.isTouchingGround = false;
                            break;
                        case Direction.Front:
                            parentStates.isTouchingWallInFront = false;
                            break;
                        case Direction.Back:
                            parentStates.isTouchingWallBehind = false;
                            break;
                    }
                }
            }
        }
    }
}
