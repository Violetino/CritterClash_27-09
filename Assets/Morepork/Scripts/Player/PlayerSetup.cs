using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;
using Morepork.FinalCharacterController;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public PlayerController controller;
    public PlayerLocomotionInput locomotionInput;
    public GameObject dummycam; // This represents the player's camera
    public PlayerInputActions playerInputActions;
    public ThirdPersonInput thirdPersonInput;
    public string nickname;
    public TextMeshPro nicknameText;
    public GameObject killcam;

    private bool isDead = false; // Track if the player is dead

    private void Start()
    {
        // Automatically call IsLocalPlayer when the player spawns
        IsLocalPlayer();
    }

    public void IsLocalPlayer()
    {
        // Only enable the player controls and camera if this is the local player and not dead
        if (photonView.IsMine && !isDead)
        {
            Debug.Log("Enabling player controls and camera for local player.");
            EnablePlayerComponents();
            photonView.RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        }
        else
        {
            Debug.Log("Disabling camera for non-local player.");
            // Disable the camera for remote players
            if (dummycam != null) dummycam.SetActive(false);
        }
    }

    private void EnablePlayerComponents()
    {
        Debug.Log("Enabling player components...");
        if (controller != null) controller.enabled = true;
        if (locomotionInput != null) locomotionInput.enabled = true;
        if (playerInputActions != null) playerInputActions.enabled = true;
        if (thirdPersonInput != null) thirdPersonInput.enabled = true;
        if (dummycam != null) dummycam.SetActive(true); // Enable the camera only for the local player
    }

    // Call this method when the player dies
    public void OnPlayerDeath()
    {
        // Only enable the kill cam for the local player who dies
        if (photonView.IsMine)
        {
            if (killcam != null)
            {
                killcam.SetActive(true);
            }

            Debug.Log("Activating kill cam for the local player.");
        }

        Debug.Log("Disabling player controls due to death.");
        isDead = true;
        DisablePlayerComponents();
    }

    // Call this method when the player respawns
    public void OnPlayerRespawn()
    {
        // Deactivate the kill cam for the local player who respawns
        if (photonView.IsMine)
        {
            if (killcam != null)
            {
                killcam.SetActive(false);
            }

            Debug.Log("Deactivating kill cam for the local player.");
        }

        Debug.Log("Respawning player. Enabling controls again...");
        isDead = false;
        EnablePlayerComponents();  // Re-enable player components after respawn
    }

    // Disable components if needed (e.g., when player dies)
    public void DisablePlayerComponents()
    {
        Debug.Log("Disabling player components...");
        if (controller != null) controller.enabled = false;
        if (locomotionInput != null) locomotionInput.enabled = false;
        if (playerInputActions != null) playerInputActions.enabled = false;
        if (thirdPersonInput != null) thirdPersonInput.enabled = false;
        if (dummycam != null) dummycam.SetActive(false);
    }

    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;
        nicknameText.text = nickname;
    }
}





