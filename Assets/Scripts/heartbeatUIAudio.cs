using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeartbeatUIAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HeartbeatManager heartbeatManager;

    [Header("UI")]
    [SerializeField] private TMP_Text bpmText;
    [SerializeField] private Image stressFillImage;

    [Header("Audio")]
    [SerializeField] private AudioSource heartbeatAudioSource;

    [Header("Audio Settings")]
    [SerializeField] private float minVolume = 0.2f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.6f;

    private void Start()
    {
        if (heartbeatManager != null)
        {
            heartbeatManager.OnHeartbeatChanged += UpdateHeartbeatDisplay;
        }

        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.loop = true;
            heartbeatAudioSource.Play();
        }
    }

    private void UpdateHeartbeatDisplay(float bpm, float stressPercent)
    {
        // Updates the BPM text on the player's HUD.
        if (bpmText != null)
        {
            bpmText.text = "BPM: " + Mathf.RoundToInt(bpm);
        }

        // Updates the stress meter fill amount from 0 to 1.
        if (stressFillImage != null)
        {
            stressFillImage.fillAmount = stressPercent;
        }

        UpdateHeartbeatAudio(stressPercent);
    }

    private void UpdateHeartbeatAudio(float stressPercent)
    {
        if (heartbeatAudioSource == null)
        {
            return;
        }

        // Higher stress makes the heartbeat sound louder and faster.
        heartbeatAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, stressPercent);
        heartbeatAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, stressPercent);
    }

    private void OnDestroy()
    {
        // Prevents errors if this object is destroyed during scene changes.
        if (heartbeatManager != null)
        {
            heartbeatManager.OnHeartbeatChanged -= UpdateHeartbeatDisplay;
        }
    }
}
