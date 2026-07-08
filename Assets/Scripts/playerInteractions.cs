using UnityEngine;

public class playerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask interactableLayer;
    [Range(1f, 5)][SerializeField] private float maxDistance = 2f;

    [Header("Audio")]
    [SerializeField] AudioClip equip;
    [Range(0, 1)][SerializeField] float equipVol;
    [SerializeField] AudioClip throwSFX;
    [Range(0, 1)][SerializeField] float throwSFXVol;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            weaponManager.instance.attack();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            audioManager.instance.playSFX(throwSFX, throwSFXVol);
            weaponManager.instance.throwCurrentWeapon();
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Attempt to grab the pickWeapon component from the target directly
                if (hitObject.TryGetComponent<pickWeapon>(out var weaponPickup))
                    if (TryGetComponent<IPickWeapon>(out var picker))
                    {
                        audioManager.instance.playSFX(equip, equipVol);
                        weaponPickup.interact(picker);
                    }
            }
        }
    }
}