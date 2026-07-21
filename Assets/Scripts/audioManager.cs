using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance { get; private set; }

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Global Sound Effects")]
    [SerializeField] private AudioClip nukeSFX;

    [Header("Music")]
    [SerializeField] private AudioClip roundTransitionMusic;
    [SerializeField] private float musicVolume = 0.6f;

    private AudioSource musicSource;

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

        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = false;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume * masterVolume;
    }
    public void playSFX(AudioClip clip, float localVolumeMod = 1f)
    {
        if (clip == null) return;
        
        float Volume = localVolumeMod * sfxVolume * masterVolume;
        audioSource.PlayOneShot(clip, Volume);
    }

    // Plays the global nuke sound.
    public void playNuke()
    {
        playSFX(nukeSFX);
    }

    // Plays the techno music during the delay between waves.
    public void playRoundTransitionMusic()
    {
        if (roundTransitionMusic == null)
            return;

        musicSource.Stop();

        musicSource.clip = roundTransitionMusic;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }

    // Stops any transition music.
    public void stopRoundTransitionMusic()
    {
        musicSource.Stop();
    }
}