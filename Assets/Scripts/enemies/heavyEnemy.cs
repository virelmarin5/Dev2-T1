using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class heavyEnemy : EnemyBase
{
    [SerializeField] float pushbackForce = 5f;
    [SerializeField] float retreatDistance = 5f;
    [SerializeField] float windUpTime = 0.6f;
    [SerializeField] float chargeSpeed = 12f;
    [SerializeField] float chargeMaxDuration = 2f;
    [SerializeField] float hitRange = 1.5f;
    [SerializeField] float recoverTime = 1f;

    float agentSpeedOrig;
    bool isCharging;

    protected override void Start()
    {
        base.Start();
        agentSpeedOrig = agent.speed;
    }

    protected override void attack()
    {
        if (isCharging) return;

        agent.stoppingDistance = Mathf.Max(0.5f, attackRange - 0.5f);
        float dist = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        if (dist > attackRange) return;

        if (attackTimer > attackRate)
        {
            attackTimer = 0;

            StartCoroutine(chargeSequence());
        }
    }

    IEnumerator chargeSequence()
    {
        isCharging = true;

        // 1. Back away from the player
        agent.isStopped = false;
        Vector3 player = gameManager.instance.player.transform.position;
        Vector3 awayDir = (transform.position - player).normalized;
        if (awayDir == Vector3.zero) awayDir = -transform.forward;

        if (NavMesh.SamplePosition(transform.position + awayDir * retreatDistance, out NavMeshHit retreatHit, retreatDistance, NavMesh.AllAreas))
            agent.SetDestination(retreatHit.position);

        float retreatTimer = 0f;
        while (retreatTimer < 2f)
        {
            retreatTimer += Time.deltaTime;
            if (!agent.pathPending && agent.remainingDistance <= 0.2f) break;
            yield return null;
        }

        // 2. Wind up / telegraph - freeze in place
        agent.isStopped = true;
        agent.ResetPath();
        yield return new WaitForSeconds(windUpTime);

        // 3. Charge at the player's current position
        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.speed = chargeSpeed;

        Vector3 playerPosAtStart = gameManager.instance.player.transform.position;
        Vector3 chargeDir = (playerPosAtStart - transform.position).normalized;
        Vector3 chargeTarget = playerPosAtStart + chargeDir * 2f; // overshoot so it doesn't stop short

        if (NavMesh.SamplePosition(chargeTarget, out NavMeshHit chargeHit, 10f, NavMesh.AllAreas))
            agent.SetDestination(chargeHit.position);

        float chargeTimer = 0f;
        bool hit = false;
        while (chargeTimer < chargeMaxDuration)
        {
            chargeTimer += Time.deltaTime;

            float distToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
            if (distToPlayer <= hitRange)
            {
                hit = true;
                break;
            }

            if (!agent.pathPending && agent.remainingDistance <= 0.1f)
                break; // reached the charge target without connecting

            yield return null;
        }

        agent.speed = agentSpeedOrig;

        if (hit)
        {
            IDamage damageable = gameManager.instance.player.gameObject.GetComponent<IDamage>();
            damageable?.takeDamage(attackDamage);

            playerController pc = gameManager.instance.player.gameObject.GetComponent<playerController>();
            if (pc != null)
            {
                Vector3 pushDir = (gameManager.instance.player.transform.position - transform.position).normalized;
                pc.PushBack(pushDir, pushbackForce);
            }
        }

        // 4. Recover before the next cycle can start
        agent.isStopped = true;
        agent.ResetPath();
        yield return new WaitForSeconds(recoverTime);
        agent.isStopped = false;

        isCharging = false;
    }
}