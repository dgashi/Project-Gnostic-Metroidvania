using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
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
    
    [HideInInspector]
    public BoxCollider2D bc;

    [SerializeField]
    public virtual void Start()
    {
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
