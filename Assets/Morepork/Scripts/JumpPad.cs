using Morepork.FinalCharacterController;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpBoost = 20f; // The strength of the jump boost
    [SerializeField] private Animator jumpPadAnimator; // Reference to the Animator for the jump pad animation
    private string playerTag = "Player"; // Tag to identify the player

    // Detect when something enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is the player
        if (other.CompareTag(playerTag))
        {
            // Get the PlayerController component from the player
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Apply the jump boost
                playerController.BoostJump(jumpBoost);

                // Play the jump pad animation
                if (jumpPadAnimator != null)
                {
                    jumpPadAnimator.SetTrigger("JumpTrigger");
                }
            }
        }
    }
}

