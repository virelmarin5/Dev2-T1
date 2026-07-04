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

        if (gameManager.instance != null && gameManager.instance.player != null)
            playerTransform = gameManager.instance.player.transform;

        gameManager.instance?.updateGameGoal(1);
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

        if (currentHP <= 0)
        {
            Die();
        }
        else if (model != null)
        {
            StartCoroutine(FlashRed());
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        gameManager.instance?.updateGameGoal(-1);
        NotifyWaveManager();
        OnDeath();
        Destroy(gameObject);
    }

    protected virtual void NotifyWaveManager()
    {
        // Example: WaveManager.Instance?.OnEnemyDied(this);
    }

    protected virtual void OnDeath() { }

    protected virtual IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    protected virtual void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }
}