using UnityEngine;
using UnityEngine.SceneManagement;

public class titleMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject titleMenuPanel;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioManager.instance.playTitleScreenSound();
    }

    public void continueGame()
    {
        audioManager.instance.playButtonClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void quitGame()
    {
        audioManager.instance.playButtonClick();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}