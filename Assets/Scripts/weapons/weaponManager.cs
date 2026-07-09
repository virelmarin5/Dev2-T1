using UnityEngine;

public class weaponManager : MonoBehaviour
{
    public static weaponManager instance { get; private set; }

    [Header("Weapon")]
    public weaponStats activeWeapon;
    public GameObject spawnedWeaponModel;


    [SerializeField] public GameObject weaponHoldPos;
    public Transform gunBarrel;
    private float attackTimer;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        attackTimer += Time.unscaledDeltaTime;
    }

    public void equipWeapon(weaponStats newWeapon)
    {
        if (newWeapon == null || weaponHoldPos == null) return;

        // Drop and destroy current weapon
        if (spawnedWeaponModel != null)
        {
            Vector3 dropPos = transform.position + transform.forward * 1.5f + transform.up * 0.5f;
            Instantiate(activeWeapon.weaponModel, dropPos, Quaternion.identity);
            Destroy(spawnedWeaponModel);
        }

        activeWeapon = newWeapon;
        // Spawn model directly to target
        spawnedWeaponModel = Instantiate(newWeapon.weaponModel, weaponHoldPos.transform, false);

        spawnedWeaponModel.transform.localPosition = Vector3.zero;
        spawnedWeaponModel.transform.localRotation = Quaternion.identity;

        if (spawnedWeaponModel.TryGetComponent<Collider>(out var col)) col.enabled = false;

        // Locate the barrel or hitpoint
        string targetName = (newWeapon is gunStats) ? "Muzzle" : "HitPoint";
        gunBarrel = FindDeepChild(spawnedWeaponModel.transform, targetName);
    }

    public void throwCurrentWeapon()
    {
        if (activeWeapon == null || spawnedWeaponModel == null) return;

        // Instantiate the world version of the weapon
        GameObject thrownObj = Instantiate(activeWeapon.weaponModel, Camera.main.transform.position + Camera.main.transform.forward * 1.0f, Camera.main.transform.rotation);

        if (!thrownObj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb = thrownObj.AddComponent<Rigidbody>();
        }

        // Turn on colliders on
        if (thrownObj.TryGetComponent<Collider>(out var col))
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        // Apply the throwing force velocity
        rb.linearVelocity = Camera.main.transform.forward * activeWeapon.throwForce;

        Destroy(spawnedWeaponModel);
        activeWeapon = null;
        gunBarrel = null;
    }

    // find nested children
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    public void attack()
    {
        if (activeWeapon == null || attackTimer < activeWeapon.attackRate)
            return;

        attackTimer = 0f;
        activeWeapon.Attack(this);
    }
}