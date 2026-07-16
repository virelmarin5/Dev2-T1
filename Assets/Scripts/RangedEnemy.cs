using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : EnemyBase
{

    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;
    [SerializeField] int gunRotateSpeed;
    float shootTimer;
    bool playerInTrigger;
    protected override void Update()
    {
        base.Update();
    }
    protected override void UpdateBehavior()
    {
        if (playerInTrigger)
        {


            shootTimer += Time.deltaTime;
            rotateGun();
            if (shootTimer >= shootRate)
            {
                shoot();
            }
        }
        if (agent == null || !agent.isActiveAndEnabled)
            return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        float stopDistance = 5.0f; // how far back you want it to stop

        if (dist <= stopDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();
            FaceTarget();
        }
        else
        {
            agent.isStopped = false;
            if (NavMesh.SamplePosition(playerTransform.position, out NavMeshHit hit, 50f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
    }


    protected override void Start()
    {
        base.Start(); // Runs EnemyBase.Start() first (HP, color, playerTransform)
    }

    void shoot()
    {
        if (bullet == null || shootPos == null)
        {
            Debug.LogWarning("Missing bullet or shootPos reference!", this);
            return;
        }
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, gunPivot.rotation);
    }

    void rotateGun()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, rot, gunRotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    public override void takeDamage(int amount)
    {
        base.takeDamage(amount);
    }
}
