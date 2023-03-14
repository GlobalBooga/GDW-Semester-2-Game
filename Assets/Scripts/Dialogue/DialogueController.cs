using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogueController : MonoBehaviour
{
    [Serializable]
    public struct DialoguePart
    {
        public bool leftSide;
        public string name;
        public string[] sentences;
        public Sprite speakerImage;
    }

    // animations
    const string ENTER = "DialogueEntry";
    const string EXIT = "DialogueExit";


    private Text DialogueText;
    private Text NameText;
    private Image LeftSpeakerImage;
    private Image RightSpeakerImage;
    private Queue<DialoguePart> script = new();
    private int sentenceIndex = 0;
    public float DialogueSpeed = 0.08f;
    private Animator DialogueAnimator;
    private bool isWriting;
    
    public bool isEnabled { get; private set; }
    public bool isFinished { get; private set; }

    public Color notSpeaking;

    DialoguePart currentPart;

    const int NAME_INDEX = 0;
    const int DIALOGUE_INDEX = 1;

    private void Awake()
    {
        DialogueAnimator = GetComponent<Animator>();
        DialogueText = transform.GetChild(DIALOGUE_INDEX).GetChild(0).GetComponent<Text>();
        NameText = transform.GetChild(NAME_INDEX).GetChild(0).GetComponent<Text>();
    }

    public void NextSentence()
    {
        if (!isEnabled)
        {
            return;
        }


        if (isWriting)
        {
            isWriting = false;
            StopCoroutine(nameof(WriteSentence));
            DialogueText.text = currentPart.sentences[sentenceIndex];
            sentenceIndex++; 
            return;
        }

        if (sentenceIndex < currentPart.sentences.Length)
        {
            DialogueText.text = "";
            StartCoroutine(nameof(WriteSentence));
        }
        else
        {
            NextPart();
        }
    }

    void NextPart()
    {
        sentenceIndex = 0;

        if (script.Count == 0)
        {
            DialogueText.text = "";
            DialogueAnimator.Play(EXIT);
            sentenceIndex = 0;
            isFinished = true;
            isEnabled = false;
            return;
        }

        if (script.Peek().sentences != null)
        {
            currentPart = script.Dequeue();

            NameText.text = currentPart.name;

            if (currentPart.speakerImage && currentPart.leftSide)
            {
                LeftSpeakerImage.color = Color.white;
                LeftSpeakerImage.sprite = currentPart.speakerImage;
                RightSpeakerImage.color = notSpeaking;
            }
            else if (currentPart.speakerImage && !currentPart.leftSide)
            {
                RightSpeakerImage.color = Color.white;
                RightSpeakerImage.sprite = currentPart.speakerImage;
                LeftSpeakerImage.color = notSpeaking;
            }

            NextSentence();
        }
    }

    IEnumerator WriteSentence()
    {
        isWriting = true;

        foreach(char Character in currentPart.sentences[sentenceIndex].ToCharArray())
        {
            DialogueText.text += Character;
            yield return new WaitForSeconds(DialogueSpeed);
        }
        sentenceIndex++; 
        isWriting = false;
    }

    public void StartDialogue(DialoguePart[] script)
    {
        isFinished = false;
        isEnabled = true;

        foreach (var item in script)
        {
            this.script.Enqueue(item);
        }
        DialogueAnimator.Play(ENTER);

        NextPart();
    }
}
