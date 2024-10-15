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
  
    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Im moving" + context.ReadValue<Vector2>());
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
        Debug.Log("Jump");
        if(context.performed)
        {
            jump = true;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {   
        Debug.Log("Im Crouching");
        if(context.performed)
        {
            crouch = true;
        }
        else if(context.canceled)
        {
            crouch = false;
        }
    }
    // get input from our player
   // void Update(){
    //    horizontalMove = Input.GetAxisRaw("Horizontal")*runSpeed;
     //   if(Input.GetButtonDown("Jump")){
     //       jump = true;
     //   }
      //  if(Input.GetButtonDown("Crouch")){
     //       crouch = true;
      //  }else if(Input.GetButtonUp("Crouch")){
     //       crouch = false;
     //   }

// }
    void FixedUpdate(){
        // move our character
        //controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        //jump = false;
        float horizontalMove = (movementInput.x * runSpeed * Time.fixedDeltaTime);
       // Debug.Log("Horizontal Move: " + horizontalMove);
        controller.Move(horizontalMove, crouch, jump);
        // Reset jump after it's used
        jump = false;

    }
}
