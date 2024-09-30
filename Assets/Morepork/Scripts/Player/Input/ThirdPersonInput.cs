using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace Morepork.FinalCharacterController
{
    [DefaultExecutionOrder(-2)] //Runs before everything other script.
    public class ThirdPersonInput : MonoBehaviour, PlayerControls.IThirdPersonMapActions
    {
        #region Class Variables
        public Vector2 ScrollInput { get; private set; }
        public bool AimToggledOn { get; private set; }
        public bool ShootPressed { get; private set; }
        [Header("Rigging")]
        [SerializeField] private Rig rig1; // Reference to the Rig component
        [SerializeField] private float lerpSpeed = 5f; // Speed of the lerp
        private float targetRigWeight;
        [Header("Aiming and Shooting")]
        [SerializeField] private bool holdToAim = true;
        [SerializeField] private Transform ofBulletProjectile;
        [SerializeField] private Transform spawnBulletPostion;
        private bool isShooting;
        [Header("Camera Wheel Adjustments")]
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;
        [SerializeField] private float _cameraZoomSpeed = 0.2f;
        [SerializeField] private float _cameraMinZoom = 1f;
        [SerializeField] private float _cameraMaxZoom = 5f;
        [Header("References")]
        private Cinemachine3rdPersonFollow _thirdPersonFollow;
        [SerializeField] private PlayerAnimation _playerAnimation;
        [SerializeField] private WeaponManager _weaponManager; // Reference to the WeaponManager

        #endregion

        #region Start Up
        private void Awake()
        {
            _thirdPersonFollow = _virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            _playerAnimation = _playerAnimation.GetComponent<PlayerAnimation>();

            // Ensure WeaponManager is assigned properly
            if (_weaponManager == null)
            {
                _weaponManager = GetComponent<WeaponManager>();
                if (_weaponManager == null)
                {
                    Debug.LogError("WeaponManager component not found on the same GameObject.");
                }
            }

            if (rig1 == null)
            {
                Debug.LogError("Rig1 is not assigned in the inspector.");
            }
        }

        private void OnEnable() //Enable the player controls
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable.");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.Enable();
            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.SetCallbacks(this);
        }

        private void OnDisable() //Disable the player controls
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable.");
                return;
            }
            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.Disable();
            PlayerInputManager.Instance.PlayerControls.ThirdPersonMap.RemoveCallbacks(this);
        }

        #region Update

        private void Update()
        {
            // Camera zoom and rig weight logic
            _thirdPersonFollow.CameraDistance = Mathf.Clamp(_thirdPersonFollow.CameraDistance + ScrollInput.y, _cameraMinZoom, _cameraMaxZoom);
            if (rig1 != null)
            {
                rig1.weight = Mathf.Lerp(rig1.weight, targetRigWeight, Time.deltaTime * lerpSpeed);
            }

            // Continuous shooting logic - only if aiming is true
            if (AimToggledOn && isShooting && _weaponManager != null)
            {
                if (_weaponManager.nextFireTime <= 0 && _weaponManager.ammo > 0)
                {
                    _weaponManager.Fire();
                }
            }
        }

        private void LateUpdate()
        {
            ScrollInput = Vector2.zero;
        }
        #endregion

        #endregion

        #region Input Callbacks
        public void OnScrollCamera(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            Vector2 scrollInput = context.ReadValue<Vector2>();
            ScrollInput = -1f * scrollInput.normalized * _cameraZoomSpeed;
            print(scrollInput);
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AimToggledOn = holdToAim || !AimToggledOn;
                Debug.Log("Aim = " + AimToggledOn);
                _aimVirtualCamera.gameObject.SetActive(true);

                // Set the target rig weight to 1 when aiming
                targetRigWeight = 1f;
            }
            else if (context.canceled)
            {
                AimToggledOn = !holdToAim && AimToggledOn;
                Debug.Log("Aim = " + AimToggledOn);
                _aimVirtualCamera.gameObject.SetActive(false);

                // Set the target rig weight to 0 when not aiming
                targetRigWeight = 0f;
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {

            if (AimToggledOn)
            {
                if (context.performed)
                {
                    isShooting = true;
                }
                else if (context.canceled)
                {
                    isShooting = false;
                }
                if (_weaponManager.nextFireTime > 0 || _weaponManager.ammo <= 0)
                    return;
                _weaponManager.Fire();

            }

        }

        public void OnReload(InputAction.CallbackContext context) //Reload Wepaon
        {
            _weaponManager?.Reload();
        }

        public void OnQuitGame(InputAction.CallbackContext context) //Close App
        {
            if (context.performed)
            {
                Application.Quit();
            }
        }
        #endregion
    }
}




