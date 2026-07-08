using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Gun", order = 2)]

public class gunStats : weaponStats
{    
    [Header("Projectile")]
    [SerializeField] public Transform bullet;

    [Header("Ammo")]
    [Range(2, 50)] public int magazineSize;
    [Range(1, 10)] public int totalMagazines = 3;
    public int ammoCur;
    public int ammoMax;

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

        // Spawn bullet projectile
        if (bullet != null)
            MonoBehaviour.Instantiate(bullet, gunBarrel.position, gunBarrel.rotation);

        if (shootSound != null)
            audioManager.instance.playSFX(shootSound, shootSoundVol);
    }
}
