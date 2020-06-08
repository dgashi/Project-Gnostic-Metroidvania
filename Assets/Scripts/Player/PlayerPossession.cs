using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPossession : MonoBehaviour
{
    PlayerStates states;
    PlayerInput input;
    PlayerProximitySensor proximitySensor;
    Possessable closestTarget;
    public float possessionTime;

    void Start()
    {
        states = GetComponent<PlayerStates>();
        input = GetComponent<PlayerInput>();
        proximitySensor = GetComponentInChildren<PlayerProximitySensor>();
    }

    public void LookForTarget()
    {
        //Initialize closest distance squared with a high number to ensure that first target is always closer 
        float closestDistanceSquared = 1000f;

        //If last closest target has left the range, set closest target to null
        if (closestTarget)
        {
            if (!proximitySensor.targetsInRange.Contains(closestTarget))
            {
                closestTarget = null;
            }
        }

        //Iterate through all the valid targets within range
        foreach (Possessable target in proximitySensor.targetsInRange)
        {
            //Calculate player and target position in 2D
            Vector2 playerPosition2D = new Vector2(transform.position.x, transform. position.y);
            Vector2 targetPosition2D = new Vector2(target.transform.position.x, target.transform.position.y);

            //Compare distance to current target against the current shortest distance
            if (Utilities.DistanceBetweenVectorsSquared(playerPosition2D, targetPosition2D) < closestDistanceSquared)
            {
                //If current target is closer, register it as closest target and update closest distance
                closestTarget = target;
                closestDistanceSquared = Utilities.DistanceBetweenVectorsSquared(playerPosition2D, targetPosition2D);
            }
        }
    }

    public void Possession()
    {
        //If there is a valid target
        if (closestTarget)
        {
            states.isPossessing = true;
            StartCoroutine(WhilePossessing());
        }
    }

    IEnumerator WhilePossessing()
    {
        closestTarget.StartPossession();

        //Set possession timer to given value
        float timer = possessionTime;

        //Iterate for as long as the timer is running and the possessable gameobject is active
        while (timer > 0 && closestTarget.gameObject.activeInHierarchy)
        {
            closestTarget.WhilePossessed();
            timer -= Time.deltaTime;
            yield return null;
        }

        //End possession
        closestTarget.EndPossession();
        states.isPossessing = false;
        yield break;
    }
}
