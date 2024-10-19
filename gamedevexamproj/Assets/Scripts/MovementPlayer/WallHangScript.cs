using System.Collections;
using UnityEngine;

public class WallHangScript : MonoBehaviour
{
    [Header("Wall Hang Settings")]
    [SerializeField] private float hangDuration = 2f;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private float wallJumpCooldown = 0.5f;
    //[SerializeField] private float wallHangCooldown = 1f; 
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private AudioClip jumpSound;

    private bool canWallHang = true;                  
    private float hangTimer = 0f;
    private float wallJumpTimer = 0f;
    //private float wallHangTimer = 0f;                   
    private Rigidbody2D playerBody;
    private bool isTouchingWallLeft = false;
    private bool isTouchingWallRight = false;
    private GeneralMovement generalMovement; 
    private MovementController movementController;
    private PlayerStateManager playerStateManager; 
    private float originalGravityScale = 3f;
    private Vector2 wallJumpDirection;  

    private void Awake() {
        playerBody = GetComponent<Rigidbody2D>();
        if (playerBody == null) {
            Debug.LogError("Rigidbody2D component not found! Make sure the GameObject has a Rigidbody2D component attached.");
        }
        generalMovement = GetComponent<GeneralMovement>();
        movementController = GetComponent<MovementController>();
        playerStateManager = GetComponent<PlayerStateManager>();
    }

    private void Update() {
        if (!canWallHang) {
            wallJumpTimer += Time.deltaTime;
            if (wallJumpTimer >= wallJumpCooldown) {
                canWallHang = true;
                wallJumpTimer = 0f;
            }
        }
        /*
        if (!canWallHang && wallHangTimer > 0) {
            wallHangTimer -= Time.deltaTime;
            if (wallHangTimer <= 0) {
                canWallHang = true;  
            }
        }
        */
    }
    

    public bool CheckForWallHang() {
        /*
        if(!canWallHang) {
            return false;
        }
        */
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, whatIsWall);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, whatIsWall);
        isTouchingWallLeft = hitLeft.collider != null;
        isTouchingWallRight = hitRight.collider != null;
        if ((isTouchingWallLeft || isTouchingWallRight) && canWallHang && playerBody.linearVelocity.y < 0) {
            StartHanging();
            return true;
        }
        return false;
    }

    private void StartHanging() {
        movementController.SetHorizontalMove(0f);
        hangTimer = hangDuration;
        playerBody.linearVelocity = Vector2.zero;
        originalGravityScale = playerBody.gravityScale;
        playerBody.gravityScale = 0;
    }

    public void HandleHanging() {
        hangTimer -= Time.deltaTime;
        if (Input.GetAxisRaw("Horizontal") > 0 && !generalMovement.GetIsFacingRight()) {
            generalMovement.Flip(transform);
        } else if (Input.GetAxisRaw("Horizontal") < 0 && generalMovement.GetIsFacingRight()) {
            generalMovement.Flip(transform);
        }
        if (hangTimer <= 0) {
            StopHanging();
        }
    } 
     public void JumpFromWall(Rigidbody2D rb, float jumpForce, float horizontalForce,AudioSource audioSource) {
        float jumpDirection = generalMovement.GetIsFacingRight() ? 1 : -1;
        audioSource.PlayOneShot(jumpSound);
        rb.AddForce(new Vector2(jumpDirection * horizontalForce, jumpForce), ForceMode2D.Impulse);
        playerStateManager.ChangeState(PlayerState.Jumping); 
        //playerBody.gravityScale = originalGravityScale;
        canWallHang = false;
        wallJumpTimer = 0f;
        movementController.SetHorizontalMove(0f); 
        StartCoroutine(ResetHorizontalVelocityAfterWallJump());
    }
     private IEnumerator ResetHorizontalVelocityAfterWallJump() {
        yield return new WaitForSeconds(0.1f);
        playerBody.linearVelocity = new Vector2(0, playerBody.linearVelocity.y);
    }
     public void StopHanging() { 
        playerBody.gravityScale = originalGravityScale;
        playerStateManager.ChangeState(PlayerState.Falling);
        //wallHangTimer = wallHangCooldown;
        //canWallHang = false;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.left * wallCheckDistance);
        Gizmos.DrawRay(transform.position, Vector2.right * wallCheckDistance);
    }
}

