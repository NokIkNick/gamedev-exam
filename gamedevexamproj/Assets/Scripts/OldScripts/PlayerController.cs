using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using System;


public class PlayerController : MonoBehaviour
{
	[Header("Jump settings")]
	[SerializeField] private float coyoteTime = 0.1f; // Duration of coyote time
	private float coyoteTimeCounter = 0f;
  	[SerializeField] private float m_JumpForce = 1500f;
  	[SerializeField] private float m_doubleJumpForce = 1500f;					// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed modifier to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;	
	[SerializeField] private float wallHangDuration = 2f; // Duration to hang on the wall						// Whether or not a player can steer while jumping;
	private bool isHanging = false; 
    private float hangTimer = 0f; 
	const float k_WallCheckRadius = 0.5f;
	private bool isTouchingWall = false; 
	private bool isWallHanging = false; 
	[Header("Checks")]
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
	[SerializeField] private LayerMask m_WhatIsWall;   							// A mask determining what is a wall to the character
	[SerializeField] private Transform m_WallCheck;    							// A position marking where to check for walls

	[Header("Dash Settings")]
    [SerializeField] private float m_DashSpeed = 50f;    // Speed during dash
    [SerializeField] private float m_DashDuration = 0.2f; // Duration of the dash
    [SerializeField] private float m_DashCooldown = 1f;   // Cooldown time between dashes

    private bool m_IsDashing = false;
    private float m_LastDashTime = -Mathf.Infinity;
	private bool m_IsFalling = false;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private int m_JumpCount = 1;
	[SerializeField] private int maxJumpCount = 2;
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector2 m_Velocity = Vector2.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
	private Animator animator;

	void Start(){
   		 animator = GetComponent<Animator>();
	}
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
		CheckWallHang();
		// Manage coyote time counter
    	if (m_Grounded){
        // Reset the coyote timer if the player is grounded
        coyoteTimeCounter = coyoteTime;
    	} else {
        // Decrease the coyote timer while the player is not grounded
        coyoteTimeCounter -= Time.fixedDeltaTime;
    	}
		 // If the player is hanging, do not apply gravity
        if (isHanging) {
            hangTimer -= Time.fixedDeltaTime;
            if (hangTimer <= 0) {
                isHanging = false; // End wall hang after the timer expires
            }
        }
	}
	   private void CheckWallHang() {
        // Check for wall contact using the WallCheck Transform
        isTouchingWall = Physics2D.OverlapCircle(m_WallCheck.position, k_WallCheckRadius, m_WhatIsWall);
        if (isTouchingWall) {
            // Start wall hang
            if (!isHanging) {
                isHanging = true;
                hangTimer = wallHangDuration; // Reset the timer for wall hang duration
                m_Rigidbody2D.linearVelocity = Vector2.zero; // Stop vertical movement while hanging
            }
        }
    }


 	public void Move(float move, bool crouch, bool jump){

   		// Wall Hang Logic
    	if (isTouchingWall && !m_Grounded && move != 0){
        // Player is pressing towards the wall and in the air, start hanging
			Debug.Log("Wall Hanging");
        	isWallHanging = true;
       		 m_Rigidbody2D.linearVelocity = new Vector2(0, 0); // Stop player movement
		} else {
        	isWallHanging = false;
    	}

    	// If wall hanging, return without allowing movement or jumping
    	if (isWallHanging) {
        	return;
    	}

		// If crouching, check to see if the character can stand up
		if (!crouch){
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)){
				//crouch = true;
			}
		}

		 // Only control the player if grounded or airControl is turned on
    if (m_Grounded || m_AirControl) {
        // Handle crouching
        if (crouch) {
            // If transitioning to crouch
            if (!m_wasCrouching)
            {	
                m_wasCrouching = true;
                OnCrouchEvent.Invoke(true);
                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable the collider when crouching
                if (m_CrouchDisableCollider != null)
                {
                    m_CrouchDisableCollider.enabled = false;
                    Debug.Log("Collider disabled while crouching.");
                }
            }
        } else {
            // If transitioning out of crouch
            if (m_wasCrouching)
            {
                m_wasCrouching = false;
                OnCrouchEvent.Invoke(false);

                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                {
                    m_CrouchDisableCollider.enabled = true;
                    Debug.Log("Collider enabled after standing up.");
                }
            }
        }

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.linearVelocity = Vector2.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight){
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight){
				// ... flip the player.
				Flip();
			}
		}
		  // Jump logic
    if (jump){
            if (m_Grounded || coyoteTimeCounter > 0f){
                // First jump
                m_Grounded = false;
				coyoteTimeCounter = 0f;
                m_JumpCount = 1;  // Set jump count to 1 after the first jump
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            else if (m_JumpCount < maxJumpCount){
                // Double jump
                m_JumpCount++; // Increase jump count
                m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0f); // Reset y velocity
                m_Rigidbody2D.AddForce(new Vector2(0f, m_doubleJumpForce));
            } else if (isTouchingWall){
				// If the player is wall hanging, they can jump off the wall
				Debug.Log("Jumping off wall");
            	isWallHanging = false; // Stop wall hanging after jumping
            	m_JumpCount = 1; // Start counting jumps again
            	Vector2 jumpDirection = new Vector2(-move * 10f, 1f).normalized; // Push away from wall
            	m_Rigidbody2D.AddForce(jumpDirection * m_JumpForce);
			}
		}
	}

	public void Dash()
    {
        if (Time.time < m_LastDashTime + m_DashCooldown)
        {
            return;
        }

        m_LastDashTime = Time.time;
        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        m_IsDashing = true;
        float originalGravity = m_Rigidbody2D.gravityScale;
        m_Rigidbody2D.gravityScale = 0; // Temporarily disable gravity during dash

        // Set dash velocity
        float dashDirection = m_FacingRight ? 1 : -1;
        m_Rigidbody2D.linearVelocity = new Vector2(dashDirection * m_DashSpeed, 0f);

        yield return new WaitForSeconds(m_DashDuration);

        // Reset gravity and stop dash
        m_Rigidbody2D.gravityScale = originalGravity;
        m_IsDashing = false;
    }
	private void Flip(){
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	public bool IsGrounded(){
		return m_Grounded;
	}

    public bool IsFalling(){
		if(m_Rigidbody2D.linearVelocity.y < 0){
			m_IsFalling = true;
		} else {
			m_IsFalling = false;
		}
        return m_IsFalling;
    }

	private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_CeilingCheck.position, k_CeilingRadius);
    }
}