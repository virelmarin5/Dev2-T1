using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : EnemyBase
{
    
    

    [Header("Melee Attack")]
    [SerializeField] private float attackRange = 5.0f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRate = 1f;
    //[SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] GameObject weapon;
    [SerializeField] Transform handPos;

    private Quaternion katanaOrigRot;
    private Transform katanaTransform;
    private float attackTimer;
    private killChainManager killChain;
    

    protected override  void  Start()
    {
        base.Start();
        currentHP = maxHP;

        if (model != null)
            colorOrig = model.material.color;

        GameObject weaponInstance = Instantiate(weapon, handPos);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;

        katanaTransform = weaponInstance.transform;
        katanaOrigRot = katanaTransform.localRotation;
        attackTimer += Time.deltaTime;



        if (gameManager.instance != null)
        {

            if (gameManager.instance.player != null)
                playerTransform = gameManager.instance.player.transform;
        }


    }
    protected override void UpdateBehavior()
    {
        
        if (agent == null || !agent.isActiveAndEnabled)
            return;

        if (!hasLeftSpawnRoom)
            return;


        Debug.Log("State: " + state);

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        bool playerDetected = dist <= detectionRange;
        //Roam when player is outside of detection range
        if (state == EnemyState.Roam)
        {
            agent.isStopped = false;
            HandleRoam();

            if (dist <= detectionRange)
                state = EnemyState.Chase;

            return;
        }

        //Chase when player is within detection range
        if (state == EnemyState.Chase)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            
            if (dist <= attackRange)
                state = EnemyState.Attack;
            if (dist > detectionRange)
                state = EnemyState.Roam;

            return;
        }

        if (state == EnemyState.Attack)
        {
            agent.isStopped = true;
            FaceTarget();

            if (dist > attackRange)
                state = EnemyState.Chase;

            if (attackTimer >= attackRate)
            {
                attackTimer = 0;
                MeleeAttack();
            }

            return;
        }
    }

    private void MeleeAttack()
    {
        StartCoroutine(KatanaSwing());
        IDamage damageable = playerTransform.GetComponent<IDamage>();
        damageable?.takeDamage(attackDamage);
    }


    protected override void Update()
    {


        if (isDead || playerTransform == null)
        {
            Debug.Log("EARLY RETURN � enemy not moving");
            return;
        }

        playerDir = playerTransform.position - transform.position;
        FaceTarget();
        UpdateBehavior();
    }

    private IEnumerator KatanaSwing()
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