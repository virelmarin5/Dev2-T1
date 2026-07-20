using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance { get; private set; }

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio")]
    [SerializeField] public AudioClip jump;
    [Range(0, 1)][SerializeField] public float jumpVol;
    [SerializeField] public AudioClip hurt;
    [Range(0, 1)][SerializeField] public float hurtVol;
    [SerializeField] public AudioClip steps;
    [Range(0, 1)][SerializeField] public float stepsVol;
    [SerializeField] public AudioClip enemyHit;
    [Range(0, 1)][SerializeField] public float enemyHitVol;
    [SerializeField] public AudioClip wallHit;
    [Range(0, 1)][SerializeField] public float wallHitVol;
    [SerializeField] public AudioClip equip;
    [Range(0, 1)][SerializeField] public float equipVol;
    [SerializeField] public AudioClip bulletRicochet;
    [Range(0, 1)][SerializeField] public float bulletRicochetVol;

    private AudioSource audioSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (!TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 0f;
    }
    public void playSFX(AudioClip clip, float localVolumeMod = 1f)
    {
        if (clip == null) return;

        float Volume = localVolumeMod * sfxVolume * masterVolume;
        audioSource.PlayOneShot(clip, Volume);
    }

    public void playSpatialSFX(AudioClip clip, Vector3 position, float localVolumeMod = 1f, float minDistance = 1f, float maxDistance = 50f)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = localVolumeMod * sfxVolume * masterVolume;

        // 3D Spatial Audio setup
        source.spatialBlend = 1f; // Full 3D spatialization
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.Play();

        Destroy(tempGO, clip.length);
    }
}