using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class heavyEnemy : EnemyBase
{
    [Header("Shield")]
    [SerializeField] private Collider shieldCollider;       // The shield's collider (trigger or normal)
    [SerializeField] private Transform shieldTransform;      // Reference to shield for angle checks
    [SerializeField] private float shieldDeflectionAngle = 75f; // Degrees from forward to deflect
    //[SerializeField] private float deflectionForce = 15f;   // Optional: force for physics-based deflection

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
        // Ensure shield collider is set up properly
        if (shieldCollider != null)
        {
            shieldCollider.gameObject.tag = "Shield";
            shieldCollider.gameObject.layer = LayerMask.NameToLayer("Shield");
        }
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

    // --- Shield Deflection ---


    public bool TryDeflectBullet(Vector3 bulletPosition, Vector3 bulletDirection, out Vector3 deflectedDirection)
    {
        deflectedDirection = bulletDirection;

        // Check if bullet is hitting the front of the shield
        Vector3 toBullet = (bulletPosition - shieldTransform.position).normalized;
        float angleToShield = Vector3.Angle(shieldTransform.forward, toBullet);

        // Only deflect if hitting the front face of the shield
        if (angleToShield > shieldDeflectionAngle / 2f)
        {
            return false; // Hit the side/back of shield � bullet goes through to enemy
        }

        // Deflect the bullet (ricochet off)
        // Reflect the bullet direction across the shield's normal
        Vector3 shieldNormal = shieldTransform.forward;
        deflectedDirection = Vector3.Reflect(bulletDirection, shieldNormal);

        // Optional: add some randomness to deflection
        deflectedDirection += Random.insideUnitSphere * 0.1f;
        deflectedDirection.Normalize();

        // Visual/audio feedback
        Debug.Log("Shield deflected bullet!");
        // Play deflect sound/particles here

        return true;
    }

    // --- Damage Override (Shield Protection) ---

    public override void takeDamage(int amount)
    {
        // Check if hit came from the front (shield side)
        // We can check this by seeing if the damage source is in front of us
        // For simplicity, we'll assume the player is always in front when we face them
        // But you may want to pass hit direction from the bullet

        // If you want the shield to fully block damage from the front:
        // (Comment this out if you want bullets to always damage, just deflect from shield collider)

        // For now, we let the shield collider handle deflection separately
        // and normal damage applies if the bullet bypasses the shield
        base.takeDamage(amount);
    }

    // --- Death Override ---



    // --- Gizmos ---

    void OnDrawGizmosSelected()
    {
        // Melee range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // Shield deflection angle
        if (shieldTransform != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 leftBound = Quaternion.Euler(0, -shieldDeflectionAngle / 2f, 0) * shieldTransform.forward;
            Vector3 rightBound = Quaternion.Euler(0, shieldDeflectionAngle / 2f, 0) * shieldTransform.forward;
            Gizmos.DrawLine(shieldTransform.position, shieldTransform.position + leftBound * 3f);
            Gizmos.DrawLine(shieldTransform.position, shieldTransform.position + rightBound * 3f);
        }
    }
}