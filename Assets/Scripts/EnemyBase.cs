using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour, IDamage
{
    [Header("Health")]
    [SerializeField] protected int maxHP = 10;
    [SerializeField] protected int currentHP;

    [Header("Movement")]
    [SerializeField] protected float faceTargetSpeed = 8f;
    [SerializeField] float roamRadius = 10f;
    [SerializeField] float roamWaitMin = 1f;
    [SerializeField] float roamWaitMax = 3f;
    [SerializeField] public float detectionRange = 3.0f;

    Vector3 roamCenter;
    Vector3 roamTarget;
    float roamTimer;

    [Header("Visuals")]
    [SerializeField] protected Renderer model;
    [SerializeField] protected Material flashMaterial;
    protected Color colorOrig;

    protected NavMeshAgent agent;
    protected Transform playerTransform;
    protected Vector3 playerDir;
    protected bool isDead;
    protected SpawnRoom mySpawnRoom;
    protected bool hasLeftSpawnRoom = false;

    
    protected enum EnemyState
    {
        Roam,
        Chase,
        Attack
    }

    protected EnemyState state = EnemyState.Roam;

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

    protected void OnDeath()
    {
        // Override in subclass for particles, audio, etc.
        FindAnyObjectByType<killChainManager>().RegisterKill();
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


    //Pick a random point on the navmesh and set destination to that point
    protected void PickRoamTarget()
    {
        if (!hasLeftSpawnRoom) return;
        if (state != EnemyState.Roam) return;

        roamTimer = Random.Range(roamWaitMin, roamWaitMax);

        Vector2 circle = Random.insideUnitCircle * roamRadius;
        Vector3 candidate = roamCenter + new Vector3(circle.x, 0, circle.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            roamTarget = hit.position;
        else
            roamTarget = transform.position;

        agent.SetDestination(roamTarget);
    }

    protected void HandleRoam()
    {
        if (!hasLeftSpawnRoom)
            return;
        if (roamTimer > 0)
        {
            roamTimer -= Time.deltaTime;
            return;
        }
        agent.SetDestination(roamTarget);
        if (Vector3.Distance(transform.position, roamTarget) < 1f)
            PickRoamTarget();
    }

    public void AssignSpawnRoom(SpawnRoom sr)
    {
        Debug.Log($"{name} assigned spawnroom: {sr}");
        mySpawnRoom = sr;
        StartCoroutine(LeaveSpawnRoom());
    }

    protected IEnumerator LeaveSpawnRoom()
    {
        Debug.Log($"{name} isOnNavMesh = {agent.isOnNavMesh}");
        while (mySpawnRoom == null || mySpawnRoom.exit == null)
            yield return null;

        agent.isStopped = false;
        agent.SetDestination(mySpawnRoom.exit.position);

        while (Vector3.Distance(transform.position, mySpawnRoom.exit.position) > 1.5f)
            yield return null;

        hasLeftSpawnRoom = true;
        
        state = EnemyState.Roam;

        roamCenter = transform.position;

        PickRoamTarget();
    }
}