using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamager : MonoBehaviour
{
    [SerializeField]
    private Damage.DamageType damageType;
    [SerializeField]
    private int amount;

    private HashSet<GameObject> damagedAlready;

    private void Start()
    {
        damagedAlready = new HashSet<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damagable damagable = collision.gameObject.GetComponent<Damagable>();

        if (damagable && !damagedAlready.Contains(collision.gameObject))
        {
            damagable.TakeDamage(damageType, amount);
            damagedAlready.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (damagedAlready.Contains(collision.gameObject))
        {
            damagedAlready.Remove(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damagable damagable = collision.gameObject.GetComponent<Damagable>();

        if (damagable && !damagedAlready.Contains(collision.gameObject))
        {
            damagable.TakeDamage(damageType, amount);
            damagedAlready.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (damagedAlready.Contains(collision.gameObject))
        {
            damagedAlready.Remove(collision.gameObject);
        }
    }
}
