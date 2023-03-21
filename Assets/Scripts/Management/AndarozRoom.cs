using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndarozRoom : BossRoomManager
{
    public Animator forceFieldAnimator;

    [Header("Dialogue")]
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] enterScript;
    public DialogueController.DialoguePart[] stage2Script;
    public DialogueController.DialoguePart[] endScript;


    public override void ForceSetScene()
    {
        base.ForceSetScene();

        largeRoomCamera.enabled = true;

        LevelManager.instance.DisablePlayerInput();
        CameraShake.instance.restoreCamPosAfterShake = false;
        CameraShake.instance.isLargePanningRoom = true;

        StartCutscene();
    }

    public void StartCutscene()
    {
        if (enableDialogue)
        {
            StartCoroutine(StartCutsceneRoutine());
        }
        else
        {
            LevelManager.instance.CurrentScene.enemyContainer.GetComponent<Andaroz>().StartBossFight();
        }
    }

    public void EndCutscene()
    {
        if (enableDialogue)
        { 
            StartCoroutine(EndCutsceneRoutine());
        }
        else
        {
            LevelManager.instance.NextScene();
        }
    }

    IEnumerator StartCutsceneRoutine()
    {
        yield return new WaitForSeconds(.4f);
        if (forceFieldAnimator) forceFieldAnimator.Play("ForceFieldOn");
        LevelManager.instance.DisablePlayerInput();
        yield return new WaitForSeconds(.4f);


        // dialogue
        LevelManager.instance.dialogueController.StartDialogue(enterScript);
        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        yield return new WaitForSeconds(0.25f);

        LevelManager.instance.EnablePlayerInput();

        yield return new WaitForSeconds(0.5f);
        LevelManager.instance.CurrentScene.enemyContainer.GetComponent<Andaroz>().StartBossFight();
    }

    IEnumerator EndCutsceneRoutine()
    {
        yield return new WaitForSeconds(2f);
        LevelManager.instance.DisablePlayerInput();

        // dialogue
        LevelManager.instance.dialogueController.StartDialogue(endScript);
        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        yield return new WaitForSeconds(1f);

        GameData gd = LevelManager.instance.LoadGameData();
        gd.beatAndaroz = true;
        LevelManager.instance.Save(gd);

        LevelManager.instance.NextScene();
    }
}
