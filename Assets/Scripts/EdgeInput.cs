using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeInput : MonoBehaviour
{
    public bool isColliding;
    public List<GameObject> collidingObjects;
    public List<PlatformController> collidingPlatformControllers;
    public LayerMask whatIsPlatform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If colliding object is in the "Ground" layer:
        if (Utilities.IsInLayerMask(collision.gameObject.layer, whatIsPlatform))
        {
            isColliding = true;

            //Save colliding object to list of all currently colliding objects
            collidingObjects.Add(collision.gameObject);

            //Save the MovingPlatformController of colliding object if it has one
            if(collision.gameObject.GetComponent<PlatformController>() != null)
            {
                collidingPlatformControllers.Add(collision.gameObject.GetComponent<PlatformController>());
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
            }

            //If the list is empty, show that nothing is colliding
            if (collidingObjects.Count == 0)
            {
                isColliding = false;
            }
        }
    }
}
