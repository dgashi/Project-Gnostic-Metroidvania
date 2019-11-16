using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemyController : FallingObjectController
{
    public float moveSpeed;
    public float acceleration;
    public float detectionDistance;
    public int numberOfRays;
    public LayerMask whatIsCharacters;
    private float direction;
    [SerializeField]
    private bool targetAvailable;

    [SerializeField]
    private Transform target;
    SpriteRenderer sr;

    public override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        if (Time.timeScale != 0)
        {
            base.Update();

            //Apply gravity;
            if (velocity.y <= 0)
            {
                velocity.y += -gravity * 3 * Time.deltaTime;
            }
            else
            {
                velocity.y += -gravity * Time.deltaTime;
            }

            //Stop moving if hitting ceiling or ground
            if (velocity.y > 0 && aboveInput.isColliding)
            {
                velocity.y = 0;
            }

            if (velocity.y < 0 && belowInput.isColliding)
            {
                velocity.y = 0;
            }

            //Remove target if it is deactivated
            if (target != null)
            {
                if (!target.gameObject.activeInHierarchy)
                {
                    target = null;
                }
            }

            //Find targets
            for (int i = 0; i < numberOfRays; i++)
            {
                float angle = Mathf.Deg2Rad * (360f / numberOfRays) * i;
                Vector2 angleVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, angleVector, detectionDistance, whatIsPlatform | whatIsCharacters);

                if (hitInfo.collider == null)
                {
                    Debug.DrawLine(transform.position, new Vector2(transform.position.x + angleVector.x * detectionDistance, transform.position.y + angleVector.y * detectionDistance), Color.blue, .0001f);
                }
                else
                {
                    if (hitInfo.collider.CompareTag("Decoy"))
                    {
                        target = hitInfo.collider.gameObject.transform;
                    }
                    else if (target == null && hitInfo.collider.gameObject.CompareTag("Player"))
                    {
                        target = hitInfo.collider.gameObject.transform;
                    }

                    Debug.DrawLine(transform.position, new Vector2(transform.position.x + angleVector.x * hitInfo.distance, transform.position.y + angleVector.y * hitInfo.distance), Color.blue, .0001f);
                }
            }

            //Move towards target
            if (target == null)
            {
                sr.color = Color.white;
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * acceleration);
            }
            else
            {
                sr.color = Color.red;
                Vector2 distanceToTarget = target.position - transform.position;
                direction = Mathf.Sign(distanceToTarget.x);
                velocity.x = Mathf.Lerp(velocity.x, moveSpeed * direction, Time.deltaTime * acceleration);

                if (Utilities.DistanceBetweenVectorsSquared(transform.position, target.position) > Mathf.Pow(detectionDistance, 2))
                {
                    target = null;
                }
            }
        }
    }
}
