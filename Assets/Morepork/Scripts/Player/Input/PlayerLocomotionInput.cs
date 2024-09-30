using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Morepork.FinalCharacterController
{
    [DefaultExecutionOrder(-2)] //Runs before everything other script.
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMAPActions
    {
        #region Class Variables
        [SerializeField] private bool holdToSprint = true;
        public Vector2 MovementInput { get; private set; } //set the Movement Input
        public Vector2 LookInput { get; private set; } 
        public bool JumpPressed { get; private set; }
        public bool SprintToggledOn { get; private set; }
        public bool WalkToggledOn { get; private set; }
        #endregion

        #region Start Up
        private void OnEnable() //Enable the player controls
        {
            if(PlayerInputManager.Instance?.PlayerControls == null)          
            {
                Debug.LogError("Player controls is not initizlized - cannot enable, boy");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMAP.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMAP.SetCallbacks(this);
        }

        private void OnDisable() //Disable the player controls
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initizlized - cannot disabled, boy");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMAP.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMAP.RemoveCallbacks(this);
        }
        #endregion

        #region Late Update
        private void LateUpdate()
        {
            JumpPressed = false; // Disable spacebar.
        }
        #endregion

        #region Input Callbacks
        public void OnMovement(InputAction.CallbackContext context)
        {
            //Set the movement input to the vector2 output
            MovementInput = context.ReadValue<Vector2>();
            //Testing input
            Debug.Log(MovementInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            if(context.performed) // Hold shift
            {
                SprintToggledOn = holdToSprint || !SprintToggledOn; // Should it not be "= true"?
                Debug.Log("Sprint = " + SprintToggledOn);
            }
            else if (context.canceled) // Lift of shift
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn; // Should it not be "= false"?
                Debug.Log("Sprint = " + SprintToggledOn);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return; //Nothing happens under this line untill space is pressed.
            
            JumpPressed = true;
            Debug.Log("Jump = " + JumpPressed);

            
        }

        public void OnToggleWalk(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            WalkToggledOn = !WalkToggledOn;
        }
        #endregion

    }

}
