using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    [Header("Movement Settings")]
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

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
}
