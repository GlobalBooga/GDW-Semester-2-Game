using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    private bool isPaused;

    public void ResumeGame()
    {
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
        }
        
        LevelManager.instance.EnablePlayerRotation();

        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
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
        Application.Quit();
    }
}
