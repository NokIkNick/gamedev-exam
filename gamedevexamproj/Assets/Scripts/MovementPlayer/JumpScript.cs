using UnityEngine;
using UnityEngine.Events;

public class JumpScript : MonoBehaviour {

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip doubleJumpSound;
    private int m_JumpCount = 0;
    public UnityEvent OnLandEvent;
    private bool m_Grounded;
	const float k_GroundedRadius = .2f;
    private float m_CoyoteTimeCounter = 0f;
    
    private void Awake() {
        OnLandEvent ??= new UnityEvent();
    }
    public void Jump(Rigidbody2D m_Rigidbody2D, float jumpForce,float m_DoubleJumpForce, int maxJumpCount, AudioSource audioSource) {
            //Debug.Log("Jump");

          if (m_Grounded || m_CoyoteTimeCounter > 0f){
                m_Grounded = false;
				m_CoyoteTimeCounter = 0f;
                m_JumpCount++;
                audioSource.PlayOneShot(jumpSound);
                m_Rigidbody2D.AddForce(new Vector2(0f, jumpForce),ForceMode2D.Impulse);
            }
            else if (m_JumpCount < maxJumpCount){
                m_JumpCount++;
                audioSource.PlayOneShot(doubleJumpSound);
                m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0f); // Reset y velocity
                m_Rigidbody2D.AddForce(new Vector2(0f, m_DoubleJumpForce),ForceMode2D.Impulse);
            }
    }
    public void WallJump(Rigidbody2D m_Rigidbody2D, float m_JumpForce) {
       // Debug.Log("Wall Jump");
        m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0f); // Reset y velocity
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce),ForceMode2D.Impulse);
    }
    
    public void GroundCheck(LayerMask m_WhatIsGround, Transform m_GroundCheck) {
            bool wasGrounded = m_Grounded;
		    m_Grounded = false;
		    Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		    for (int i = 0; i < colliders.Length; i++) {
			    if (colliders[i].gameObject != gameObject) {
				    m_Grounded = true;
                    m_JumpCount = 0;
				    if(!wasGrounded)
					    OnLandEvent.Invoke();
			    }
		    }
    }
    public void ManageCoytoeTime(float coyoteTime) {
            if (m_Grounded){
                m_CoyoteTimeCounter = coyoteTime;
    	    } else {
                m_CoyoteTimeCounter -= Time.fixedDeltaTime;
    	    }
    }
    public bool IsGrounded() {
		return m_Grounded;
	}
}
