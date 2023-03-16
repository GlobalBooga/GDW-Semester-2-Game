using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : SceneManager
{
    [Header("Dialogue")]
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] introScript;
    public DialogueController.DialoguePart[] contractScript;

    public override void ForceSetScene()
    {
        base.ForceSetScene();
        
        if (!enableDialogue) return;

        // dialogue
        if (LevelManager.instance.hintController)
            StartCoroutine(IntroDialogue());
    }

    private IEnumerator IntroDialogue()
    {
        LevelManager.instance.DisablePlayerInput();

        LevelManager.instance.dialogueController.StartDialogue(introScript);
        while (!LevelManager.instance.dialogueController.isFinished) yield return null;
        
        LevelManager.instance.EnablePlayerInput();

        yield return new WaitForSeconds(0.5f);
        LevelManager.instance.hintController.ShowHint();
    }

    public void CompletePickAContract()
    {
        LevelManager.instance.hintController.ObjectiveComplete(PICK_A_CONTRACT_OBJECTIVE);
    }
}
