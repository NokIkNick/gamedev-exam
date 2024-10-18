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
    [SerializeField] private int maxJumpCount = 1;

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
    public void Move(InputAction.CallbackContext context) {
        if(wallHang.GetIsWallHanging()) {
            return;
        }
        movementInput = context.ReadValue<Vector2>();  
        if (context.canceled) {
            movementInput = Vector2.zero;
        }
    }
    public void Jump(InputAction.CallbackContext context) {
       if (context.performed) {
            if (!crouch.GetIsCrouching() && !wallHang.GetIsWallHanging()) {
                jump.Jump(playerBody, playerJumpForce, playerdoubleJumpForce, maxJumpCount, actionAudioSource);
                //Debug.Log("Normal Jump");
            }
            else if (context.performed && wallHang.GetIsWallHanging()) {
               wallHang.JumpFromWall(playerBody, playerJumpForce, wallJumpForce);
                //Debug.Log("Wall Jump");
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
            dash.Dash(generalMovement.GetIsFacingRight(), m_DashSpeed, m_DashDuration, m_DashCooldown, dashParticles, actionAudioSource);
        }
    }
     void FixedUpdate(){
         if (!wallHang.GetIsWallHanging()) {
            if (jump.IsGrounded() || playerAirControl) {
                float speed = crouch.GetIsCrouching() ? crouchSpeed : runSpeed;
                horizontalMove =  movementInput.x * speed * Time.fixedDeltaTime;
                generalMovement.MoveHori(horizontalMove, movementSmoothing, playerBody, wallHang.GetIsWallHanging());
                /*
                // forsøg på at stoppe en bug hvor spiller begynder at løbe af sig selv efter wallHang (virker ikke, Arbejder videre på det)
                if (Mathf.Abs(playerBody.linearVelocity.y) <= 0.1f) {
                   playerBody.linearVelocity = new Vector2(0, playerBody.linearVelocity.y);
                } 
                */  
            }
        } else {
            horizontalMove = 0;
            playerBody.linearVelocity = new Vector2(0, playerBody.linearVelocity.y);
        }
        jump.GroundCheck(whatIsGround, groundCheck);
        jump.ManageCoytoeTime(coyoteTime);
        if (crouch.GetIsCrouching()) {
            if (!crouch.IsObstacleAbove() && !crouchButtonPressed) {
                crouch.StopCrouch();
            } else if (crouch.IsObstacleAbove()) {
                crouch.StartCrouch();
            }
        }
        if (jump.IsGrounded() && Mathf.Abs(horizontalMove) > 0 && !walkingAudioSource.isPlaying) {
            walkingAudioSource.Play();
        }else if (!jump.IsGrounded() || Mathf.Abs(horizontalMove) == 0 && walkingAudioSource.isPlaying) {
            walkingAudioSource.Stop();
        }
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("IsJumping", !jump.IsGrounded());
        //animator.SetBool("IsCrouching", crouch.GetIsCrouching());
        animator.SetBool("IsFalling", generalMovement.IsFalling(playerBody));
        //animator.SetBool("isDashing", dash.getIsDashing());
    }
    public void SetHorizontalMove(float move) {
        horizontalMove = move;
    }
}
