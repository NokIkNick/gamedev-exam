using Unity.VisualScripting;
using UnityEngine;

public class GeneralMovement : MonoBehaviour
{
	private bool playerFacingRight = true;
	private Vector2 m_Velocity = Vector2.zero;
    private bool m_IsFalling = false;
    internal object playerBody;

    public void MoveHori(float move, float movementSmoothing, Rigidbody2D playerBody,bool isWallHanging){
			
			if(!isWallHanging) {
				//Debug.Log("Move");
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, playerBody.linearVelocity.y);
				// And then smoothing it out and applying it to the character
				playerBody.linearVelocity = Vector2.SmoothDamp(playerBody.linearVelocity, targetVelocity, ref m_Velocity, movementSmoothing);
			}

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !playerFacingRight) {
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && playerFacingRight) {
				// ... flip the player.
				Flip();
			}

    }
    public void Flip() {
		    // Switch the way the player is labelled as facing.
		    playerFacingRight = !playerFacingRight;
		    Vector3 theScale = transform.localScale;
		    theScale.x *= -1;
		    transform.localScale = theScale;
			Debug.Log("Now facing " + (playerFacingRight ? "right" : "left"));
	}

    public bool IsFalling(Rigidbody2D m_Rigidbody2D){
		if(m_Rigidbody2D.linearVelocity.y < 0){
			m_IsFalling = true;
		} else {
			m_IsFalling = false;
		}
        return m_IsFalling;
    }
	public bool GetIsFacingRight(){
		return playerFacingRight;
	}
}
