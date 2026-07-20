using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class heavyEnemy : EnemyBase
{
    [Header("Melee")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private int meleeDamage = 15;
    [SerializeField] private float meleeCooldown = 1.5f;
    //[SerializeField] private float meleeKnockback = 5f;

    [Header("Heavy Stats")]
    [SerializeField] private float moveSpeed = 2.5f;        // Slower than regular enemies
    //[SerializeField] private float attackRange = 2f;

    private float meleeTimer;
    //private bool isAttacking = false;

    // --- Setup ---

    protected override void Awake()
    {
        base.Awake();
        maxHP = 50; // High HP
        agent.speed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();
    }

    // --- Core Behavior ---

    protected override void UpdateBehavior()
    {
        if (playerTransform == null) return;

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Always face the player (shield faces them)
        FaceTarget();

        // Move toward player if not in melee range
        if (distToPlayer > meleeRange)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            // In melee range � stop and attack
            agent.isStopped = true;
            TryMeleeAttack();
        }

        // Cooldown management
        meleeTimer -= Time.deltaTime;
    }

    // --- Melee Attack ---

    void TryMeleeAttack()
    {
        if (meleeTimer > 0) return;

        meleeTimer = meleeCooldown;
        StartCoroutine(MeleeAttackRoutine());
    }

    System.Collections.IEnumerator MeleeAttackRoutine()
    {
        // Small wind-up delay (optional telegraph)
        yield return new WaitForSeconds(0.3f);

        // Check if player is still in range
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= meleeRange)
        {

            IDamage playerDmg = playerTransform.GetComponent<IDamage>();
            playerDmg?.takeDamage(meleeDamage);

            // Pushback via CharacterController instead of Rigidbody
            playerController pc = playerTransform.GetComponent<playerController>();
            if (pc != null)
            {
                Vector3 pushDir = (playerTransform.position - transform.position).normalized;
                pc.PushBack(pushDir);
            }

        }

        yield return new WaitForSeconds(0.2f);
    }

    public override void takeDamage(int amount)
    {
        base.takeDamage(amount);
    }
}