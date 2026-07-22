using System.Collections;
using UnityEngine;

public class BasicEnemy : EnemyBase
{
    [Header("Melee")]
    [SerializeField] GameObject weapon;
    [SerializeField] Transform handPos;

    private Quaternion katanaOrigRot;
    private Transform katanaTransform; 

    protected override void Start()
    {
        base.Start();
        
        GameObject weaponInstance = Instantiate(weapon, handPos);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;

        katanaTransform = weaponInstance.transform;
        katanaOrigRot = katanaTransform.localRotation;
        attackTimer += Time.deltaTime;
    }

    protected override void attack(float distToPlayer, Transform playerTransform)
    {
        agent.stoppingDistance = attackRange;
        
        if (distToPlayer <= attackRange && attackTimer > attackRate)
        {
            attackTimer = 0;
            StartCoroutine(katanaSwing());
            
            IDamage damageable = playerTransform.GetComponent<IDamage>();
            damageable?.takeDamage(attackDamage);
        }
    }

    private IEnumerator katanaSwing()
    {
        float duration = 0.1f;
        float t = 0f;

        Quaternion startRot = katanaOrigRot;
        Quaternion endRot = katanaOrigRot * Quaternion.Euler(60f, 0f, 0f);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            katanaTransform.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            katanaTransform.localRotation = Quaternion.Lerp(endRot, startRot, t);
            yield return null;
        }
    }
}