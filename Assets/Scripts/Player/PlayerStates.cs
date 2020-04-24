using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public bool isColliderLying;
    public bool isColliderUpright;
    public bool isColliderBunched;
    public bool isGrabbed = false;
    public bool isSliding = false;
    public bool isCloseToGround;
    public bool isCloseToWall;
    public bool isSqueezedUpright;
    public bool isSqueezedLying;
    public bool isSqueezedTeleport = false;
    public bool hasAirJumped = false;
    public bool hasTeleported = false;
    public bool dontStopX = false;
    public bool dontMoveX = false;
    public bool isInvincible;
}
