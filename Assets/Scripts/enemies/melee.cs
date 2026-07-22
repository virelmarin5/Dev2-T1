using UnityEngine;
using System.Collections;

// Swings a weapon pivoted at handPos through an arc, and lands a hit via an
// overlap sphere partway through the swing (so the hit lines up visually
// with the weapon actually being near the player).
public class enemyMelee : enemyAI_Base
{
    [Header("Melee Weapon")]
    [SerializeField] Transform handPos;
    [SerializeField] float swingArc = 90f;
    [Range(1, 10)][SerializeField] float swingSpeed = 5f;
    [SerializeField] int meleeDamage = 1;
    [SerializeField] float hitRadius = 1.5f;

    bool isSwinging;

    protected override void attack()
    {
        if (isSwinging) return;
        attackTimer = 0;
        StartCoroutine(swing());
    }

    IEnumerator swing()
    {
        isSwinging = true;

        Quaternion startRot = handPos.localRotation;
        Quaternion swungRot = startRot * Quaternion.Euler(0, swingArc, 0);

        bool hasHit = false;
        float t = 0f;

        // swing weapon out
        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;
            handPos.localRotation = Quaternion.Lerp(startRot, swungRot, t);

            // check for the hit partway through the swing, once,
            // roughly when the weapon is passing through the player
            if (!hasHit && t > 0.4f)
            {
                hasHit = tryHitPlayer();
            }

            yield return null;
        }

        // swing weapon back to rest
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * swingSpeed;
            handPos.localRotation = Quaternion.Lerp(swungRot, startRot, t);
            yield return null;
        }

        handPos.localRotation = startRot;
        isSwinging = false;
    }

    bool tryHitPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(handPos.position, hitRadius);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Player"))
            {
                IDamage dmg = col.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.takeDamage(meleeDamage);
                    return true;
                }
            }
        }
        return false;
    }
}