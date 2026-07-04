using UnityEngine;

public class gunController : MonoBehaviour
{
    [SerializeField] Transform barrel;
    [SerializeField] Transform bullet;
    [SerializeField] float shootRate;

    float shootTimer;
    
    
    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, barrel.position, barrel.rotation);
    }
}
