using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyController : FallingObjectController
{
    public bool isInvincible = false;

    public override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
    }

    public override void Update()
    {

        if (Time.timeScale !=0)
        {
            base.Update();
        }

        if ((aboveInput.isColliding && belowInput.isColliding) || (frontInput.isColliding && backInput.isColliding))
        {
            gameObject.SetActive(false);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Environmental Damage 1") || collision.gameObject.CompareTag("Demon Damage 1")) && !isInvincible)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        velocity = new Vector2(0, 0);
    }
}
