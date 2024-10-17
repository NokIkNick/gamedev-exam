using UnityEngine;
public class PlayerStateManager : MonoBehaviour {
    public PlayerState currentState { get; private set; }

    public void ChangeState(PlayerState newState) {
        if (currentState != newState) {
            Debug.Log("State changed from " + currentState + " to " + newState);
            currentState = newState;
            OnStateEnter(newState);
        }
    }

    // Optional: You can handle specific logic when entering a state
    private void OnStateEnter(PlayerState state) {
        switch (state) {
            case PlayerState.Jumping:
                // Handle Jump Start
                break;
            case PlayerState.Climbing:
                // Handle Climb Start
                break;
            // Add other cases as needed
        }
    }
    public bool IsInState(PlayerState state) {
        return currentState == state;
    }
}
