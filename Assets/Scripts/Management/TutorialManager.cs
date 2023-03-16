using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SceneManager
{
    [Header("Dialogue")]
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] enterScript;

    private bool firstTime = true;

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        if (enterScript.Length > 0 && firstTime)
        {
            firstTime = false;
            LevelManager.instance.DisablePlayerInput();
            StartCoroutine(StartDialogue());
        }
    }

    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.dialogueController.StartDialogue(enterScript);

        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        LevelManager.instance.EnablePlayerInput();
    }
}

