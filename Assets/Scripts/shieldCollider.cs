using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShieldCollider : MonoBehaviour
{
    private heavyEnemy heavyEnemyRef;

    void Start()
    {
        // Get the heavy enemy component from parent
        heavyEnemyRef = GetComponentInParent<heavyEnemy>();
        if (heavyEnemyRef == null)
        {
            Debug.LogError("ShieldCollider must be a child of a heavyEnemy!", this);
        }

        /* Ensure collider is set as trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = false;*/
    }

    /*void OnTriggerEnter(Collider other)
    {
        // Check if a bullet hit the shield
        // Assumes bullets have a "Bullet" tag or a Bullet component
        if (other.CompareTag("Bullet") || other.GetComponent<Bullet>() != null)
        {
            if (heavyEnemyRef != null)
            {
                Vector3 bulletPos = other.transform.position;
                Vector3 bulletDir = other.transform.forward;

                if (heavyEnemyRef.TryDeflectBullet(bulletPos, bulletDir, out Vector3 deflectedDir))
                {
                    // Deflect the bullet
                    Bullet bullet = other.GetComponent<Bullet>();
                    if (bullet != null)
                    {
                        bullet.Deflect(deflectedDir);
                    }
                    else
                    {
                        // Fallback: just rotate the bullet object
                        other.transform.rotation = Quaternion.LookRotation(deflectedDir);
                    }
                }
                else
                {
                    // Bullet hit edge of shield � destroy it or let it pass through
                    // Option A: Destroy bullet (it "absorbed" into the shield edge)
                    Destroy(other.gameObject);
                }
            }
        }
    }*/
}