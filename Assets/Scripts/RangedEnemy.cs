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
    protected override void Update()
    {
        base.Update();
    }
    protected override void UpdateBehavior()
    {
     
        if (agent == null || !agent.isActiveAndEnabled)
            return;

        if (!hasLeftSpawnRoom)
            return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        
        //Roam when player is far
        if (state == EnemyState.Roam)
        {
            agent.isStopped = false;
            HandleRoam();

            if (dist <= detectionRange)
                state = EnemyState.Chase;
            return;
            
        }

        if (state == EnemyState.Chase)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            if (dist <= detectionRange)
                state = EnemyState.Attack;
            if (dist > detectionRange)
                state = EnemyState.Roam;
            return;
        }

        if (state == EnemyState.Attack)
        {
            shootTimer += Time.deltaTime;
            agent.isStopped = true;
            rotateGun();
            

            if (shootTimer >= shootRate)
                shoot();

            if (dist > 4f)
                state = EnemyState.Chase;

            if (dist > detectionRange)
                state = EnemyState.Roam;
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
}
