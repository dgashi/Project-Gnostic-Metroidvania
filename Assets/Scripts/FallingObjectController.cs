using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectController : MonoBehaviour
{
    public float gravity;

    public LayerMask whatIsPlatform;

    public GameObject aboveColliderObject;
    private EdgeCollider2D aboveCollider;
    [HideInInspector]
    public EdgeInput aboveInput;

    public GameObject belowColliderObject;
    private EdgeCollider2D belowCollider;
    [HideInInspector]
    public EdgeInput belowInput;

    public GameObject frontColliderObject;
    private EdgeCollider2D frontCollider;
    [HideInInspector]
    public EdgeInput frontInput;

    public GameObject backColliderObject;
    private EdgeCollider2D backCollider;
    [HideInInspector]
    public EdgeInput backInput;

    public Vector2 velocity;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public BoxCollider2D bc;

    [SerializeField]
    private PlatformController platformBelowController;

    [SerializeField]
    private bool isTouchingGround;
    [SerializeField]
    private bool isTouchingCeiling;
    [SerializeField]
    private bool isTouchingWallInFront;

    private Vector3 lastPosition;

    [SerializeField]
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        aboveInput = aboveColliderObject.GetComponent<EdgeInput>();
        belowInput = belowColliderObject.GetComponent<EdgeInput>();
        frontInput = frontColliderObject.GetComponent<EdgeInput>();
        backInput = backColliderObject.GetComponent<EdgeInput>();

        aboveCollider = aboveColliderObject.GetComponent<EdgeCollider2D>();
        belowCollider = belowColliderObject.GetComponent<EdgeCollider2D>();
        frontCollider = frontColliderObject.GetComponent<EdgeCollider2D>();
        backCollider = backColliderObject.GetComponent<EdgeCollider2D>();

        UpdateEdgeCollidersUpright();

        lastPosition = rb.position;
    }


    public virtual void Update()
    {
        isTouchingGround = belowInput.isColliding;
        isTouchingCeiling = aboveInput.isColliding;
        isTouchingWallInFront = frontInput.isColliding;

        //Attach to moving platform below
        if (!rb.isKinematic)
        {
            if (belowInput.collidingPlatformControllers.Count != 0)
            {
                if (!belowInput.collidingPlatformControllers.Contains(platformBelowController))
                {
                    platformBelowController = belowInput.collidingPlatformControllers[belowInput.collidingPlatformControllers.Count - 1];

                    Debug.Log(platformBelowController.gameObject.name);
                }
            }
            else
            {
                platformBelowController = null;
            }

            if (belowInput.isColliding && platformBelowController != null)
            {
                platformBelowController.AddPassengerRB(rb);
            }
        }

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

        //Draw line for debugging
        Debug.DrawLine(lastPosition, transform.position, Color.white, 1f);
        lastPosition = transform.position;
    }


    private void LateUpdate()
    {
        rb.velocity = velocity;
    }


    public void UpdateEdgeCollidersLying()
    {
        
        List<Vector2> newPointsHorizontal = new List<Vector2>();
        List<Vector2> newPointsVertical = new List<Vector2>();

        newPointsHorizontal.Add(new Vector2(-(bc.size.x / 2 - 0.015f), 0));
        newPointsHorizontal.Add(new Vector2(bc.size.x / 2 - 0.015f, 0));

        newPointsVertical.Add(new Vector2(0, -(bc.size.y / 2 - 0.015f)));
        newPointsVertical.Add(new Vector2(0, bc.size.y / 2 - 0.015f));

        aboveCollider.transform.localPosition = new Vector3(0, bc.size.y / 2, aboveCollider.transform.localPosition.z);
        belowCollider.transform.localPosition = new Vector3(0, -bc.size.y / 2, aboveCollider.transform.localPosition.z);
        frontCollider.transform.localPosition = new Vector3(bc.size.x / 2, 0, aboveCollider.transform.localPosition.z);
        backCollider.transform.localPosition = new Vector3(-bc.size.x / 2, 0, aboveCollider.transform.localPosition.z);

        aboveCollider.points = newPointsHorizontal.ToArray();
        belowCollider.points = newPointsHorizontal.ToArray();
        frontCollider.points = newPointsVertical.ToArray();
        backCollider.points = newPointsVertical.ToArray();
    }

    public void UpdateEdgeCollidersUpright()
    {
        List<Vector2> newPointsHorizontal = new List<Vector2>();
        List<Vector2> newPointsVertical = new List<Vector2>();

        newPointsHorizontal.Add(new Vector2(-(bc.size.x / 2 - 0.015f), 0));
        newPointsHorizontal.Add(new Vector2(bc.size.x / 2 - 0.015f, 0));

        newPointsVertical.Add(new Vector2(0, -(bc.size.y / 2 - 0.015f)));
        newPointsVertical.Add(new Vector2(0, bc.size.y / 2 - 0.015f));

        aboveCollider.transform.localPosition = new Vector3(0, bc.size.y / 2, aboveCollider.transform.localPosition.z);
        belowCollider.transform.localPosition = new Vector3(0, -bc.size.y / 2, aboveCollider.transform.localPosition.z);
        frontCollider.transform.localPosition = new Vector3(bc.size.x / 2, 0, aboveCollider.transform.localPosition.z);
        backCollider.transform.localPosition = new Vector3(-bc.size.x / 2, 0, aboveCollider.transform.localPosition.z);

        aboveCollider.points = newPointsHorizontal.ToArray();
        belowCollider.points = newPointsHorizontal.ToArray();
        frontCollider.points = newPointsVertical.ToArray();
        backCollider.points = newPointsVertical.ToArray();
    }
}
