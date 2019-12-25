using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyController : FallingObjectController
{
    public bool isInvincible = false;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if (Time.timeScale !=0)
        {
            base.Update();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("1 Damage") && !isInvincible)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        velocity = new Vector2(0, 0);
    }
}
