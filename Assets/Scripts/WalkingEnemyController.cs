using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemyController : FallingObjectController
{
    public float moveSpeed;
    public float acceleration;
    public float detectionDistance;
    private float direction;

    public bool isInvincible = false;

    public LayerMask whatIsCharacters;
    public Vector2 eyePosition;
    public float eyeAngle;
    public float fieldOfView;
    public HashSet<Transform> possibleTargets;
    public HashSet<Transform> seenTargets;
    public HashSet<Transform> toBeRemovedFromSeenTargets;

    [SerializeField]
    private Transform target;
    SpriteRenderer sr;

    private void Awake()
    {
        possibleTargets = new HashSet<Transform>();

        possibleTargets.Add(GameObject.FindGameObjectsWithTag("Player")[0].transform);

        if (GameObject.FindGameObjectsWithTag("Decoy").Length != 0)
        {
            possibleTargets.Add(GameObject.FindGameObjectsWithTag("Decoy")[0].transform);
        }
    }

    public override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
        seenTargets = new HashSet<Transform>();
        toBeRemovedFromSeenTargets = new HashSet<Transform>();
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
            Vector2 localEyePosition = new Vector2(transform.position.x, transform.position.y) + new Vector2(eyePosition.x, eyePosition.y);
            float eyeAngleInRadians = Mathf.Deg2Rad * eyeAngle;
            Vector2 eyeAngleVector = new Vector2(Mathf.Cos(eyeAngleInRadians), Mathf.Sin(eyeAngleInRadians));

            foreach (Transform i in possibleTargets)
            {
                Vector2 directionToTarget = (new Vector2(i.position.x, i.position.y) - localEyePosition).normalized;

                if (Utilities.DistanceBetweenVectorsSquared(localEyePosition, new Vector2(i.position.x, i.position.y)) <= Mathf.Pow(detectionDistance, 2))
                {
                    if (Vector2.Angle(eyeAngleVector, directionToTarget) <= fieldOfView * 0.5f)
                    {
                        //RaycastHit2D hitInfo = Physics2D.Raycast(localEyePosition, new Vector2(i.position.x, i.position.y) - localEyePosition, detectionDistance, whatIsCharacters | whatIsPlatform);
                        RaycastHit2D hitInfo = Physics2D.Linecast(localEyePosition, new Vector2(i.position.x, i.position.y), whatIsCharacters | whatIsPlatform);

                        if (hitInfo && hitInfo.collider.transform == i)
                        {
                            if (!seenTargets.Contains(i))
                            {
                                seenTargets.Add(i);
                                Debug.Log(hitInfo.collider.gameObject.name + " is visible");
                            }
                            Debug.DrawLine(localEyePosition, i.position, Color.blue, .0001f);
                        }
                        else if (seenTargets.Contains(i))
                        {
                            seenTargets.Remove(i);
                            Debug.Log(i.name + " is hidden behind something");
                        }
                    }
                    else if (seenTargets.Contains(i))
                    {
                        seenTargets.Remove(i);
                        Debug.Log(i.name + " is outside field of view");
                    }
                }
                else if (seenTargets.Contains(i))
                {
                    seenTargets.Remove(i);
                    Debug.Log(i.name + " is out of range");
                }
            }

            //Draw borders and center of field of view for debugging
            Vector2 localEyeAngleVector = new Vector2(Mathf.Cos(eyeAngleInRadians) * detectionDistance + localEyePosition.x, Mathf.Sin(eyeAngleInRadians) * detectionDistance + localEyePosition.y);

            Debug.DrawLine(localEyePosition, localEyeAngleVector, Color.red, .0001f);

            for (int i = -1; i < 2; i = i + 2)
            {
                float debugLineAngle = Mathf.Deg2Rad * (eyeAngle + fieldOfView * 0.5f * i);
                Vector2 debugLineAngleVector = new Vector3(Mathf.Cos(debugLineAngle), Mathf.Sin(debugLineAngle));
                Debug.DrawLine(localEyePosition, localEyePosition + debugLineAngleVector * detectionDistance, Color.blue, .0001f);
            }

            //Choose target from seenTargets and mark non-active ones to be removed
            foreach (Transform i in seenTargets)
            {
                if (!i.gameObject.activeInHierarchy)
                {
                    toBeRemovedFromSeenTargets.Add(i);
                }

                if (i.gameObject.CompareTag("Decoy") && (target == null || !target.CompareTag("Decoy")))
                {
                    target = i;
                }
                else if (i.gameObject.CompareTag("Player") && (target == null || !target.CompareTag("Decoy") && !target.CompareTag("Player")))
                {
                    target = i;
                }
            }

            //Remove non-active targets
            foreach (Transform i in toBeRemovedFromSeenTargets)
            {
                Debug.Log(i.name + " was removed");
                seenTargets.Remove(i);
            }

            toBeRemovedFromSeenTargets.Clear();

            //If no targets have been seen, set target to null
            if (seenTargets.Count == 0)
            {
                target = null;
            }

            //Move towards target
            if (target == null)
            {
                //sr.color = Color.white;
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * acceleration);
            }
            else
            {
                //sr.color = Color.red;
                Vector2 distanceToTarget = target.position - transform.position;
                direction = Mathf.Sign(distanceToTarget.x);
                velocity.x = Mathf.Lerp(velocity.x, moveSpeed * direction, Time.deltaTime * acceleration);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environmental Damage 1") && !isInvincible)
        {
            gameObject.SetActive(false);
        }
    }
}
