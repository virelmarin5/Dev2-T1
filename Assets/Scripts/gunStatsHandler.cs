using UnityEngine;

[CreateAssetMenu]

public class gunStatsHandler : ScriptableObject
{
    public GameObject gunModel;

    [SerializeField] public Transform bullet;

    [Range(1, 10)] public int shootDamage;
    [Range(5, 1000)] public int shootDist;
    [Range(0.1f, 2)] public float shootRate;

    public int ammoCur;
    [Range(2, 50)] public int ammoMax;

    public AudioClip magDumpSound;
    [Range(0, 1)] public float magDumpSoundVol;
    public AudioClip shootSound;
    [Range(0, 1)] public float shootSoundVol;
    public AudioClip[] reloadSound;
    [Range(0, 1)] public float reloadSoundVol;
}
