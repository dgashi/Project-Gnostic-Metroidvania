using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnedBlockController : FallingObjectController
{
    private bool isInvincible = false;
    [SerializeField]
    private bool pairedToPlatform;
    private HashSet<Rigidbody2D> passengerrbs;
    private SpriteRenderer sr;
    private GameObject platformConnectedTo;
    private PlatformController platformControllerBelow;

    public override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
        passengerrbs = new HashSet<Rigidbody2D>();
    }

    public override void Update()
    {

        if (Time.timeScale != 0)
        {
            base.Update();

            if (belowInput.isColliding && velocity.y == 0)
            {
                rb.isKinematic = true;
            }
        }

        if (belowInput.collidingObjects.Count != 0 && !pairedToPlatform)
        {
            platformConnectedTo = belowInput.collidingObjects[0];
            sr.sortingLayerID = platformConnectedTo.GetComponent<TilemapRenderer>().sortingLayerID;
            sr.sortingOrder = platformConnectedTo.GetComponent<TilemapRenderer>().sortingOrder;
            pairedToPlatform = true;

            platformControllerBelow = platformConnectedTo.GetComponent<PlatformController>();
        }
        else if (belowInput.collidingObjects.Count == 0 && pairedToPlatform)
        {
            platformConnectedTo = null;
            platformControllerBelow = null;
            pairedToPlatform = false;
        }

        if (platformControllerBelow != null && passengerrbs.Count > 0)
        {
            foreach (Rigidbody2D rb in passengerrbs)
            {
                platformControllerBelow.AddPassengerRB(rb);
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            RemovePassengerRB(collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }

    private void OnDisable()
    {
        velocity = new Vector2(0, 0);
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
    }

    public void AddPassengerRB(Rigidbody2D passengerrb)
    {
        if (!passengerrbs.Contains(passengerrb))
        {
            passengerrbs.Add(passengerrb);
        }
    }

    public void RemovePassengerRB(Rigidbody2D passengerrb)
    {
        if (passengerrbs.Contains(passengerrb))
        {
            passengerrbs.Remove(passengerrb);
        }
    }
}
