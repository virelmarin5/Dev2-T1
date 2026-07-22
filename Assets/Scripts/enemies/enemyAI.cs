using UnityEngine;
using System.Collections;
using UnityEngine.AI;

// Shared brain for all enemy types: roaming, seeing the player, facing them,
// taking damage, and flashing red on hit. Each concrete enemy only has to
// implement attack() (and optionally onPlayerSpotted() for per-frame stuff
// like aiming a gun before it fires).
public abstract class enemyAI_Base : MonoBehaviour, IDamage
{
    [Header("Components")]
    [SerializeField] protected Renderer model;
    [SerializeField] protected NavMeshAgent agent;

    [Header("Stats")]
    [Range(1, 10)][SerializeField] protected int HP;
    [Range(1, 10)][SerializeField] protected int faceTargetSpeed;
    [Range(15, 120)][SerializeField] protected int FOV;
    [SerializeField] protected int roamDist;
    [SerializeField] protected int roamPauseTime;

    [Header("Attack Timing")]
    [Range(0.1f, 3f)][SerializeField] protected float attackRate;
    [Tooltip("How close the player needs to be before attack() is allowed to fire.")]
    [SerializeField] protected float attackRange = 2f;

    protected Color colorOrig;
    protected Vector3 playerDir;
    protected Vector3 startingPos;
    protected bool playerInTrigger;

    protected float attackTimer;
    protected float angleToPlayer;
    protected float roamTimer;
    protected float stoppingDistOrig;

    protected virtual void Start()
    {
        colorOrig = model.material.color;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    protected virtual void Update()
    {
        if (playerInTrigger && canSeePlayer())
        {
            // engagement logic runs inside canSeePlayer()
        }
        else
        {
            checkRoam();
        }
    }

    protected void checkRoam()
    {
        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;

            if (roamTimer > roamPauseTime)
            {
                roam();
            }
        }
    }

    protected void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    protected virtual bool canSeePlayer()
    {
        attackTimer += Time.deltaTime;
        playerDir = gameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                faceTarget();
                onPlayerSpotted();

                float distToPlayer = playerDir.magnitude;
                if (attackTimer > attackRate && distToPlayer <= attackRange)
                {
                    attack();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    // Runs every frame the player is visible, before the attack-rate/range
    // check. Ranged uses this to rotate the gun pivot toward the player;
    // melee/heavy can leave it empty.
    protected virtual void onPlayerSpotted() { }

    // Each enemy type defines its own attack. Implementations should reset
    // attackTimer = 0 themselves so cooldown timing stays per-subclass.
    protected abstract void attack();

    protected void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            agent.stoppingDistance = 0;
        }
    }

    public virtual void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    protected IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}