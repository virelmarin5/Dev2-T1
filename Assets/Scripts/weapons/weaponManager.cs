using UnityEngine;

public class weaponManager : MonoBehaviour
{
    public static weaponManager instance { get; private set; }

    [Header("Weapon")]
    public weaponStats activeWeapon;
    private GameObject spawnedWeaponModel;


    private GameObject weaponHoldPos;
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

        if (weaponHoldPos == null)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Transform foundTransform = FindDeepChild(player.transform, "Weapon Hold Position");
            if (foundTransform != null)
            {
                weaponHoldPos = foundTransform.gameObject;
            }
        }
        
        if (weaponHoldPos == null)
        {
            Debug.LogError("Weapon Manager: Failed to automatically locate 'Weapon Hold Position' under the Player structure.");
        }
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
        // Spawn model
        spawnedWeaponModel = Instantiate(newWeapon.weaponModel, weaponHoldPos.transform, false);

        spawnedWeaponModel.transform.localPosition = Vector3.zero;
        spawnedWeaponModel.transform.localRotation = Quaternion.identity;

        if (spawnedWeaponModel.TryGetComponent<Collider>(out var col)) col.enabled = false;

        // Locate the barrel or hitpoint
        string targetName = (newWeapon is gunStats) ? "Muzzle" : "HitPoint";
        gunBarrel = FindDeepChild(spawnedWeaponModel.transform, targetName);
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