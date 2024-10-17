using UnityEngine;

public class GeneralMovement : MonoBehaviour {
    private bool playerFacingRight = true;
    private bool m_IsFalling = false;

    public void Flip(Transform playerTransform) {
        playerFacingRight = !playerFacingRight;
        Vector3 theScale = playerTransform.localScale;
        theScale.x *= -1;
        playerTransform.localScale = theScale;
    }

    public bool IsFalling(Rigidbody2D playerBody) {
        return playerBody.linearVelocity.y < 0;
    }

    public bool GetIsFacingRight() {
        return playerFacingRight;
    }
}
