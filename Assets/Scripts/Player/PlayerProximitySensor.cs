using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProximitySensor : MonoBehaviour
{
    public HashSet<Possessable> targetsInRange;

    CircleCollider2D detectionTrigger;

    public float detectionDistance;

    void Start()
    {
        targetsInRange = new HashSet<Possessable>();

        detectionTrigger = GetComponent<CircleCollider2D>();

        detectionTrigger.radius = detectionDistance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if triggering object is a valid target
        Possessable target = collision.GetComponent<Possessable>();

        //If a valid target enters the detection range, add them to valid targets
        if (target)
        {
            targetsInRange.Add(target);
            Debug.Log(target.gameObject.name + " entered range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Check if triggering object is a valid target
        Possessable target = collision.GetComponent<Possessable>();

        //If a valid target exits the detection range, remove them from valid targets
        if (target && targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
            Debug.Log(target.gameObject.name + " is out of range");
        }
    }
}
