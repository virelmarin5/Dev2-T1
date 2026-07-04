using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : EnemyBase
{
    [Header("Melee Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float stoppingDistance = 1.5f;

    private float attackTimer;

    protected override  void  Start()
    {
        currentHP = maxHP;

        if (model != null)
            colorOrig = model.material.color;

        Debug.Log("EnemyBase Start running");
        Debug.Log("gameManager.instance: " + gameManager.instance);

        if (gameManager.instance != null)
        {
            Debug.Log("gameManager.instance.player: " + gameManager.instance.player);
            if (gameManager.instance.player != null)
                playerTransform = gameManager.instance.player.transform;
        }

        Debug.Log("playerTransform set to: " + playerTransform);
    }

    protected override void UpdateBehavior()
    {
        if (agent == null || !agent.isActiveAndEnabled)
            return;

        // --- FOLLOW PLAYER ---
        // Try to find the nearest NavMesh point to the player (increased to 50f)
        if (NavMesh.SamplePosition(playerTransform.position, out NavMeshHit hit, 50f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);

            // DEBUG: draw line to where the agent is trying to go
            Debug.DrawLine(transform.position, hit.position, Color.green);
            Debug.Log("Dest: " + hit.position + " | PathStatus: " + agent.pathStatus + " | HasPath: " + agent.hasPath + " | Remaining: " + agent.remainingDistance);
        }
        else
        {
            Debug.LogError("Player is NOT on the NavMesh! Player pos: " + playerTransform.position);
        }

        // --- MELEE ATTACK ---
        float distSqr = (playerTransform.position - transform.position).sqrMagnitude;
        if (distSqr <= attackRange * attackRange)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                MeleeAttack();
                attackTimer = 0f;
            }
        }
    }

    private void MeleeAttack()
    {
        IDamage damageable = playerTransform.GetComponent<IDamage>();
        damageable?.takeDamage(attackDamage);
    }

    protected override void OnDeath() { }

    protected override void Update()
    {
        Debug.Log("EnemyBase Update running. isDead: " + isDead + " | playerTransform: " + playerTransform);

        if (isDead || playerTransform == null)
        {
            Debug.Log("EARLY RETURN — enemy not moving");
            return;
        }

        playerDir = playerTransform.position - transform.position;
        FaceTarget();
        UpdateBehavior();
    }
}