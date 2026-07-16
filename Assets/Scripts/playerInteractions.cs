using UnityEngine;

public class playerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    //[SerializeField] private LayerMask interactableLayer;
    [Range(1f, 5)][SerializeField] private float maxDistance = 2f;

    [Header("Audio")]
    [SerializeField] AudioClip equip;
    [Range(0, 1)][SerializeField] float equipVol;
    [SerializeField] AudioClip throwSFX;
    [Range(0, 1)][SerializeField] float throwSFXVol;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (gameManager.instance != null && gameManager.instance.isPaused) return;

            weaponManager.instance.attack();
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            GameObject hitObject = hit.transform.gameObject;

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Attempt to grab the pickWeapon component from the target directly
                if (hitObject.TryGetComponent<pickWeapon>(out var weaponPickup))
                {
                    // Apply outline
                    if (TryGetComponent<IPickWeapon>(out var picker))
                    {
                        audioManager.instance.playSFX(equip, equipVol);
                        weaponPickup.interact(picker);
                    }
                }
            }
        }
    }
}


/*using UnityEngine;
using System.Collections.Generic;

public class playerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Range(1f, 5)][SerializeField] private float maxDistance = 2f;

    [Header("Outline Settings")]
    [SerializeField] private Material outlineMaterial;

    [Header("Audio")]
    [SerializeField] AudioClip equip;
    [Range(0, 1)][SerializeField] float equipVol;
    [SerializeField] AudioClip throwSFX;
    [Range(0, 1)][SerializeField] float throwSFXVol;

    // Keeps track of the weapon we are currently looking at
    private GameObject currentHoveredWeapon;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (gameManager.instance != null && gameManager.instance.isPaused) return;

            weaponManager.instance.attack();
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            GameObject hitObject = hit.transform.gameObject;

            // Check if the object we are looking at has the pickWeapon component
            if (hitObject.TryGetComponent<pickWeapon>(out var weaponPickup))
            {
                // If we just started looking at a NEW weapon pickup
                if (currentHoveredWeapon != hitObject)
                {
                    // Clear outline from the old one just in case
                    ClearCurrentOutline();

                    // Track the new weapon and turn its outline on
                    currentHoveredWeapon = hitObject;
                    ToggleOutline(currentHoveredWeapon, true);
                }

                // Handle picking it up
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (TryGetComponent<IPickWeapon>(out var picker))
                    {
                        audioManager.instance.playSFX(equip, equipVol);
                        
                        // Clear the outline tracking right before it gets picked up/destroyed
                        ClearCurrentOutline(); 
                        
                        weaponPickup.interact(picker);
                    }
                }
            }
            else
            {
                // We hit something, but it's not a weapon pickup
                ClearCurrentOutline();
            }
        }
        else
        {
            // Raycast hit nothing at all
            ClearCurrentOutline();
        }
    }

    private void ClearCurrentOutline()
    {
        if (currentHoveredWeapon != null)
        {
            ToggleOutline(currentHoveredWeapon, false);
            currentHoveredWeapon = null;
        }
    }

    private void ToggleOutline(GameObject target, bool enable)
    {
        if (target == null || outlineMaterial == null) return;

        MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            List<Material> materialsList = new List<Material>(renderer.sharedMaterials);

            if (enable)
            {
                if (!materialsList.Contains(outlineMaterial))
                {
                    materialsList.Add(outlineMaterial);
                    renderer.materials = materialsList.ToArray();
                }
            }
            else
            {
                if (materialsList.Contains(outlineMaterial))
                {
                    materialsList.Remove(outlineMaterial);
                    renderer.materials = materialsList.ToArray();
                }
            }
        }
    }
}*/