using Photon.Pun;
using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviourPun
{
    [Header("Ammo Pickup")]
    public int magAmount = 1; // Number of mags to add on pickup

    [Header("VFX")]
    public ParticleSystem vfx;
    public Light vfxlight;

    private Collider pickupCollider;
    [SerializeField]private GameObject Visuals;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
        //pickupRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);

        WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();

        if (weaponManager != null)
        {
            Debug.Log("WeaponManager found and is local player.");
            weaponManager.AddMagazine(magAmount);
            StartCoroutine(RespawnPickup());
        }
    }

    private IEnumerator RespawnPickup()
    {
        photonView.RPC("HidePickup", RpcTarget.All);
        yield return new WaitForSeconds(10f); // Adjust as necessary
        photonView.RPC("ShowPickup", RpcTarget.All);
    }

    [PunRPC]
    private void HidePickup()
    {
        pickupCollider.enabled = false;
        Visuals.SetActive(false);
        if (vfx != null) vfx.Stop();
        if (vfxlight != null) vfxlight.enabled = false;
    }

    [PunRPC]
    private void ShowPickup()
    {
        pickupCollider.enabled = true;
        Visuals.SetActive(true);
        if (vfx != null) vfx.Play();
        if (vfxlight != null) vfxlight.enabled = true;
    }
}

