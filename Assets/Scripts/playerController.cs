using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour, IDamage, IPickWeapon
{
    [Header("Controller")]
    [SerializeField] CharacterController controller;

    [Header("Player Settings")]
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] float pushbackForce = 5f;
    [SerializeField] float pushbackFriction = 5f;

    [Header("Audio Settings")]    
    [SerializeField] AudioClip audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

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
    public void PushBack(Vector3 direction)
    {
        // CharacterController can be moved directly
        playerVel += direction * pushbackForce;
    }
    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0;
        }
        moveDir = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;
        int  currSpeed = Input.GetButton("Sprint") ? speed * sprintMod : speed;
        audioManager.instance.playSFX(audSteps, audStepsVol);
        controller.Move(moveDir * currSpeed * Time.unscaledDeltaTime);

        jump();

        controller.Move(playerVel * Time.unscaledDeltaTime);

        playerVel.x = Mathf.MoveTowards(playerVel.x, 0, pushbackFriction * Time.unscaledDeltaTime);
        playerVel.z = Mathf.MoveTowards(playerVel.z, 0, pushbackFriction * Time.unscaledDeltaTime);
        playerVel.y -= gravity * Time.unscaledDeltaTime;
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            audioManager.instance.playSFX(audJump, audJumpVol);
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    public void takeDamage(int amount)
    {
        audioManager.instance.playSFX(audHurt, audHurtVol);

        if (heartbeatManager.instance != null)
        {
            heartbeatManager.instance.playerDamaged();
        }
        
        //HP -= amount;

        //if (HP <= 0)
        //{
        //    // dead
        //}
    }

    public void weaponStats(weaponStats weapon)
    {
        if (weaponManager.instance != null)
        {
            weaponManager.instance.equipWeapon(weapon);
        }
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