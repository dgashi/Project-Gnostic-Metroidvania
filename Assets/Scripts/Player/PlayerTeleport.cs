using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{

    public float maxTeleportDistance;
    [SerializeField]
    private float teleportDistance;

    BoxCollider2D bc;
    
    public GameObject teleportSprite;
    public SpriteRenderer teleportSpriteRenderer;

    PlayerStates states;
    Movement movement;

    private void Start()
    {
        states = GetComponent<PlayerStates>();
        movement = GetComponent<Movement>();
        bc = GetComponent<BoxCollider2D>();
        teleportSpriteRenderer = teleportSprite.GetComponent<SpriteRenderer>();
        teleportSpriteRenderer.enabled = false;
    }

    public void PrepareTeleport()
    {
        states.isPreparingTeleport = true;
        StartCoroutine(LookForSpace());
    }

    IEnumerator LookForSpace()
    {
        //Keep updating the teleport distance and moving the teleport sprite
        //until the player lets go of the button
        while (!Input.GetButtonUp("L1"))
        {
            //Find the biggest distance with enough space to fit the player
            for (float distance = 0; distance <= maxTeleportDistance; distance += 0.1f)
            {
                states.isSqueezedTeleport = Physics2D.OverlapBox(new Vector2(transform.position.x + distance * states.direction, transform.position.y + bc.size.y / 2 - bc.size.x / 2), new Vector2(bc.size.x - 0.015f, bc.size.y - 0.015f), 0f, states.whatIsPlatform | states.whatIsDamage);

                if (!states.isSqueezedTeleport)
                {
                    teleportDistance = distance;
                }
            }

            //Position the teleport target sprite at that distance from the player
            teleportSprite.transform.localPosition = new Vector3(teleportDistance, 0, 0);

            //Only display the sprite if teleport distance is bigger than zero
            if (teleportDistance > 0)
            {
                teleportSpriteRenderer.enabled = true;
            }
            else
            {
                teleportSpriteRenderer.enabled = false;
            }

            yield return null;
        }

        //Once the player lets go of the button, disable the teleport sprite
        //and move the player if the distance is bigger than zero
        teleportSpriteRenderer.enabled = false;

        if (teleportDistance > 0)
        {
            states.isGrabbed = false;
            movement.velocity.x = 0;
            movement.velocity.y = 0;
            transform.position = new Vector3(transform.position.x + states.direction * teleportDistance, transform.position.y, transform.position.z);
            states.hasTeleported = true;
        }
        
        states.isPreparingTeleport = false;
        yield break;
    }
}
