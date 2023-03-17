using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SceneManager
{
    [Header("Dialogue")]
    public int roomNumber;
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] script;
    public DialogueController.DialoguePart[] secondTimeArountScript;

    private int lap = 0;

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        CameraShake.instance.restoreCamPosAfterShake = true;

        if (!enableDialogue) return;

        if (script.Length > 0 && lap == 0)
        {
            lap++;
            LevelManager.instance.DisablePlayerInput();
            StartCoroutine(StartDialogue());
        }
        else if (lap == 1 && secondTimeArountScript.Length > 0 && roomNumber == 1)
        {
            lap++;
            LevelManager.instance.DisablePlayerInput();
            StartCoroutine(StartSecondDialogue());
        }
    }

    IEnumerator StartSecondDialogue()
    {
        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.dialogueController.StartDialogue(secondTimeArountScript);

        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        yield return new WaitForSeconds(0.25f);

        LevelManager.instance.EnablePlayerInput();

    }

    IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.dialogueController.StartDialogue(script);

        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        LevelManager.instance.hintController.ShowHint();

        yield return new WaitForSeconds(0.25f);

        LevelManager.instance.EnablePlayerInput();

        if (roomNumber == 4)
        {
            yield return new WaitForSeconds(4);
            LevelManager.instance.hintController.ObjectiveComplete(ENEMIES_TUTORIAL);
        }
    }

    public void ExplosiveLabPart2Dialogue()
    {
            LevelManager.instance.DisablePlayerInput();
        StartCoroutine(ExplosiveLabPart2DialogueRoutine());
    }

    IEnumerator ExplosiveLabPart2DialogueRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.dialogueController.StartDialogue(secondTimeArountScript);

        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        yield return new WaitForSeconds(0.25f);

        LevelManager.instance.EnablePlayerInput();
    }
}
