using UnityEngine;

public class playerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float maxDistance = 3f;

    private void Update()
    {
        // 1. Handle Weapon Attacking
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            weaponManager.instance.attack();
        }

        // 2. Handle Raycast Interaction
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * maxDistance, Color.green);

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // DEBUG: Prints what your crosshair is currently hovering over
            Debug.Log($"[Looking At] Object: {hitObject.name} | Layer: {LayerMask.LayerToName(hitObject.layer)}");

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Attempt to grab the pickWeapon component from the target directly
                if (hitObject.TryGetComponent<pickWeapon>(out var weaponPickup))
                {
                    if (TryGetComponent<IPickWeapon>(out var picker))
                    {
                        Debug.Log($"[Interaction] Successfully interacting with weapon pickup: {hitObject.name}");
                        weaponPickup.interact(picker);
                    }
                    else
                    {
                        Debug.LogWarning($"[Interaction Failed] Player component 'IPickWeapon' is missing on this GameObject!");
                    }
                }
                else
                {
                    Debug.Log($"[Interaction Failed] {hitObject.name} does not have a 'pickWeapon' component attached.");
                }
            }
        }
    }
}