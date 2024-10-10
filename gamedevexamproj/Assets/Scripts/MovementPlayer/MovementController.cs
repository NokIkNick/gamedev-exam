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
    ////////////////////////////////////////////////////////////
    
    ///////////////Jump Variables///////////////////////////////
    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.1f;
  	[SerializeField] private float playerJumpForce = 25f;
  	[SerializeField] private float playerdoubleJumpForce = 25f;
	[SerializeField] private bool playerAirControl = true;
    [SerializeField] private int maxJumpCount = 2;

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
    [SerializeField] private float m_DashDuration = 0.2f;
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
 
    ///////////////Particle Systems////////////////////////////////////////
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem dashParticles;
    ////////////////////////////////////////////////////////////
    
    /////////////Script Imports/////////////////////////////////
    [Header("Script Imports")]
    [SerializeField]private GeneralMovement generalMovement;
    [SerializeField]private JumpScript jump;
    [SerializeField]private CrouchScript crouch;
    [SerializeField]private DashScript dash;
    [SerializeField]private WallHangScript wallHang;
    ////////////////////////////////////////////////////////////
    					

    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
        if(crouch != null) {
            crouch.Initialize(standingCollider,headCheckRadius, headCheck, whatIsGround);
        }
    }
     void Awake() {
            playerBody = GetComponent<Rigidbody2D>(); 
            if (playerBody == null) {       
                Debug.LogError("Rigidbody2D component not found! Make sure the GameObject has a Rigidbody2D component attached.");
            }
    } 
    void Update(){

    }
    public void Move(InputAction.CallbackContext context) {
        if(wallHang.GetIsWallHanging()) {
            return;
        }
        //float speed = crouch.GetIsCrouching() ? crouchSpeed : runSpeed;
        //movementInput = context.ReadValue<Vector2>();
        //horizontalMove = movementInput.x * speed * Time.fixedDeltaTime;
        if (context.started) {  // When the movement button is pressed or held down
            movementInput = context.ReadValue<Vector2>();  // Store the movement input
        }

        if (context.canceled) {  // When the movement button is released
            movementInput = Vector2.zero;  // Reset the movement input when released
        }
    }
    public void Jump(InputAction.CallbackContext context) {
       if (context.performed) {
            if (!crouch.GetIsCrouching() && !wallHang.GetIsWallHanging()) {
                jump.Jump(playerBody, playerJumpForce, playerdoubleJumpForce, maxJumpCount);
            }
            else if (context.performed && wallHang.GetIsWallHanging()) {
                // Perform wall-jump with added horizontal force
                wallHang.JumpFromWall(playerBody, playerJumpForce, wallJumpForce);
            }
        }
    }

    public void Crouch(InputAction.CallbackContext context) {   
        if(context.performed) {
            crouchButtonPressed = true;
            crouch.StartCrouch();
        }
        else if(context.canceled) {
            crouchButtonPressed = false;
            crouch.StopCrouch();
        }

    }
    public void Dash(InputAction.CallbackContext context) {
        if (context.performed) {
            dash.Dash(generalMovement.GetIsFacingRight(), m_DashSpeed, m_DashDuration, m_DashCooldown, dashParticles);
        }
    }
     void FixedUpdate(){
         if (!wallHang.GetIsWallHanging()) {
            if (jump.IsGrounded() || playerAirControl) {
                float speed = crouch.GetIsCrouching() ? crouchSpeed : runSpeed;
                horizontalMove =  movementInput.x * speed * Time.fixedDeltaTime;
                if(wallHang.GetIsWallHanging()) {
                    horizontalMove = 0;
                }
                generalMovement.MoveHori(horizontalMove, movementSmoothing, playerBody, wallHang.GetIsWallHanging());
            }
        } else {
            horizontalMove = 0;
            playerBody.linearVelocity = new Vector2(0, playerBody.linearVelocity.y);
        }
        jump.ManageCoytoeTime(coyoteTime);
        if (crouch.GetIsCrouching()) {
            if (!crouch.IsObstacleAbove() && !crouchButtonPressed) {
                crouch.StopCrouch();
            } else if (crouch.IsObstacleAbove()) {
                crouch.StartCrouch();
            }
        }
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("IsJumping", !jump.IsGrounded());
        //animator.SetBool("IsCrouching", crouch.GetIsCrouching());
        animator.SetBool("IsFalling", generalMovement.IsFalling(playerBody));
        jump.GroundCheck(whatIsGround, groundCheck);
    }
    public void SetHorizontalMove(float move) {
        horizontalMove = move;
    }
}
