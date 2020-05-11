using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceChecker : MonoBehaviour
{
    private BoxCollider2D bc;
    private States states;

    private float smallerDimension;
    private float biggerDimension;

    private void Start()
    {
        states = GetComponent<States>();
        bc = GetComponent<BoxCollider2D>();
        smallerDimension = Mathf.Min(bc.size.x, bc.size.y);
        biggerDimension = Mathf.Max(bc.size.x, bc.size.y);
    }

    public bool HasSpaceUpright()
    {
        return !Physics2D.OverlapBox(new Vector2(transform.position.x + (bc.size.x / 2 - bc.size.y / 2) * states.direction, 
                                                 transform.position.y + (bc.size.x / 2 - bc.size.y / 2)), 
                                     new Vector2(smallerDimension - 0.02f, biggerDimension - 0.02f), 
                                     0f, 
                                     states.whatIsPlatform | states.whatIsDamage);
    }

    public bool HasSpaceLying()
    {
        return !Physics2D.OverlapBox(new Vector2(transform.position.x + (bc.size.y / 2 - bc.size.x / 2) * states.direction, 
                                                 transform.position.y - (bc.size.y / 2 - bc.size.x / 2)), 
                                     new Vector2(biggerDimension - 0.02f, smallerDimension - 0.02f), 
                                     0f, 
                                     states.whatIsPlatform | states.whatIsDamage);
    }

    private void Update()
    {
        states.isSqueezedLying = !HasSpaceLying();
        states.isSqueezedUpright = !HasSpaceUpright();
    }
}
