using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickGun
{
    [SerializeField] CharacterController controller;

    [Header("Movement Settings")]
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] gunStatsHandler currGun;
    [SerializeField] GameObject gunHoldPos;

    float shootTimer;

    int jumpCount;

    Vector3 moveDir;
    Vector3 playerVel;

    void Update()
    {
        movement();
        sprint();
        jump();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right +
                  Input.GetAxis("Vertical") * transform.forward;

        controller.Move(moveDir * speed * Time.deltaTime);

        // Applies gravity to the CharacterController.
        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        shootTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && currGun != null && shootTimer > currGun.shootRate)
            shoot();

        if(currGun != null)
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * currGun.shootDist, Color.red);
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    public void takeDamage(int amount)
    {
        // Player does not lose HP in this version.
        // Getting hit increases stress/BPM instead.
        if (heartbeatManager.instance != null)
        {
            heartbeatManager.instance.playerDamaged();
        }

        Debug.Log("Player was hit. Stress increased.");
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, currGun.shootDist))
        {
            IDamage dmg = hit.transform.GetComponent<IDamage>();
            if (dmg != null)
                dmg.takeDamage(currGun.shootDamage);
        }
    }

    public void gunStatsHandler(gunStatsHandler gun)
    {
        // Replace the current gun with the newly picked up one
        currGun = gun;
        changeGun();
    }

    void changeGun()
    {
        if (gunHoldPos == null) return;

        // 1. Destroy whatever gun the player is currently holding
        foreach (Transform child in gunHoldPos.transform)
        {
            Destroy(child.gameObject);
        }

        // 2. If we just dropped/cleared the weapon slot, stop here
        if (currGun == null || currGun.gunModel == null) return;

        // 3. Spawn the entire modular weapon prefab as a child of our gun holder
        GameObject newWeapon = Instantiate(currGun.gunModel, gunHoldPos.transform);

        // 4. Reset its transform so it snaps perfectly to the player's hands
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = Vector3.one;

        // 5.Turn off colliders and trigger scripts on the held version
        // So the player doesn't accidentally trigger a pickup on the gun they are holding
        if (newWeapon.TryGetComponent<Collider>(out Collider col)) col.enabled = false;
        if (newWeapon.TryGetComponent<pickGun>(out pickGun script)) script.enabled = false;
    }
}
