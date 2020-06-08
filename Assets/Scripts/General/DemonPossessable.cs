using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonPossessable : Possessable
{
    AI ai;
    States states;
    Movement movement;

    void Start()
    {
        ai = GetComponent<AI>();
        states = GetComponent<States>();
        movement = GetComponent<Movement>();
    }

    public override void StartPossession()
    {
        states.isPossessed = true;
        ai.Deactivate();
        movement.SetVelocity(0, 0, false);
    }

    public override void WhilePossessed()
    {
        Debug.Log(gameObject.name + " is being possessed");
    }

    public override void EndPossession()
    {
        states.isPossessed = false;
        ai.Activate();
        Debug.Log(gameObject.name + " broke free of possession");
    }
}
