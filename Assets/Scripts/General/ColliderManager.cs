using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private EdgeCollider2D aboveCollider;
    private EdgeInput aboveInput;
    
    private EdgeCollider2D belowCollider;
    private EdgeInput belowInput;
    
    private EdgeCollider2D frontCollider;
    private EdgeInput frontInput;
    
    private EdgeCollider2D backCollider;
    private EdgeInput backInput;
    
    private BoxCollider2D bc;

    [SerializeField]
    public virtual void Start()
    {
        bc = GetComponent<BoxCollider2D>();

        foreach (EdgeInput i in GetComponentsInChildren<EdgeInput>())
        {
            switch (i.colliderPosition)
            {
                case EdgeInput.Direction.Above:
                    aboveInput = i;
                    aboveCollider = i.gameObject.GetComponent<EdgeCollider2D>();
                    break;
                case EdgeInput.Direction.Below:
                    belowInput = i;
                    belowCollider = i.gameObject.GetComponent<EdgeCollider2D>();
                    break;
                case EdgeInput.Direction.Front:
                    frontInput = i;
                    frontCollider = i.gameObject.GetComponent<EdgeCollider2D>();
                    break;
                case EdgeInput.Direction.Back:
                    backInput = i;
                    backCollider = i.gameObject.GetComponent<EdgeCollider2D>();
                    break;
            }
        }

        UpdateEdgeColliders();
    }

    public void UpdateEdgeColliders()
    {
        List<Vector2> newPointsHorizontal = new List<Vector2>();
        List<Vector2> newPointsVertical = new List<Vector2>();

        newPointsHorizontal.Add(new Vector2(-(bc.size.x / 2 - 0.015f), 0));
        newPointsHorizontal.Add(new Vector2(bc.size.x / 2 - 0.015f, 0));

        newPointsVertical.Add(new Vector2(0, -(bc.size.y / 2 - 0.015f)));
        newPointsVertical.Add(new Vector2(0, bc.size.y / 2 - 0.015f));

        aboveCollider.transform.localPosition = new Vector3(bc.offset.x, (bc.size.y / 2) + bc.offset.y, aboveCollider.transform.localPosition.z);
        belowCollider.transform.localPosition = new Vector3(bc.offset.x, -(bc.size.y / 2) + bc.offset.y, aboveCollider.transform.localPosition.z);
        frontCollider.transform.localPosition = new Vector3((bc.size.x / 2) + bc.offset.x, bc.offset.y, aboveCollider.transform.localPosition.z);
        backCollider.transform.localPosition = new Vector3(-(bc.size.x / 2) + bc.offset.x, bc.offset.y, aboveCollider.transform.localPosition.z);

        aboveCollider.points = newPointsHorizontal.ToArray();
        belowCollider.points = newPointsHorizontal.ToArray();
        frontCollider.points = newPointsVertical.ToArray();
        backCollider.points = newPointsVertical.ToArray();
    }
}
