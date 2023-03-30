using UnityEngine;

public class DeadScreen : MonoBehaviour
{
    public void RestartLevel()
    {
        LevelManager.instance.EnablePlayerInput();
        LevelManager.instance.EnablePlayerRotation();
        LevelManager.instance.RestartLevel();
    }

    public void ReturnToMainMenu()
    {
        LevelManager.instance.ReturnToMainMenu();
    }

    public void ReturnToHub()
    {
        LevelManager.instance.EnablePlayerInput();
        LevelManager.instance.EnablePlayerRotation();
        LevelManager.instance.ReturnToHub();
    }
    
}
