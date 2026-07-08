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
        attackTimer += Time.deltaTime;
    }

    public void equipWeapon(weaponStats newWeapon)
    {
        if (newWeapon == null || weaponHoldPos == null) return;

        // 1. Drop and destroy current weapon
        if (spawnedWeaponModel != null)
        {
            Vector3 dropPos = transform.position + transform.forward * 1.5f + transform.up * 0.5f;
            Instantiate(activeWeapon.weaponModel, dropPos, Quaternion.identity);
            Destroy(spawnedWeaponModel);
        }

        // 2. Assign and initialize data
        activeWeapon = newWeapon;
        if (newWeapon is gunStats gun) gun.InitializeAmmo();

        // 3. Spawn model directly to target with default local space properties
        spawnedWeaponModel = Instantiate(newWeapon.weaponModel, weaponHoldPos.transform, false);

        if (spawnedWeaponModel.TryGetComponent<Collider>(out var col)) col.enabled = false;

        // 4. Locate the barrel or hitpoint using FindDeepChild
        string targetName = (newWeapon is gunStats) ? "Muzzle" : "HitPoint";
        gunBarrel = FindDeepChild(spawnedWeaponModel.transform, targetName);
    }

    // A reusable helper to find nested children cleanly without allocations
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