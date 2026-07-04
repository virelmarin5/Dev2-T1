using UnityEngine;
using System;

public class HeartbeatManager : MonoBehaviour
{
    [Header("Heartbeat Settings")]

    [SerializeField]
    private float restingBPM = 60f;

    [SerializeField]
    private float maxBPM = 180f;

    [SerializeField]
    private float currentBPM;


    [Header("Stress Settings")]

    [SerializeField]
    private float currentStress = 0f;

    [SerializeField]
    private float maxStress = 100f;

    [SerializeField]
    private float stressDecayRate = 5f;


    [Header("Stress Values")]

    [SerializeField]
    private float damageStress = 25f;

    [SerializeField]
    private float shootingStress = 3f;

    [SerializeField]
    private float nearMissStress = 10f;

    [SerializeField]
    private float killStressReduction = 8f;

    [SerializeField]
    private float waveCompleteStressReduction = 25f;


    // Other scripts can subscribe to heartbeat updates.
    public event Action<float, float> OnHeartbeatChanged;
    public float GetCurrentBPM()
    {
        return currentBPM;
    }

    public float GetStressPercent()
    {
        return currentStress / maxStress;
    }

    public float GetCurrentStress()
    {
        return currentStress;
    }
}