using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    
    //[SerializeField] private Button muteButton;
    //[SerializeField] private Sprite soundOnSprite;
    //[SerializeField] private Sprite soundOffSprite;

    void Start()
    {
        if (audioManager.instance != null)
        {
            masterSlider.value = audioManager.instance.masterVolume;
            sfxSlider.value = audioManager.instance.sfxVolume;

            masterSlider.onValueChanged.AddListener(audioManager.instance.setMasterVolume);
            sfxSlider.onValueChanged.AddListener(audioManager.instance.setSFXVolume);
        }
    }
}