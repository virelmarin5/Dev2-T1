
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] protected int maxHP;
    [SerializeField] protected int currentHP;

    [Header("Movement")]
    [SerializeField] protected float faceTargetSpeed;

    [Header("Visuals")]
    [SerializeField] protected Renderer model;
    [SerializeField] protected Material flashMaterial;

    protected Color colorOrig;
    protected NavMeshAgent agent;
    protected Transform playerTransform;
    protected Vector3 playerDir;
    protected bool isDead;

    protected abstract void UpdateBehavior();

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        currentHP = maxHP;

        if (model != null)
        {
            colorOrig = model.material.color;
        }

        // Try to get the player through GameManager first.
        if (gameManager.instance != null &&
            gameManager.instance.player != null)
        {
            playerTransform = gameManager.instance.player.transform;
        }
        else
        {
            // Fallback in case GameManager has not found the player yet.
            GameObject playerObject = GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }

    protected virtual void Update()
    {
        if (isDead || playerTransform == null)
            return;

        playerDir = playerTransform.position - transform.position;

        FaceTarget();
        UpdateBehavior();
    }

    public virtual void takeDamage(int amount)
    {
        if (isDead)
            return;

        currentHP -= amount;

        Debug.Log(
            "Enemy took damage: " + amount +
            " | Current HP: " + currentHP
        );

        if (currentHP <= 0)
        {
            Die();
        }
        else if (model != null)
        {
            StartCoroutine(FlashBlack());
        }
        else
        {
            Debug.LogWarning(
                "EnemyBase: No model assigned, so damage flash cannot play."
            );
        }
    }

    /// <summary>
    /// Immediately kills this enemy while still running the normal
    /// WaveManager, kill-chain, death-effect, and cleanup sequence.
    /// Used by killstreaks such as the Nuke.
    /// </summary>
    public void ForceKill()
    {
        if (isDead)
            return;

        Die();
    }

    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;

        NotifyWaveManager();
        OnDeath();

        Destroy(gameObject);
    }

    protected virtual void NotifyWaveManager()
    {
        if (waveManager.instance != null)
        {
            waveManager.instance.enemyKilled();
        }
    }

    protected virtual void OnDeath()
    {
        // Normal player kills are registered here.
        // The Nuke temporarily tells KillChainManager to ignore these calls.
        if (killChainManager.instance != null)
        {
            killChainManager.instance.RegisterKill();
        }
    }

    protected virtual IEnumerator FlashBlack()
    {
        if (model == null || flashMaterial == null)
            yield break;

        Material[] originalMaterials = model.materials;
        Material[] flashMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < flashMaterials.Length; i++)
        {
            flashMaterials[i] = flashMaterial;
        }

        model.materials = flashMaterials;

        yield return new WaitForSeconds(0.15f);

        if (model != null)
        {
            model.materials = originalMaterials;
        }
    }

    protected virtual void FaceTarget()
    {
        if (playerDir.sqrMagnitude <= 0f)
            return;

        Vector3 flatDirection =
            new Vector3(playerDir.x, 0f, playerDir.z);

        if (flatDirection.sqrMagnitude <= 0f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(flatDirection);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            faceTargetSpeed * Time.deltaTime
        );
    }
}