using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] timeManager timeManager;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    // Pause the game
    public void statePause()
    {
        isPaused = true;
        timeManager.pauseTime();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Unpause the game
    public void stateUnpause()
    {
        isPaused = false;
        timeManager.unpauseTime();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    // Update the heart rate in UI only, moving it to just heartBeatManager
    public void updateHeartRate(int bpm)
    {
        // Update the heart rate in the UI (not implemented here)
    }

    // Handle the lose state
    public void stateLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
