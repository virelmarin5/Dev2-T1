using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        audioManager.instance.playButtonClick();
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        audioManager.instance.playButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void sound()
    {
        audioManager.instance.playButtonClick();
        gameManager.instance.openSoundMenu();
    }

    public void soundBack()
    {
        audioManager.instance.playButtonClick();
        gameManager.instance.openPauseMenu();
    }

    public void quit()
    {
        audioManager.instance.playButtonClick();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}