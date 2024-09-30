using Photon.Pun;
using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviourPun
{
    [Header("Health Pickup")]
    public int healAmount = 20; // Amount of health the pickup restores
    public float respawnTime = 10f; // Time before the pickup respawns
    [Header("VFX")]
    public ParticleSystem vfx;
    public Light vfxlight;

    private Collider pickupCollider;
    private Renderer pickupRenderer;
    

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
        pickupRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthManager healthManager = other.GetComponent<HealthManager>();

        if (healthManager != null && healthManager.isLocalPlayer)
        {
            // Heal the player
            healthManager.Heal(healAmount);

            // Disable the pickup and start the respawn timer
            StartCoroutine(RespawnPickup());
        }
    }

    private IEnumerator RespawnPickup()
    {
        photonView.RPC("HidePickup", RpcTarget.All);
        yield return new WaitForSeconds(respawnTime);
        photonView.RPC("ShowPickup", RpcTarget.All);
    }

    [PunRPC]
    private void HidePickup()
    {
        pickupCollider.enabled = false;
        pickupRenderer.enabled = false;
        vfx.Stop();
        vfxlight.enabled = false;

    }

    [PunRPC]
    private void ShowPickup()
    {
        pickupCollider.enabled = true;
        pickupRenderer.enabled = true;
        vfx.Play();
        vfxlight.enabled = true;
    }
}


