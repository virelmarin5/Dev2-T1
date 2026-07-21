using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Gun")]

public class gunStats : weaponStats
{
    public enum GunType { Pistol, AR, Shotgun }

    [Header("Gun Settings")]
    public GunType gunType;

    [Header("Projectile")]
    [SerializeField] public Transform bullet;

    [Header("Spawn Position")]
    public Vector3 Position;
    public Vector3 Rotation;

    [Header("Ammo")]
    [Range(2, 6)] public int pelletCount;
    [Range(.2f, 3f)] public float spreadAngle;

    [Header("Audio")]
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;

    public override void Attack(weaponManager manager)
    {
        Transform gunBarrel = manager.gunBarrel;
        if (gunBarrel == null) return;

            audioManager.instance.playSFX(shootSound, shootSoundVol);

        int shotsToFire = (gunType == GunType.Shotgun) ? pelletCount : 1;
        spreadAngle = (gunType == GunType.Shotgun) ? spreadAngle : 0;

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
