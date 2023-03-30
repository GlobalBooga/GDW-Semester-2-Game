using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneManager : SceneManager
{
    [Header("Dialogue")]
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] introScript;

    public AudioSource audioSource;
    public AudioClip mainTheme;
    private void Start()
    {
        LevelManager.instance.screenOverlayAnimator.Play(LevelManager.TELEPORT_END);
        audioSource.clip = mainTheme;
        audioSource.Play();
    }

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

        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.dialogueController.StartDialogue(introScript);
        while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        LevelManager.instance.EnablePlayerInput();
    }
}
