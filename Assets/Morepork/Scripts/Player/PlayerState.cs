using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Morepork.FinalCharacterController
{
    // This class manages the player's current movement state and provides utilities to check certain conditions.
    public class PlayerState : MonoBehaviour
    {
        // The current movement state of the player.
        // It is serialized to be visible in the Unity Editor, but can only be set within this class.
        [field: SerializeField]
        public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;

        // This method sets the player's current movement state to a new state.
        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
        }

        // This method checks if the player is in a grounded state.
        // Returns true if the player is idling, walking, running, or sprinting.
        public bool InGroundedState()
        {
            return IsStateGroundedState(CurrentPlayerMovementState);
        }

        // This method checks if a specific movement state is considered a grounded state.
        // Used internally and by InGroundedState() to determine if the player is on the ground.
        public bool IsStateGroundedState(PlayerMovementState movementState)
        {
            return movementState == PlayerMovementState.Idling ||
                   movementState == PlayerMovementState.Walking ||
                   movementState == PlayerMovementState.Running ||
                   movementState == PlayerMovementState.Sprinting;
        }
    }

    // This enum defines the various movement states a player can be in.
    // Each state is assigned a unique integer value, which can be used for comparisons.
    public enum PlayerMovementState
    {
        Idling = 0,    // Player is stationary
        Walking = 1,   // Player is walking
        Running = 2,   // Player is running
        Sprinting = 3, // Player is sprinting
        Jumping = 4,   // Player is jumping
        Falling = 5,   // Player is falling
        Strafing = 6,  // Player is strafing (moving sideways)
    }
}
