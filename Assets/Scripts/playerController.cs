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
    [SerializeField] GameObject gunHoldPos;

    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    Transform gunBarrel;

    float shootTimer;

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

        shootTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && currGun != null && shootTimer > currGun.shootRate)
            shoot();

        if(currGun != null)
            Debug.DrawRay(gunBarrel.transform.position, gunBarrel.transform.forward * currGun.shootDist, Color.red);
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

    void shoot()
    {
        shootTimer = 0;

        currGun.ammoCur--;

        if (currGun.ammoCur <= 0)
        {
            audPlayer.PlayOneShot(currGun.magDumpSound, currGun.magDumpSoundVol);
            return;
        }

        Instantiate(currGun.bullet, gunBarrel.position, gunBarrel.rotation);

        audPlayer.PlayOneShot(currGun.shootSound, currGun.shootSoundVol);

        RaycastHit hit;
        if (Physics.Raycast(gunBarrel.transform.position, gunBarrel.transform.forward, out hit, currGun.shootDist))
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

        gunBarrel = newWeapon.transform.Find("Muzzle");

        // 5.Turn off colliders and trigger scripts on the held version
        // So the player doesn't accidentally trigger a pickup on the gun they are holding
        if (newWeapon.TryGetComponent<Collider>(out Collider col)) col.enabled = false;
        if (newWeapon.TryGetComponent<pickGun>(out pickGun script)) script.enabled = false;
    }

    public float getSpeedPercent()
    {
        // Returns a value between 0 and 1 representing how fast the player is moving relative to their max speed.
        Vector3 hor = new Vector3(moveDir.x,0, moveDir.z);
        float horPercent = Mathf.Clamp01(hor.magnitude);

        // If the player is in the air, we also consider their vertical speed relative to jump speed.
        float vertPercent = 0;
        if (!controller.isGrounded)
        {
            vertPercent = Mathf.Clamp01(Mathf.Abs(playerVel.y) / jumpSpeed);
        }

        // Return the greater of the two percentages to represent overall movement intensity.
        return Mathf.Max(horPercent, vertPercent);
    }

}