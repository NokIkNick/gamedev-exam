using UnityEngine;
using UnityEngine.InputSystem;

public class LadderClimb : MonoBehaviour
{
    [Header("Ladder Climbing Settings")]
    [SerializeField] private float climbSpeed = 8f;          
    [SerializeField] private LayerMask ladderLayer;          
    private bool canClimb = false;                        
    private Rigidbody2D playerBody;                          
    private PlayerInput playerInput;                         
    private Vector2 inputVector;
    private Collider2D ladderCollider;                           
    private float originalGravityScale; 

    private void Awake() {
        playerBody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        originalGravityScale = playerBody.gravityScale;
    }

    private void OnTriggerEnter2D(Collider2D triggerCollider) {
        if (triggerCollider.gameObject.layer == LayerMask.NameToLayer("Ladder")) {
            canClimb = true; 
            ladderCollider = triggerCollider; 
            playerBody.gravityScale = 0; 
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, 0); 
        }
    }
   private void OnTriggerStay2D(Collider2D triggerCollider) {
        if (triggerCollider.gameObject.layer == LayerMask.NameToLayer("Ladder")) {
            canClimb  = true;
        }
    }

    private void OnTriggerExit2D(Collider2D triggerCollider) {
        if (triggerCollider.gameObject.layer == LayerMask.NameToLayer("Ladder")) {
            StopClimbing();
        }
    }

    private void Update() {
        if (canClimb) {
            HandleClimbing();
        }
    }

    private void HandleClimbing() {
        inputVector = playerInput.actions["Move"].ReadValue<Vector2>();
        if (inputVector.y > 0) {
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, climbSpeed);
        } else if (inputVector.y < 0) {
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, -climbSpeed);
        } else {
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, 0);
        }
    }
     private void StopClimbing() {
        canClimb  = false;
        playerBody.gravityScale = originalGravityScale;
    }
}

