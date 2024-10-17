using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    ///////////////General Movement Variables///////////////////
    [Header("Movement Settings")]
    [SerializeField] private float horizontalMove = 0f;
    [SerializeField] private float runSpeed = 60f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    private Vector2 movementInput;
    private Rigidbody2D playerBody;

    private Vector2 m_Velocity = Vector2.zero;
    ////////////////////////////////////////////////////////////
    
    ///////////////Jump Variables///////////////////////////////
    [Header("Jump Settings")]
    [SerializeField] private float playerJumpForce = 25f;
    [SerializeField] private float playerdoubleJumpForce = 25f;
    [SerializeField] private bool playerAirControl = true;
    [SerializeField] private int maxJumpCount = 1;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float originalGravityScale = 3f;

    /////////////////////////////////////////////////////////////
    
    ///////////////Crouch Variables//////////////////////////////
    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeed = 30f;
    [SerializeField] private float headCheckRadius = 0.2f;
    [SerializeField] private Collider2D standingCollider;
    private bool crouchButtonPressed = false;
    ////////////////////////////////////////////////////////////
    
    ///////////////Dash Variables//////////////////////////////
    [Header("Dash Settings")]
    [SerializeField] private float m_DashSpeed = 500f; 
    [SerializeField] private float m_DashDuration = 0.3f;
    [SerializeField] private float m_DashCooldown = 1.0f;

    /////////////////////////////////////////////////////////////
    
    ///////////////Wall Hang Variables///////////////////////////
    [Header("Wall Hang Settings")]
    [SerializeField] private float wallJumpForce = 5f;
    /////////////////////////////////////////////////////////////

    ///////////////Checks////////////////////////////////////////
    [Header("Checks")]
    [SerializeField] private LayerMask whatIsGround;						
	[SerializeField] private Transform groundCheck;
    [SerializeField] private Transform headCheck;
    ////////////////////////////////////////////////////////////
 
    ///////////////Particle Systems//////////////////////////////
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem dashParticles;
    ////////////////////////////////////////////////////////////
    
    /////////////Audio Scource//////////////////////////////////
    [Header("Audio Scource")]
    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private AudioSource actionAudioSource;
    ////////////////////////////////////////////////////////////
    
    /////////////Script Imports/////////////////////////////////
    [Header("Script Imports")]
    [SerializeField]private GeneralMovement generalMovement;
    [SerializeField]private JumpScript jump;
    [SerializeField]private CrouchScript crouch;
    [SerializeField]private DashScript dash;
    [SerializeField]private WallHangScript wallHang;
    ////////////////////////////////////////////////////////////
    			
    private PlayerStateManager stateManager;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
        if(crouch != null) {
            crouch.Initialize(standingCollider,headCheckRadius, headCheck, whatIsGround);
        }
        playerBody = GetComponent<Rigidbody2D>();
        stateManager = GetComponent<PlayerStateManager>();
    }
    public void Move(InputAction.CallbackContext context) {
        // maybe remove this if statement and just use the else statement
        if(stateManager.IsInState(PlayerState.WallHanging) || stateManager.IsInState(PlayerState.Dashing)) {
            return;
        }
        movementInput = context.ReadValue<Vector2>();  
        if (context.canceled) {
            movementInput = Vector2.zero;
        }
    }
    public void Jump(InputAction.CallbackContext context) {
       if (context.performed) {
            if (stateManager.IsInState(PlayerState.WallHanging)) {
                wallHang.JumpFromWall(playerBody, playerJumpForce, wallJumpForce);
                Debug.Log("wall Jump");
            }
            else if (!stateManager.IsInState(PlayerState.Crouching)) {
                jump.Jump(playerBody, playerJumpForce, playerdoubleJumpForce, maxJumpCount, actionAudioSource);
                Debug.Log("normal Jump");
            }
        }
    }

    public void Crouch(InputAction.CallbackContext context) {   
        if(context.performed) {
            crouch.StartCrouch();
            stateManager.ChangeState(PlayerState.Crouching);
        }
        else if(context.canceled) {
            crouch.StopCrouch();
            stateManager.ChangeState(PlayerState.Idle);
        }
    }
    public void Dash(InputAction.CallbackContext context) {
        if (context.performed && !stateManager.IsInState(PlayerState.Dashing)) {
            dash.Dash(generalMovement.GetIsFacingRight(), m_DashSpeed, m_DashDuration, m_DashCooldown, dashParticles, actionAudioSource);
            stateManager.ChangeState(PlayerState.Dashing);
        }
    }
     void FixedUpdate(){
        jump.GroundCheck(whatIsGround, groundCheck);
        jump.ManageCoytoeTime(coyoteTime);
        if(wallHang.CheckForWallHang()) {
            stateManager.ChangeState(PlayerState.WallHanging);
        }   
        switch (stateManager.currentState) {
            case PlayerState.Idle:
                HandleMovement();
                break;
            case PlayerState.Grounded:
                HandleMovement();
                break;
            case PlayerState.Running:
                HandleMovement();
                break;
            case PlayerState.Jumping:
                HandleMovement();
                playerBody.gravityScale = originalGravityScale;
                if (playerBody.linearVelocity.y < 0) {
                    stateManager.ChangeState(PlayerState.Falling);
                }
                break;
            case PlayerState.Falling:
                if (jump.IsGrounded()) {
                    stateManager.ChangeState(PlayerState.Grounded);
                }
                break;
            case PlayerState.Crouching:
                HandleCrouchMovement(); // remember to implement xD
                break;
            case PlayerState.WallHanging:
                wallHang.HandleHanging();
                break;
            case PlayerState.Dashing:
                // Handle dashing
                break;
        }
        /*
        if (playerBody.linearVelocity.y < 0) {
            stateManager.ChangeState(PlayerState.Falling);
        }
        */
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("IsJumping", stateManager.IsInState(PlayerState.Jumping));
        //animator.SetBool("IsCrouching", crouch.GetIsCrouching());
        animator.SetBool("IsFalling", stateManager.IsInState(PlayerState.Falling));
        //animator.SetBool("isDashing", dash.getIsDashing());
        Debug.Log(stateManager.currentState);
        //Debug.Log(jump.IsGrounded());
    }
    private void HandleMovement() {
        if (jump.IsGrounded() || playerAirControl) {
            float speed = crouch.GetIsCrouching() ? crouchSpeed : runSpeed;
            horizontalMove = movementInput.x * speed * Time.fixedDeltaTime;
            Vector3 targetVelocity = new Vector2(horizontalMove * 10f, playerBody.linearVelocity.y);
            playerBody.linearVelocity = Vector2.SmoothDamp(playerBody.linearVelocity, targetVelocity, ref m_Velocity, movementSmoothing);
            if (horizontalMove > 0 && !generalMovement.GetIsFacingRight()) {
                generalMovement.Flip(transform);
            } else if (horizontalMove < 0 && generalMovement.GetIsFacingRight()) {
                generalMovement.Flip(transform);
            }
            // Update state based on movement input
            if (horizontalMove != 0 && stateManager.currentState != PlayerState.Crouching ) {
                stateManager.ChangeState(PlayerState.Running);
            } else if (horizontalMove == 0 && stateManager.currentState != PlayerState.Crouching) {
                stateManager.ChangeState(PlayerState.Idle);
            }
        } else {
            stateManager.ChangeState(PlayerState.Falling);
        }
    }
    private void HandleCrouchMovement() {
        // Handle crouching movement if applicable
    }
    public void SetHorizontalMove(float move) {
        movementInput.x = move;
        horizontalMove = move;
    }
}
