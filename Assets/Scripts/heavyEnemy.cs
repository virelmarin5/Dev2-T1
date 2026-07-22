using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class heavyEnemy : EnemyBase
{
    [SerializeField] float pushbackForce = 5f;
    
    protected override void attack(float distToPlayer, Transform playerTransform)
    {
        agent.stoppingDistance = attackRange;

        if (distToPlayer <= attackRange && attackTimer > attackRate)
        {
            attackTimer = 0;

            // Apply Damage
            IDamage damageable = playerTransform.GetComponent<IDamage>();
            damageable?.takeDamage(attackDamage);

            // Pushback Logic
            playerController pc = playerTransform.GetComponent<playerController>();
            if (pc != null)
            {
                Vector3 pushDir = (playerTransform.position - transform.position).normalized;
                pc.PushBack(pushDir, pushbackForce);
            }
        }
    }
}