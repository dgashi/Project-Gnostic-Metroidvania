using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleTarget : MonoBehaviour
{
    public enum TargetType
    {
        Player,
        Decoy
    }

    public TargetType type;
}
