using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    private bool isPaused;

    // the pause toggle
    public void ResumeGame()
    {
        if (LevelManager.instance.backgroundAudioSource) LevelManager.instance.backgroundAudioSource.GetComponent<AudioHighPassFilter>().enabled = false;

        if (!LevelManager.instance.dialogueController.isFinished && !isPaused)
        {
            PauseGame();
            return;
        }

        optionsMenu.SetActive(false);
        gameObject.SetActive(false);
        isPaused = false;

        if (LevelManager.instance.dialogueController.isFinished)
        {
            LevelManager.instance.EnablePlayerInput();
            if (LevelManager.instance.CurrentScene.manager.GetType() == typeof(HubManager))
            {
                LevelManager.instance.GetPlayer().DisableAttacks();
            }
        }
        
        LevelManager.instance.EnablePlayerRotation();

        Time.timeScale = 1f;
    }


    public void PauseGame()
    {
        if (LevelManager.instance.backgroundAudioSource) LevelManager.instance.backgroundAudioSource.GetComponent<AudioHighPassFilter>().enabled = true;

        isPaused = true;
        gameObject.SetActive(true);
        LevelManager.instance.DisablePlayerInput();
        LevelManager.instance.DisablePlayerRotation();
        Time.timeScale = 0f;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        LevelManager.instance.ReturnToMainMenu();
    }

    public void QuitGame()
    {
        LevelManager.instance.Quit();
    }
}
