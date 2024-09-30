using Morepork.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeapon : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLength;
    [SerializeField] private Vector3 rayOffset;
    [SerializeField] private LayerMask weaponMask;
    private RaycastHit topRayHitInfo;

    private Weapon currentWeapon;

    private ThirdPersonInput _thirdPersonInput;

    //[SerializeField] private Transform equipPos;
    //[SerializeField] private Transform aimingPos;
    [SerializeField] private Transform currentWeaponPos; //NonIK Equip

    private void Awake()
    {
        _thirdPersonInput = GetComponent<ThirdPersonInput>();
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Equip();
        }

    }
    void FixedUpdate() //Fixed update is better for physics
    {
        RaycastsHandler();
    }
    private void RaycastsHandler()
    {
        Ray topRay = new Ray(transform.position + rayOffset, transform.forward);

        Debug.DrawRay(transform.position + rayOffset, transform.forward * rayLength, Color.red);

        Physics.Raycast(topRay, out topRayHitInfo, rayLength, weaponMask);

    }
        private void Equip()
    {
        if(topRayHitInfo.collider != null)
        {
            currentWeapon = topRayHitInfo.transform.gameObject.GetComponent<Weapon>();
            
            BoxCollider boxCollider = currentWeapon.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }

        }

        if (!currentWeapon) return;

        currentWeapon.transform.parent = currentWeaponPos.transform;
        currentWeapon.transform.position = currentWeaponPos.position;
        currentWeapon.transform.rotation = currentWeaponPos.rotation;

        currentWeapon.IsRotating = false;
    }


}
