using Cinemachine;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class WeaponManager : MonoBehaviourPun
{
    [Header("Weapon Stats")]
    public int damage = 10;
    public float fireRate = 1f;
    public CinemachineVirtualCamera aimCamera;
    public Camera mainCamera;
    public float nextFireTime;

    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;

    [Header("VFX")]
    public GameObject _hitVFX;

    [Header("Raycast Options")]
    public Vector3 raycastOriginOffset = Vector3.zero;
    public Vector3 hitOffset = Vector3.zero;

    [Header("UI Elements")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    [Header("Recoil Settings")]
    public float recoilAmount = 5f;
    public float recoilRecoverySpeed = 10f;
    public float noiseAmplitude = 2f;
    public float noiseFrequency = 2f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 50f;
    public Transform muzzlePoint; // The point where the projectile will originate from

    private CinemachineBasicMultiChannelPerlin cameraNoise;

    private void Start()
    {
        UpdateUI();

        if (mainCamera == null && aimCamera != null)
        {
            mainCamera = aimCamera.VirtualCameraGameObject.GetComponent<Camera>();
            if (mainCamera != null)
            {
                Debug.Log("Camera found and assigned from Cinemachine aim camera.");
            }
            else
            {
                Debug.LogError("No Camera component found on the Cinemachine aim camera!");
            }
        }
        else if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned in the inspector!");
        }

        if (aimCamera != null)
        {
            // Access the CinemachineBasicMultiChannelPerlin component
            cameraNoise = aimCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (cameraNoise == null)
            {
                Debug.LogError("No CinemachineBasicMultiChannelPerlin component found on the aim camera!");
            }
        }
        else
        {
            Debug.LogError("Aim Camera not assigned!");
        }
    }

    private void Update()
    {
        if (nextFireTime > 0)
        {
            nextFireTime -= Time.deltaTime;
        }

        // Smoothly return the noise values to zero
        if (cameraNoise != null)
        {
            cameraNoise.m_AmplitudeGain = Mathf.Lerp(cameraNoise.m_AmplitudeGain, 0f, Time.deltaTime * recoilRecoverySpeed);
            cameraNoise.m_FrequencyGain = Mathf.Lerp(cameraNoise.m_FrequencyGain, 0f, Time.deltaTime * recoilRecoverySpeed);
        }
    }

    public void AmmoUsed()
    {
        if (ammo <= 0)
        {
            Debug.Log("No ammo left!");
            return;
        }

        nextFireTime = 1 / fireRate;
        ammo--;
        UpdateUI();
    }

    public void Reload()
    {
        if (mag <= 0 || ammo == magAmmo)
        {
            Debug.Log("Cannot reload. Either no mags or ammo is full.");
            return;
        }

        int bulletsNeeded = magAmmo - ammo;
        ammo += Mathf.Min(bulletsNeeded, magAmmo);
        mag -= 1;

        UpdateUI();
    }

    public void Fire()
    {
        if (Time.time < nextFireTime)
            return;

        if (ammo <= 0)
        {
            Debug.Log("No ammo left!");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("No camera assigned for firing mechanism!");
            return;
        }

        int layerMask = LayerMask.GetMask("Default", "Player");

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Vector3 shootDirection = ray.direction;
        if (Physics.Raycast(ray.origin + raycastOriginOffset, ray.direction, out hit, 100f, layerMask))
        {
            shootDirection = (hit.point - muzzlePoint.position).normalized;

            Vector3 hitPointWithOffset = hit.point + hitOffset;
            PhotonNetwork.Instantiate(_hitVFX.name, hitPointWithOffset, Quaternion.identity);

            HealthManager healthManager = hit.transform.gameObject.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                PhotonView targetPhotonView = hit.transform.gameObject.GetComponent<PhotonView>();
                if (targetPhotonView != null)
                {
                    PhotonNetwork.LocalPlayer.AddScore(damage); // Damage Points
                    if (damage >= hit.transform.gameObject.GetComponent<HealthManager>().health)
                    {
                            RoomManager.instance.kills++;  // Only increment kills once here
                            RoomManager.instance.SetHashes();                        
                        //PhotonNetwork.LocalPlayer.AddScore(1);
                    }
                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, (int)damage);
                }
                else
                {
                    Debug.LogError("No PhotonView component found on the hit object.");
                }
            }
        }

        // Spawn and shoot the projectile from the muzzle point across the network
        GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, muzzlePoint.position, Quaternion.LookRotation(shootDirection));
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDirection * projectileSpeed;
        }

        // Apply recoil with noise
        ApplyRecoilWithNoise();

        AmmoUsed();
    }

    private void ApplyRecoilWithNoise()
    {
        if (cameraNoise != null)
        {
            cameraNoise.m_AmplitudeGain = noiseAmplitude;
            cameraNoise.m_FrequencyGain = noiseFrequency;
        }
    }

    private void UpdateUI()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
    }
    public void AddMagazine(int amount)
    {
        Debug.Log($"Adding {amount} magazines. Current mag count: {mag}");

        if (magAmmo <= 0)
        {
            Debug.Log("Mag ammo is not set!");
            return;
        }

        mag += amount;
        UpdateUI();
    }
}



















