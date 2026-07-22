using UnityEngine;

// Same behavior as the original teacher script: aims a gun pivot at the
// player every frame it's visible, and fires a bullet once the cooldown
// and range checks in the base class pass.
public class enemyRanged : enemyAI_Base
{
    [Header("Ranged Weapon")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform shootPos;
    [Range(1, 10)][SerializeField] int gunRotateSpeed;

    protected override void onPlayerSpotted()
    {
        rotateGun();
    }

    void rotateGun()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        // Lerping from gunPivot's own current rotation (not transform's) so the
        // gun tracks smoothly frame to frame instead of snapping from the body's rotation.
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, rot, gunRotateSpeed * Time.deltaTime);
    }

    protected override void attack()
    {
        attackTimer = 0;
        if (bullet != null)
            Instantiate(bullet, shootPos.position, gunPivot.rotation);
    }
}