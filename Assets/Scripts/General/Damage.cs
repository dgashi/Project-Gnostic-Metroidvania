using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public enum DamageType
    {
        Normal,
        Fire,
        Healing
    }

    public DamageType type;
    public int amount;
}
