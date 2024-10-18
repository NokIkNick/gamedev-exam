using UnityEngine;

public class CrouchScript : MonoBehaviour {

    private float headCheckRadius;
     private Collider2D standingCollider;
    private LayerMask whatIsGround;
    private Transform headCheck;

    private bool isCrouching = false;
    private bool shouldStandUp = true;

    public void Initialize(Collider2D standingCollider,float headCheckRadius, Transform headCheck, LayerMask whatIsGround) {
        this.standingCollider = standingCollider;
        this.headCheckRadius = headCheckRadius;
        this.headCheck = headCheck;
        this.whatIsGround = whatIsGround;
    }
    public void StartCrouch() {
        isCrouching = true;
        standingCollider.enabled = false;
        shouldStandUp = false;
    }

    public void StopCrouch() {
        shouldStandUp = true;
        if (!IsObstacleAbove()) {
            isCrouching = false;
            standingCollider.enabled = true;
        }
    }
    public bool IsObstacleAbove() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(headCheck.position, headCheckRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject) {
                return true;
            }
        }
        return false;
    }
    public bool GetIsCrouching() {
        return isCrouching;
    }
    public bool GetShouldStandUp() {
        return shouldStandUp;
    }
}
