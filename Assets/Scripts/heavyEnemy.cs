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
        meleeTimer -= Time.deltaTime;
    }

    // --- Core Behavior ---

    protected override void UpdateBehavior()
    {
        if (playerTransform == null) return;

        if (!hasLeftSpawnRoom)
            return;

        Debug.Log("Heavy Enemy State: " + state);

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        
        if (state == EnemyState.Roam)
        {
            
            agent.isStopped = false;
            HandleRoam();

            if (dist <= detectionRange)
            {
                state = EnemyState.Chase;
            }
            return;
        }

        if (state == EnemyState.Chase)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            if (dist <= meleeRange)
                state = EnemyState.Attack;

            if (dist > detectionRange)
                state = EnemyState.Roam;
        }



        if (state == EnemyState.Attack)
        {
            agent.isStopped = true;
            FaceTarget();
            if (meleeTimer > meleeCooldown)
            {
                TryMeleeAttack(); 
            }
        }
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
        //isAttacking = true;

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
        //isAttacking = false;
    }
}