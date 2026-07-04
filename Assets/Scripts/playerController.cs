using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickGun
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] gunStatsHandler currGun;
    [SerializeField] GameObject gunModel;

    int jumpCount;
    int HPOriginal;

    Vector3 moveDir;
    Vector3 playerVel;
     

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOriginal = HP;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0;
        }
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        int  currSpeed = Input.GetButton("Sprint") ? speed * sprintMod : speed;
        controller.Move(moveDir * currSpeed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
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
        HP -= amount;

        if (HP <= 0)
        {
            // dead
        }
    }

    public void getGunStats(gunStatsHandler gun)
    {
        // Replace the current gun with the newly picked up one
        currGun = gun;
        changeGun();
    }

    void changeGun()
    {
        if (gunModel == null) return;

        // 1. Destroy whatever gun the player is currently holding
        foreach (Transform child in gunModel.transform)
        {
            Destroy(child.gameObject);
        }

        // 2. If we just dropped/cleared the weapon slot, stop here
        if (currGun == null || currGun.gunModel == null) return;

        // 3. Spawn the entire modular weapon prefab as a child of our gun holder
        GameObject newWeapon = Instantiate(currGun.gunModel, gunModel.transform);

        // 4. Reset its transform so it snaps perfectly to the player's hands
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.transform.localScale = Vector3.one;

        // 5.Turn off colliders and trigger scripts on the held version
        // So the player doesn't accidentally trigger a pickup on the gun they are holding
        if (newWeapon.TryGetComponent<Collider>(out Collider col)) col.enabled = false;
        if (newWeapon.TryGetComponent<pickGun>(out pickGun script)) script.enabled = false;

        // Also disable colliders hidden inside children if there are any
        foreach (Collider childCol in newWeapon.GetComponentsInChildren<Collider>())
        {
            childCol.enabled = false;
        }
    }
}