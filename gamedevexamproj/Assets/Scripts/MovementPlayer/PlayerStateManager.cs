using UnityEngine;
public class PlayerStateManager : MonoBehaviour {
    public PlayerState currentState { get; private set; }

    public void ChangeState(PlayerState newState) {
        if (currentState != newState) {
            currentState = newState;
            //OnStateEnter(newState);
        }
    }
/*
    private void OnStateEnter(PlayerState state) {
        switch (state) {
            case PlayerState.Jumping:
                // logic for jumping state
                break;
            case PlayerState.Climbing:
                // logic for climbing state
                break;
        }
    }
*/
    public bool IsInState(PlayerState state) {
        return currentState == state;
    }
}
