using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Vector2 move;
    public float reverseMovementTime = 2f;
    public float timer;
    private Vector2 velocity;
    private Rigidbody2D rb;
    private HashSet<Rigidbody2D> passengerrbs;
    int direction = 1;

    private void Start()
    {
        passengerrbs = new HashSet<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > reverseMovementTime)
        {
            direction = -direction;
            timer = 0;
        }

        velocity = move * direction;
    }

    private void FixedUpdate()
    {
        rb.velocity = velocity;

        if (passengerrbs.Count != 0)
        {
            foreach (Rigidbody2D i in passengerrbs)
            {
                i.velocity += velocity;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            RemovePassengerRB(collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }

    public void AddPassengerRB(Rigidbody2D passengerrb)
    {
        if (!passengerrbs.Contains(passengerrb))
        {
            passengerrbs.Add(passengerrb);
        }
    }

    public void RemovePassengerRB(Rigidbody2D passengerrb)
    {
        if (passengerrbs.Contains(passengerrb))
        {
            passengerrbs.Remove(passengerrb);
        }
    }

    public static PlatformController CreateInitialized(GameObject where, Vector2 initializedMove, float initializedReverseMovementTime)
    {
        PlatformController pc = where.AddComponent<PlatformController>();
        pc.move = initializedMove;
        pc.reverseMovementTime = initializedReverseMovementTime;
        return pc;
    }
}
