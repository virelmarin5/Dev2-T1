using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance { get; private set; }

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

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
}