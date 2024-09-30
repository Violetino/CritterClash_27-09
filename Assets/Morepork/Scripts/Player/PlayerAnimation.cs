using Morepork.FinalCharacterController;
using System.Linq;
using UnityEngine;

namespace Morepork.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private PlayerController _playerController;
        private PlayerInputActions _playerActionsInput;
        private ThirdPersonInput _thirdPersonInput;
        private HealthManager _healthManager;  // Reference to HealthManager for health-related checks

        // Locomotion
        private static int inputXHash = Animator.StringToHash("InputX");
        private static int inputYHash = Animator.StringToHash("InputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static int isIdlingHash = Animator.StringToHash("isIdling");
        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");

        // Actions
        private static int isAttackingHash = Animator.StringToHash("isAttacking");
        private static int isGatheringHash = Animator.StringToHash("isGathering");
        private static int isAimingHash = Animator.StringToHash("isAiming");
        private static int isPlayingActionHash = Animator.StringToHash("isPlayingAction");
        private static int isOKHash = Animator.StringToHash("isOK");  // Hash for death animation trigger

        private int[] actionHashes;

        // Camera/Rotation
        private static int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
        private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");

        private Vector3 _currentBlendInput = Vector3.zero;
        private float _sprintMaxBlendValue = 1.5f;
        private float _runMaxBlendValue = 1.0f;
        private float _walkMaxBlendValue = 0.5f;

        private bool deathAnimationPlayed = false;  // Track if death animation has been triggered

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _playerController = GetComponent<PlayerController>();
            _playerActionsInput = GetComponent<PlayerInputActions>();
            _thirdPersonInput = GetComponent<ThirdPersonInput>();
            _healthManager = GetComponent<HealthManager>();  // Get reference to HealthManager

            actionHashes = new int[] { isGatheringHash };
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            // Check if the player is dead via HealthManager
            if (_healthManager != null && _healthManager.health <= 0)
            {
                // Trigger death animation only once
                if (!deathAnimationPlayed)
                {
                    _animator.SetBool(isOKHash, true);
                    deathAnimationPlayed = true;  // Ensure the animation is only triggered once
                    Debug.Log("Death animation triggered.");
                }

                return;  // Skip further animation updates when the player is dead
            }

            // Update locomotion and action animations if player is not dead
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            bool isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            bool isGrounded = _playerState.InGroundedState();
            bool isPlayingAction = actionHashes.Any(hash => _animator.GetBool(hash));

            bool isRunBlendValue = isRunning || isJumping || isFalling;

            Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * _sprintMaxBlendValue :
                                  isRunBlendValue ? _playerLocomotionInput.MovementInput * _runMaxBlendValue :
                                                    _playerLocomotionInput.MovementInput * _walkMaxBlendValue;

            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            _animator.SetBool(isGroundedHash, isGrounded);
            _animator.SetBool(isIdlingHash, isIdling);
            _animator.SetBool(isFallingHash, isFalling);
            _animator.SetBool(isJumpingHash, isJumping);
            _animator.SetBool(isRotatingToTargetHash, _playerController.IsRotatingToTarget);
            _animator.SetBool(isAttackingHash, _playerActionsInput.AttackPressed);
            _animator.SetBool(isAimingHash, _thirdPersonInput.AimToggledOn);
            _animator.SetBool(isGatheringHash, _playerActionsInput.GatherPressed);
            _animator.SetBool(isPlayingActionHash, isPlayingAction);

            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
        }

        // Call this when the player respawns to reset the death animation state
        public void ResetDeathAnimation()
        {
            _animator.SetBool(isOKHash, false);  // Reset the isDead trigger in Animator
            deathAnimationPlayed = false;  // Reset the flag to allow the death animation to play again
        }
    }
}



