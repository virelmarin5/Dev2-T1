using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Gun", order = 2)]

public class gunStats : weaponStats
{
    public enum GunType {AR, Pistol, Shotgun}

    [Header("Gun Settings")]
    public GunType gunType;

    [Header("Projectile")]
    [SerializeField] public Transform bullet;

    [Header("Ammo")]
    [Range(2, 50)] public int magazineSize;
    [Range(1, 10)] public int totalMagazines = 3;
    public int ammoCur;
    public int ammoMax;

    [Range(1, 20)] public int pelletCount;
    [Range(0f, 20f)] public float spreadAngle;

    [Header("Audio")]
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;

    public void InitializeAmmo()
    {
        ammoCur = magazineSize;
        ammoMax = magazineSize * totalMagazines + 7;
    }

    public override void Attack(weaponManager manager)
    {
        Transform gunBarrel = manager.gunBarrel;
        if (gunBarrel == null) return;

        ammoCur--;

        if (shootSound != null)
            audioManager.instance.playSFX(shootSound, shootSoundVol);
        
        int shotsToFire = (gunType == GunType.Shotgun) ? pelletCount : 1;

        if (bullet != null)
        {
            for (int i = 0; i < shotsToFire; i++)
            {
                // Calculate random deviation within the spread angle cone
                float randomSpreadX = Random.Range(-spreadAngle, spreadAngle);
                float randomSpreadY = Random.Range(-spreadAngle, spreadAngle);

                // Combine the barrel's base rotation with our random offset angles
                Quaternion spreadRotation = gunBarrel.rotation * Quaternion.Euler(randomSpreadX, randomSpreadY, 0);

                // Spawn the bullet projectile flying out into its offset trajectory
                MonoBehaviour.Instantiate(bullet, gunBarrel.position, spreadRotation);
            }
        }
    }
}
