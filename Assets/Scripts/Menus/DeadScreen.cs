using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadScreen : MonoBehaviour
{
    public void RestartLevel()
    {
        LevelManager.instance.EnablePlayerInput();
        LevelManager.instance.EnablePlayerRotation();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }


    public void ReturnToMainMenu()
    {
        LevelManager.instance.ReturnToMainMenu();
    }

    public void ReturnToHub()
    {
        LevelManager.instance.EnablePlayerInput();
        LevelManager.instance.EnablePlayerRotation();
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    
}
