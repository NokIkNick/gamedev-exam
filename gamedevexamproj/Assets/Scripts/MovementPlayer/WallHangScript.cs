using System.Collections;
using UnityEngine;

public class WallHangScript : MonoBehaviour
{
    [Header("Wall Hang Settings")]
    [SerializeField] private float hangDuration = 2f;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private float wallJumpCooldown = 0.5f;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private float wallHangDisableTime = 0.1f;
    private bool isHanging = false; 
    private bool canWallHang = true;                  
    private float hangTimer = 0f;
    private float wallJumpTimer = 0f;                   
    private Rigidbody2D playerBody;
    private bool isTouchingWallLeft = false;
    private bool isTouchingWallRight = false;
    private GeneralMovement generalMovement; 
    private MovementController movementController;
    private float originalGravityScale;  

    private void Awake() {
        playerBody = GetComponent<Rigidbody2D>();
        if (playerBody == null) {
            Debug.LogError("Rigidbody2D component not found! Make sure the GameObject has a Rigidbody2D component attached.");
        }
        generalMovement = GetComponent<GeneralMovement>();
        movementController = GetComponent<MovementController>();
    }

    private void Update() {
        if (!canWallHang) {
            wallJumpTimer += Time.deltaTime;
            if (wallJumpTimer >= wallJumpCooldown) {
                canWallHang = true;
                wallJumpTimer = 0f;
            }
        }
        if (isHanging) {
            HandleHanging();
        }
        else if (canWallHang) {
            CheckForWallHang();
        }
    }

    private void CheckForWallHang() {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, whatIsWall);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, whatIsWall);
        isTouchingWallLeft = hitLeft.collider != null;
        isTouchingWallRight = hitRight.collider != null;
        if ((isTouchingWallLeft || isTouchingWallRight) && !isHanging && playerBody.linearVelocity.y < 0) {
            StartHanging();
        }
    }

    private void StartHanging() {
        Debug.Log("StartHanging");
        movementController.SetHorizontalMove(0f);
        isHanging = true;
        hangTimer = hangDuration;
        playerBody.linearVelocity = Vector2.zero;
        originalGravityScale = playerBody.gravityScale;
        playerBody.gravityScale = 0;
    }

    private void HandleHanging() {
        hangTimer -= Time.deltaTime;
        if (Input.GetAxisRaw("Horizontal") > 0 && !generalMovement.GetIsFacingRight()) {
            generalMovement.Flip();
        } else if (Input.GetAxisRaw("Horizontal") < 0 && generalMovement.GetIsFacingRight()) {
            generalMovement.Flip();
        }
        hangTimer -= Time.deltaTime;
        if (hangTimer <= 0) {
            StopHanging();
        }
    } 
     public void JumpFromWall(Rigidbody2D rb, float jumpForce, float horizontalForce) {
        StopHanging();  
        float jumpDirection = generalMovement.GetIsFacingRight() ? 1 : -1;
        rb.AddForce(new Vector2(jumpDirection * horizontalForce, jumpForce), ForceMode2D.Impulse);
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
        isHanging = false;  
        playerBody.gravityScale = originalGravityScale;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.left * wallCheckDistance);
        Gizmos.DrawRay(transform.position, Vector2.right * wallCheckDistance);
    }
    public bool GetIsWallHanging() {
        return isHanging;
    }
}

