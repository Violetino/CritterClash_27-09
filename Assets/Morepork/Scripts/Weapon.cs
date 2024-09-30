using Morepork.FinalCharacterController;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    public int damage;
    public float fireRate;
    private Rigidbody weaponBody;
    private float nextFire;
    public Cinemachine3rdPersonFollow camera;
    [SerializeField] PlayerController playerController;

    [SerializeField] private float rotationSpeed;

    public bool IsRotating {  get;  set; }
    void Start()
    {
        weaponBody = GetComponent<Rigidbody>();

        if (weaponBody)
        {
            weaponBody.isKinematic = true;
        }

        IsRotating = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && nextFire <= 0)
        {
            nextFire = 1 / fireRate;
            Fire();
            Debug.Log("Pressed Z, Fire");
        }
        if (nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }

        if (!IsRotating) return;
        transform.Rotate(Vector3.up * rotationSpeed * (1 - Mathf.Exp(-rotationSpeed * Time.deltaTime)));
        
    }

    void Fire()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            if (hit.transform.gameObject.GetComponent<PlayerController>())
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }
}
