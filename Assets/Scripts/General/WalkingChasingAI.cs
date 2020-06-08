using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingChasingAI : AI
{
    TargetChoosing targetChoosing;
    Movement movement;
    States states;

    //How many seconds to wait before next check
    public float targetCheckRate;
    private float targetCheckTimer;

    public VisibleTarget currentTarget;

    void Start()
    {
        targetChoosing = GetComponent<TargetChoosing>();
        movement = GetComponent<Movement>();
        states = GetComponent<States>();

        targetCheckTimer = targetCheckRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCheckTimer <= 0)
        {
            currentTarget = targetChoosing.CheckForTarget();
        }
        else
        {
            targetCheckTimer -= Time.deltaTime;
        }

        if (currentTarget)
        {
            float targetDirection = -Mathf.Sign(transform.position.x - currentTarget.transform.position.x);
            movement.SetVelocityX(targetDirection, true);
        }
        else
        {
            movement.SetVelocityX(0, true);
        }
    }

    public override void Deactivate()
    {
        Debug.Log("AI deactivated");
        enabled = false;
    }

    public override void Activate()
    {
        Debug.Log("AI activated");
        enabled = true;
    }
}
