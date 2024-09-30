using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Morepork.FinalCharacterController
{
    [DefaultExecutionOrder(-2)] // This attribute ensures that this script runs before others, useful for setting up input data.
    public class PlayerInputActions : MonoBehaviour, PlayerControls.IPlayerActionMapActions
    {
        #region Class Variables
        // Private references to other components that handle player locomotion and state.
        private PlayerLocomotionInput _playerLocomotionInput; // Handles input related to player movement.
        private PlayerState _playerState; // Manages the player's current state, such as walking, jumping, etc.

        // Public properties to track whether certain actions (attack, gather) have been initiated.
        public bool AttackPressed { get; private set; } // True if the attack button has been pressed.
        public bool GatherPressed { get; private set; } // True if the gather button has been pressed.

        #endregion

        #region Start Up
        // Initialization and setup methods for the component.

        // Called when the script instance is being loaded.
        private void Awake()
        {
            // Retrieves and stores the component for player locomotion input.
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();

            // Retrieves and stores the component for player state management.
            _playerState = GetComponent<PlayerState>();
        }

        // Called when the object becomes enabled and active.
        private void OnEnable()
        {
            // Checks if the PlayerInputManager's PlayerControls instance is properly initialized.
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                // Logs an error message if PlayerControls is not initialized.
                Debug.LogError("Player controls is not initialized - cannot enable, boy");
                return;
            }

            // Enables the player action map to start receiving input events.
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Enable();

            // Sets this class as the callback handler for player actions, ensuring it will respond to input events.
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.SetCallbacks(this);
        }

        // Called when the object becomes disabled or inactive.
        private void OnDisable()
        {
            // Checks if the PlayerInputManager's PlayerControls instance is properly initialized.
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                // Logs an error message if PlayerControls is not initialized.
                Debug.LogError("Player controls is not initialized - cannot be disabled, boy");
                return;
            }

            // Disables the player action map to stop receiving input events.
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.Disable();

            // Removes this class as the callback handler for player actions.
            PlayerInputManager.Instance.PlayerControls.PlayerActionMap.RemoveCallbacks(this);
        }

        #endregion

        #region Update
        // Methods related to the frame-by-frame update cycle.

        // Called every frame to update the state of the component.
        private void Update()
        {
            // If the player is moving (MovementInput is not zero), jumping, or falling, gathering input is reset.
            // This prevents the gather action from being continuously processed while the player is moving or airborne.
            if (_playerLocomotionInput.MovementInput != Vector2.zero ||
                _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping ||
                _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling)
            {
                GatherPressed = false;
            }
        }

        // Sets the GatherPressed property to false, typically called when the gathering action should be reset.
        public void SetGatherPressedFalse()
        {
            GatherPressed = false;
        }

        // Sets the AttackPressed property to false, typically called when the attack action should be reset.
        public void SetAttackPressedFalse()
        {
            AttackPressed = false;
        }
        #endregion

        #region Input Callbacks
        // Input callback methods that are triggered by specific player actions.

        // Callback for when the attack input action is performed.
        public void OnAttack(InputAction.CallbackContext context)
        {
            // Only set AttackPressed to true if the action is performed, not started or canceled.
            if (!context.performed)
                return;

            AttackPressed = true;
        }

        // Callback for when the gather input action is performed.
        public void OnGather(InputAction.CallbackContext context)
        {
            // Only set GatherPressed to true if the action is performed, not started or canceled.
            if (!context.performed)
                return;

            GatherPressed = true;
        }
        #endregion
    }
}


