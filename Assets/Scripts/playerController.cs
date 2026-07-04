using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

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
        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0;
        }
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    //Added by Devin C, dies if heart rate and stress is too high
    public void takeDamage(int amount)
    {
        Debug.Log("Player was hit. Stress increased instead of HP damage.");

        if (heartbeatManager.instance != null)
        {
            heartbeatManager.instance.playerDamaged();
        }
    }
}
