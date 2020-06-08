using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyHealth : Damagable
{
    public override void TakeDamage(Damage.DamageType type, int amount)
    {
        if (type != Damage.DamageType.Healing && amount > 0)
        {
            gameObject.SetActive(false);
        }
    }
}
