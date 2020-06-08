using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    public float direction = 1;
    public bool isSqueezedUpright;
    public bool isSqueezedLying;
    public bool isColliderLying;
    public bool isColliderUpright = true;
    public bool isColliderBunched;
    public bool isInvincible;
    public bool isTouchingGround;
    public bool isTouchingCeiling;
    public bool isTouchingWallInFront;
    public bool isTouchingWallBehind;
    public bool isAffectedByGravity;
    public bool isPossessed;
    public LayerMask whatIsDamage;
    public LayerMask whatIsPlatform;
}
