using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] protected int maxHP = 10;
    protected int currentHP;

    [Header("Movement")]
    [SerializeField] protected float faceTargetSpeed = 8f;

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
            colorOrig = model.material.color;

        // Try gameManager first, then fall back to FindWithTag
        if (gameManager.instance != null && gameManager.instance.player != null)
        {
            playerTransform = gameManager.instance.player.transform;
        }
        else
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
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
        if (isDead) return;

        currentHP -= amount;
        Debug.Log("Took damage: " + amount + " | HP: " + currentHP + " | model: " + model);

        if (currentHP <= 0)
        {
            Die();
        }
        else if (model != null)
        {
            Debug.Log("Starting FlashBlack coroutine");
            StartCoroutine(FlashBlack());
        }
        else
        {
            Debug.LogWarning("No model assigned — cannot flash black!");
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        NotifyWaveManager();
        OnDeath();
        Destroy(gameObject);
    }

    protected virtual void NotifyWaveManager()
    {
        // Hook this up to your WaveManager, e.g.:
        // WaveManager.Instance?.OnEnemyDied(this);
        waveManager.instance.enemyKilled();
    }

    protected virtual void OnDeath()
    {
        // Override in subclass for particles, audio, etc.
        
    }

    protected virtual IEnumerator FlashBlack()
    {
        if (model == null || flashMaterial == null) yield break;

        Material[] originalMats = model.materials;
        int count = originalMats.Length;
        Material[] flashMats = new Material[count];

        for (int i = 0; i < count; i++)
            flashMats[i] = flashMaterial;

        model.materials = flashMats;
        yield return new WaitForSeconds(0.15f);
        model.materials = originalMats;
    }

    protected virtual void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }
}