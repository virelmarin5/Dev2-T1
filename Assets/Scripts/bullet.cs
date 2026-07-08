using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;

    private Vector3 moveDirection;
    private bool isDeflected = false;
    private float timer;

    void Start()
    {
        moveDirection = transform.forward;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Move bullet
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    /// <summary>
    /// Call this when the bullet is deflected by a shield
    /// </summary>
    public void Deflect(Vector3 newDirection)
    {
        moveDirection = newDirection;
        transform.rotation = Quaternion.LookRotation(newDirection);
        isDeflected = true;

        // Optional: change bullet color to show deflection
        // Or change layer so it can now damage enemies
        gameObject.layer = LayerMask.NameToLayer("DeflectedBullet");
    }

    void OnTriggerEnter(Collider other)
    {
        // Don't hit the shooter
        if (other.CompareTag("Enemy") && !isDeflected) return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);
        }

        // Destroy on impact
        Destroy(gameObject);
    }
}