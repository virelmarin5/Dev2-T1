using UnityEngine;

public class playerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float maxDistance = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            weaponManager.instance.attack();
        }

        // 3. Handle Raycast Interaction
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Attempt to grab the pickWeapon component from the target directly
                if (hitObject.TryGetComponent<pickWeapon>(out var weaponPickup))
                {
                    if (TryGetComponent<IPickWeapon>(out var picker))
                    {
                        weaponPickup.interact(picker);
                    }
                }
            }
        }
    }
}