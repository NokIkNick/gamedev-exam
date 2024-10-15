using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private float horizontalMove = 0f;
    [SerializeField] private float runSpeed = 60f;
    private bool jump = false;
    private bool crouch = false;
    private Vector2 movementInput;

    private Animator animator;

    void Start(){
        animator = GetComponent<Animator>();
    }

  
    public void Move(InputAction.CallbackContext context) {
        if(context.ReadValue<Vector2>().x > 0)
        {
            horizontalMove = runSpeed;
        }
        else if(context.ReadValue<Vector2>().x < 0)
        {
            horizontalMove = -runSpeed;
        }
        else
        {
            horizontalMove = 0;
        }
        movementInput = context.ReadValue<Vector2>();
    }
    public void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("Jump");
        if(context.performed)
        {
            jump = true;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {   
        //Debug.Log("Im Crouching");
        if(context.performed)
        {
            crouch = true;
        }
        else if(context.canceled)
        {
            crouch = false;
        }
    }
    void FixedUpdate(){
        // move our character
        float horizontalMove = (movementInput.x * runSpeed * Time.fixedDeltaTime);
        controller.Move(horizontalMove, crouch, jump);


        // Update Animator
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("IsJumping", !controller.IsGrounded());
        animator.SetBool("IsFalling", controller.IsFalling());
        // Reset jump after it's used
        jump = false;

    }
}
