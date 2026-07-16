using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : EnemyBase
{
    [Header("Melee Attack")]
    [SerializeField] private float attackRange = 4f;
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
        currentHP = maxHP;

        if (model != null)
            colorOrig = model.material.color;

        GameObject weaponInstance = Instantiate(weapon, handPos);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;

        katanaTransform = weaponInstance.transform;
        katanaOrigRot = katanaTransform.localRotation;



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

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        float stopDistance = 3.5f; // how far back you want it to stop

        if (dist <= stopDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();

            // Still face the player
            FaceTarget();
        }
        else
        {
            agent.isStopped = false;
            if (NavMesh.SamplePosition(playerTransform.position, out NavMeshHit hit, 50f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }

        // Melee attack when in attack range (separate from stop distance)
        if (dist <= attackRange)
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

    public override void takeDamage(int amount)
    {
        base.takeDamage(amount);
    }
}