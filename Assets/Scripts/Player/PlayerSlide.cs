using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    PlayerStates states;
    BoxCollider2D bc;
    ColliderManager colliderManager;
    SpaceChecker spaceChecker;
    Movement movement;

    public GameObject sprite;

    float biggerDimension;
    float smallerDimension;

    void Start()
    {
        states = GetComponent<PlayerStates>();
        bc = GetComponent<BoxCollider2D>();
        colliderManager = GetComponent<ColliderManager>();
        spaceChecker = GetComponent<SpaceChecker>();
        movement = GetComponent<Movement>();

        smallerDimension = Mathf.Min(bc.size.x, bc.size.y);
        biggerDimension = Mathf.Max(bc.size.x, bc.size.y);
    }

    public void Slide()
    {
        if (spaceChecker.HasSpaceLying())
        {
            LayDownColliders();
        }
        else
        {
            BunchUpColliders();
        }

        sprite.transform.localRotation = Quaternion.Euler(0, 0, 90f);
        states.isSliding = true;

        StartCoroutine(WhileSliding());
    }

    IEnumerator WhileSliding()
    {
        while (states.isSliding)
        {
            Debug.Log("Sliding...");

            movement.SetVelocityX(3f * states.direction, true);

            if (spaceChecker.HasSpaceLying())
            {
                LayDownColliders();
            }

            if (states.isTouchingWallInFront && spaceChecker.HasSpaceUpright())
            {
                Debug.Log("Hit wall!");

                states.isSliding = false;
            }

            yield return null;
        }

        while (!spaceChecker.HasSpaceUpright())
        {
            yield return null;
        }

        movement.SetVelocityX(0, false);
        RaiseUpColliders();
        sprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void LayDownColliders()
    {
        if (!states.isColliderLying)
        {
            bc.size = new Vector2(biggerDimension, smallerDimension);
            transform.position -= new Vector3((bc.size.x / 2 - bc.size.y / 2) * -states.direction, bc.size.x / 2 - bc.size.y / 2, 0);
            colliderManager.UpdateEdgeColliders();
            states.isColliderUpright = false;
            states.isColliderLying = true;
            states.isColliderBunched = false;
        }
    }

    public void BunchUpColliders()
    {
        if (!states.isColliderBunched)
        {
            bc.size = new Vector2(smallerDimension, smallerDimension);
            colliderManager.UpdateEdgeColliders();
            states.isColliderUpright = false;
            states.isColliderLying = false;
            states.isColliderBunched = true;
        }
    }

    public void RaiseUpColliders()
    {
        if (!states.isColliderUpright)
        {
            transform.position += new Vector3((bc.size.x / 2 - bc.size.y / 2) * states.direction, bc.size.x / 2 - bc.size.y / 2, 0);
            bc.size = new Vector2(smallerDimension, biggerDimension);
            colliderManager.UpdateEdgeColliders();
            states.isColliderUpright = true;
            states.isColliderLying = false;
            states.isColliderBunched = false;
        }
    }
}
