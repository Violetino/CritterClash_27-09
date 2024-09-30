using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 20f;
        bulletRigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform vfx = null;
        Vector3 hitPosition = transform.position; // Default to current position

        // Calculate the collision point and normal
        if (other is MeshCollider meshCollider && meshCollider.sharedMesh != null)
        {
            // If it's a MeshCollider with a valid mesh, perform more accurate calculations
            RaycastHit hit;
            if (Physics.Raycast(transform.position - transform.forward, transform.forward, out hit))
            {
                hitPosition = hit.point;
            }
        }
        else if (other.ClosestPoint(transform.position) != transform.position)
        {
            // Use ClosestPoint to get the point on the collider closest to the current position
            hitPosition = other.ClosestPoint(transform.position);
        }

        Vector3 collisionNormal = (hitPosition - transform.position).normalized;

        // Offset the VFX spawn position along the normal vector
        Vector3 offset = collisionNormal * 1f; // Adjust offset as needed
        Vector3 vfxSpawnPosition = hitPosition + offset;

        if (other.GetComponent<BulletTarget>() != null)
        {
            // Hit target
            vfx = Instantiate(vfxHitGreen, vfxSpawnPosition, Quaternion.LookRotation(collisionNormal));
        }
        else
        {
            // Hit something else
            vfx = Instantiate(vfxHitRed, vfxSpawnPosition, Quaternion.LookRotation(collisionNormal));
        }

        if (vfx != null)
        {
            // Schedule the VFX destruction after it has finished playing
            ParticleSystem particleSystem = vfx.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                // Destroy the VFX GameObject after the particle system duration
                Destroy(vfx.gameObject, particleSystem.main.duration);
            }
            else
            {
                // If no ParticleSystem component found, destroy immediately
                Destroy(vfx.gameObject, 2f); // Arbitrary time, adjust if needed
            }
        }

        Destroy(gameObject); // Destroy the bullet
    }

}

