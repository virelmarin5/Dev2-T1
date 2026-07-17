using UnityEngine;
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

            if (hitObject.TryGetComponent<pickWeapon>(out var weaponPickup))
            {
                if (currentHoveredWeapon != hitObject)
                {
                    ClearCurrentOutline();

                    currentHoveredWeapon = hitObject;
                    ToggleOutline(currentHoveredWeapon, true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (TryGetComponent<IPickWeapon>(out var picker))
                    {
                        audioManager.instance.playSFX(equip, equipVol);
                        
                        ClearCurrentOutline(); 
                        
                        weaponPickup.interact(picker);
                    }
                }
            }
            else
            {
                ClearCurrentOutline();
            }
        }
        else
        {
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
}