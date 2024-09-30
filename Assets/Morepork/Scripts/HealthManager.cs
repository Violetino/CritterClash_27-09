using Morepork.FinalCharacterController;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviourPun
{
    public int health;
    public int maxHealth = 100;
    public bool isLocalPlayer;

    public RectTransform healthBar;
    private float originalHealthBarSize;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    private Animator animator;

    private void Start()
    {
        originalHealthBarSize = healthBar.sizeDelta.x;
        ResetHealth();

        animator = GetComponent<Animator>();
    }

    private void ResetHealth()
    {
        // Reset health to max health and update UI
        health = maxHealth;
        UpdateHealthBar();
        UpdateHealthText();
    }

    [PunRPC]
    public void TakeDamage(int _damage)
    {
        health -= _damage;
        health = Mathf.Clamp(health, 0, maxHealth);  // Ensure health never goes below 0

        UpdateHealthBar();
        UpdateHealthText();

        if (health <= 0)
        {
            HandleDeath();
            UpdateHealthBar();
            UpdateHealthText();
        }
    }

    private void HandleDeath()
    {
        UpdateHealthBar();
        UpdateHealthText();
        Debug.Log("Player died.");
        
        if (animator != null)
        {
            Debug.Log("Play KO Ainmation");
            animator.SetTrigger("IsKnockedOut");
        }

        // Disable player controls via PlayerSetup
        GetComponent<PlayerSetup>().OnPlayerDeath();

        if (isLocalPlayer)
        {
            RoomManager.instance.deaths++;
            RoomManager.instance.SetHashes();

            StartCoroutine(RespawnPlayer());
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(3f);

        RoomManager.instance.SpawnPlayer();
        GetComponent<PlayerAnimation>().ResetDeathAnimation();  // Reset death animation state
        PhotonNetwork.Destroy(gameObject);  // Destroy the current player object after respawn
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        UpdateHealthText();
        UpdateHealthBar();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.sizeDelta = new Vector2(originalHealthBarSize * Mathf.Clamp(health, 0, maxHealth) / maxHealth, healthBar.sizeDelta.y);
        }
    }


}


