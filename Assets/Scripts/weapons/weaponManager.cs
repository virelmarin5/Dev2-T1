using UnityEngine;

public class weaponManager : MonoBehaviour
{
    public static weaponManager instance { get; private set; }

    [Header("Weapon")]
    public weaponStats activeWeapon;

    [SerializeField] public GameObject weaponHoldPos;
    public Transform gunBarrel;
    private float attackTimer;

    // A hidden, reusable scene object that acts as the real-world muzzle
    private GameObject runtimeMuzzleMarker;

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
        if (newWeapon == null || weaponHoldPos == null)
            return;

        // Assign the new weapon data
        activeWeapon = newWeapon;

        if (newWeapon is gunStats gun)
        {
            gun.InitializeAmmo();
        }

        // --- FIXED MESH SWAP LOGIC ---
        // Dig into the children of the weapon prefab to find the actual visible 3D mesh components
        MeshFilter prefabMeshFilter = newWeapon.weaponModel.GetComponentInChildren<MeshFilter>(true);
        MeshRenderer prefabMeshRenderer = newWeapon.weaponModel.GetComponentInChildren<MeshRenderer>(true);

        if (prefabMeshFilter != null && prefabMeshRenderer != null)
        {
            weaponHoldPos.GetComponent<MeshFilter>().sharedMesh = prefabMeshFilter.sharedMesh;
            weaponHoldPos.GetComponent<MeshRenderer>().sharedMaterial = prefabMeshRenderer.sharedMaterial;
        }
        else
        {
            Debug.LogWarning("Could not find a MeshFilter or MeshRenderer on the weapon prefab or its children!");
        }

        // --- BARREL LOGIC ---
        // Locate the barrel or hitpoint transform dynamically from the prefab asset
        gunBarrel = null;
        Transform[] allChildren = newWeapon.weaponModel.GetComponentsInChildren<Transform>(true);
        
        string targetTagName = (newWeapon is gunStats) ? "Muzzle" : "HitPoint";
        Transform prefabMuzzle = null;

        foreach (Transform child in allChildren)
        {
            if (child.name == targetTagName)
            {
                Debug.Log("Gun Barrel Found in Prefab Asset!");
                prefabMuzzle = child;
                break;
            }
        }

        // Map the prefab's offset onto a real scene object attached to your weaponHoldPos
        if (prefabMuzzle != null)
        {
            if (runtimeMuzzleMarker == null)
            {
                runtimeMuzzleMarker = new GameObject("Runtime_Muzzle_Marker");
            }

            // Parent the marker to your moving hand transform
            runtimeMuzzleMarker.transform.SetParent(weaponHoldPos.transform);

            // Match the relative distance/rotation of the weapon's barrel
            runtimeMuzzleMarker.transform.localPosition = prefabMuzzle.localPosition;
            runtimeMuzzleMarker.transform.localRotation = prefabMuzzle.localRotation;
            runtimeMuzzleMarker.transform.localScale = prefabMuzzle.localScale;

            // Hand the active, moving transform to your shooting script
            gunBarrel = runtimeMuzzleMarker.transform;
        }
    }

    public void attack()
    {
        if (activeWeapon == null || attackTimer < activeWeapon.attackRate) 
            return;

        attackTimer = 0f;
        activeWeapon.Attack(this);
    }
}