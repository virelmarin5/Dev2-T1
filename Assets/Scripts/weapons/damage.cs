using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { bullet, stationary, DOT, throwable}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1, 10)][SerializeField] int damageAmount;
    [Range(.1f, 10)][SerializeField] float damageRate;
    [Range(1, 80)][SerializeField] int bulletSpeed;
    [Range(.1f, 20)][SerializeField] int bulletDestroyTime;
    [Range(.1f, 20)][SerializeField] float throwableLifetime = 5f;
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

            if (type == damageType.bullet)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject, throwableLifetime);
            }
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
}