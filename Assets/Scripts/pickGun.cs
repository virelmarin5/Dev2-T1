using UnityEngine;

public class pickGun : MonoBehaviour
{
    [SerializeField] gunStatsHandler gun;

    public void OnTriggerEnter(Collider other)
    {
        IPickGun pic = other.GetComponent<IPickGun>();

        if (pic != null)
        {
            gun.ammoCur = gun.ammoMax;
            pic.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}