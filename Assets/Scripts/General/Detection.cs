using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This class is attached to an eye object on a character.
It registers all objects of the type specified by the parent's
TargetChoosing component that enter the detection range
trigger, and runs the more specific detection calculations
on them when called. It has a hashset of all targets that
are actually visible, which is returned to the parent object.
*/

public class Detection : MonoBehaviour
{
    HashSet<VisibleTarget> targetsInRange;
    HashSet<VisibleTarget> seenTargets;
    public LayerMask whatIsPlatform;
    public LayerMask whatIsCharacters;

    CircleCollider2D detectionTrigger;

    public float detectionDistance;
    public float detectionAngle;

    TargetChoosing targetChoosing;

    void Start()
    {
        targetsInRange = new HashSet<VisibleTarget>();
        seenTargets = new HashSet<VisibleTarget>();

        detectionTrigger = GetComponent<CircleCollider2D>();
        targetChoosing = transform.parent.GetComponent<TargetChoosing>();

        detectionTrigger.radius = detectionDistance;
    }

    private void Update()
    {
        //Draw lines to visualize field of view
        Debug.DrawLine(transform.position, transform.position + transform.up * detectionDistance, Color.red, .0001f);

        //broken
        for (int i = -1; i < 2; i = i + 2)
        {
            float debugLineAngle = Mathf.Deg2Rad * (detectionAngle * 0.5f * i);
            Vector2 debugLineAngleVector = new Vector3(Mathf.Cos(debugLineAngle), Mathf.Sin(debugLineAngle));
            Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + debugLineAngleVector * detectionDistance, Color.blue, .0001f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if triggering object is a valid target
        VisibleTarget target = collision.GetComponent<VisibleTarget>();

        //If a valid target enters the detection range, add them to valid targets
        if (target && targetChoosing.priorities.Contains(target.type))
        {
            targetsInRange.Add(target);
            Debug.Log(target.gameObject.name + " entered range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Check if triggering object is a valid target
        VisibleTarget target = collision.GetComponent<VisibleTarget>();

        //If a valid target exits the detection range, remove them from valid targets
        if (target && targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
            Debug.Log(target.gameObject.name + " is out of range");
        }

        //Also remove them from actually visible targets
        if (target && seenTargets.Contains(target))
        {
            seenTargets.Remove(target);
        }
    }

    //Goes through all valid targets within range and filters out the non-visible ones
    public HashSet<VisibleTarget> ScanForTargets()
    {
        //Go through all valid targets within detection distance
        foreach (VisibleTarget i in targetsInRange)
        {
            //Calculate vector from eye to target position
            Vector2 directionToTarget = new Vector2(i.transform.position.x - transform.position.x, i.transform.position.y - transform.position.y).normalized;

            //If the target is within the detection angle
            if (Vector2.Angle(transform.up, directionToTarget) <= detectionAngle * 0.5f)
            {
                Debug.Log(i.name + " is within detection angle");

                //Linecast to target to see if there are walls or other characters between them
                RaycastHit2D hitInfo = Physics2D.Linecast(transform.position, new Vector2(i.transform.position.x, i.transform.position.y), whatIsCharacters | whatIsPlatform);

                //If nothing is in the way, add the target to actually visible targets
                if (hitInfo && hitInfo.collider.gameObject == i.gameObject)
                {
                    if (!seenTargets.Contains(i))
                    {
                        seenTargets.Add(i);
                        Debug.Log(hitInfo.collider.gameObject.name + " is visible");
                    }

                    //Draw line to seen target for debug purposes
                    Debug.DrawLine(transform.position, i.transform.position, Color.blue, .0001f);
                }
                //If something blocks the view, remove target from visible targets
                else if (seenTargets.Contains(i))
                {
                    seenTargets.Remove(i);
                    Debug.Log(i.name + " is hidden behind something");
                }
            }
            //If target is not within detection angle, remove target from visible targets
            else if (seenTargets.Contains(i))
            {
                seenTargets.Remove(i);
                Debug.Log(i.name + " is outside field of view");
            }
        }

        return seenTargets;
    }
}
