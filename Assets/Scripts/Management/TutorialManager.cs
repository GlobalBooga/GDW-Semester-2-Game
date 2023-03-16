using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SceneManager
{
    [Header("Dialogue")]
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] script;

    private bool firstTime = true;

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        if (script.Length > 0 && firstTime)
        {
            firstTime = false;
            LevelManager.instance.DisablePlayerInput();
            StartCoroutine(StartDialogue());
        }
        /*else
        {
            SetObjectives();
        }*/
    }

    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.dialogueController.StartDialogue(script);

        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        SetObjectives();

        //yield return new WaitForSeconds(0.5f);

        LevelManager.instance.EnablePlayerInput();
    }

    void SetObjectives()
    {
        LevelManager.instance.hintController.ShowHint();
    }
}

