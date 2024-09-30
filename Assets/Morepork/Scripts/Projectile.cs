using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPun
{
    public float lifetime = 5f; // Duration before the projectile is destroyed
    public int damage = 10; // Damage dealt by the projectile

    private void Start()
    {
        // Ignore collision with the shooter and all its colliders
        Collider projectileCollider = GetComponent<Collider>();
        GameObject shooter = PhotonView.Find(photonView.OwnerActorNr).gameObject;

        if (shooter != null)
        {
            Collider[] shooterColliders = shooter.GetComponentsInChildren<Collider>();
            foreach (Collider shooterCollider in shooterColliders)
            {
                if (shooterCollider != null)
                {
                    Physics.IgnoreCollision(projectileCollider, shooterCollider);
                }
            }
        }

        if (photonView.IsMine)
        {
            // Destroy after the set lifetime
            Destroy(gameObject, lifetime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile hit a player
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    HealthManager healthManager = collision.gameObject.GetComponent<HealthManager>();
        //    if (healthManager != null)
        //    {
        //        // Damage only other players, not yourself
        //        if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
         //       {
         //           healthManager.TakeDamage(damage);
        //        }
        //    }
      //  }
//
        // Destroy the projectile after any collision, including with the environment
     //   if (photonView.IsMine)
     //   {
       //     PhotonNetwork.Destroy(gameObject);
      //  }
    }
}


