using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    public float direction = 1;
    public bool isSqueezedUpright;
    public bool isSqueezedLying;
    public bool isColliderLying;
    public bool isColliderUpright;
    public bool isColliderBunched;
    public bool isInvincible;
    public bool isTouchingGround;
    public bool isTouchingCeiling;
    public bool isTouchingWallInFront;
    public bool isTouchingWallBehind;
    public bool isAffectedByGravity;
    public LayerMask whatIsDamage;
    public LayerMask whatIsPlatform;
}
