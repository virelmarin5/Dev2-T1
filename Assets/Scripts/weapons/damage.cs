using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { bullet, stationary, DOT }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1, 10)][SerializeField] int damageAmount;
    [Range(.1f, 10)][SerializeField] float damageRate;
    [Range(1, 80)][SerializeField] int bulletSpeed;
    [Range(.1f, 20)][SerializeField] int bulletDestroyTime;
    [SerializeField] string deflectLayer;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.bullet)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
            Destroy(gameObject, bulletDestroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (type == damageType.bullet && ((1 << other.gameObject.layer) & deflectLayer) != 0)
        {
            DeflectBullet(other);
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type != damageType.DOT)
        {
            dmg.takeDamage(damageAmount);
        }

        if (type == damageType.bullet)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    // DOT damage, we do not use it right now
    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    // Coroutine to handle damage over time, we do not use it right now
    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    private void DeflectBullet(Collider other)
    {
        Ray ray = new Ray(transform.position - rb.linearVelocity.normalized, rb.linearVelocity.normalized);
        if (other.Raycast(ray, out RaycastHit hit, 2f))
        {
            // Calculate the reflection vector based on current velocity and surface normal
            Vector3 reflectedVelocity = Vector3.Reflect(rb.linearVelocity, hit.normal);
            
            // Apply the new velocity
            rb.linearVelocity = reflectedVelocity;
        }
    }
}