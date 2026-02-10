using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInputHelper : MonoBehaviour
{
    public GameObject MouseAndKeyboardSprites;
    public GameObject MouseAndKeyboardText;
    public GameObject ControllerSprites;
    public GameObject ControllerText;

    bool showingMouseAndKeyboardControls = true;

    private void Update()
    {
        if (!LevelManager.instance.cursor.isActiveAndEnabled && showingMouseAndKeyboardControls)
        {
            showingMouseAndKeyboardControls = false;
            ShowControls();
        }    
        else if (LevelManager.instance.cursor.isActiveAndEnabled && !showingMouseAndKeyboardControls)
        {
            showingMouseAndKeyboardControls = true;
            ShowControls();
        }
    }

    private void ShowControls()
    {
        MouseAndKeyboardSprites.SetActive(showingMouseAndKeyboardControls);
        MouseAndKeyboardText.SetActive(showingMouseAndKeyboardControls);

        ControllerSprites.SetActive(!showingMouseAndKeyboardControls);
        ControllerText.SetActive(!showingMouseAndKeyboardControls);
    }
}
