using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Possessable : MonoBehaviour
{
    public abstract void StartPossession();
    public abstract void WhilePossessed();
    public abstract void EndPossession();
}
