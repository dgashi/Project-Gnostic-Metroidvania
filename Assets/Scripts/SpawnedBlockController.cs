using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnedBlockController : FallingObjectController
{
    private bool isInvincible = false;
    [SerializeField]
    private bool pairedToPlatform;
    private SpriteRenderer sr;

    public override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
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
            sr.sortingLayerID = belowInput.collidingObjects[0].GetComponent<TilemapRenderer>().sortingLayerID;
            sr.sortingOrder = belowInput.collidingObjects[0].GetComponent<TilemapRenderer>().sortingOrder;
            pairedToPlatform = true;
        }
        else if (belowInput.collidingObjects.Count == 0 && pairedToPlatform)
        {
            pairedToPlatform = false;
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
}
