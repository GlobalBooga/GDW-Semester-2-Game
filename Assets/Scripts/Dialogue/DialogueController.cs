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
        public string name;
        public bool rightSide;
        public Sprite speakerImage;
        public string[] sentences;
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
    const int DIALOGUE_INDEX = 2;
    const int LEFTSPEAKERIMAGE = 6;
    const int RIGHTSPEAKERIMAGE = 4;

    private void Awake()
    {
        DialogueAnimator = GetComponent<Animator>();
        DialogueText = transform.GetChild(DIALOGUE_INDEX).GetChild(0).GetComponent<Text>();
        NameText = transform.GetChild(NAME_INDEX).GetChild(0).GetComponent<Text>();
        RightSpeakerImage = transform.GetChild(RIGHTSPEAKERIMAGE).GetComponent<Image>();
        LeftSpeakerImage = transform.GetChild(LEFTSPEAKERIMAGE).GetComponent<Image>();
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

            if (currentPart.speakerImage && currentPart.rightSide)
            {
                LeftSpeakerImage.gameObject.SetActive(true);
                RightSpeakerImage.gameObject.SetActive(true);

                LeftSpeakerImage.color = Color.white;
                LeftSpeakerImage.sprite = currentPart.speakerImage;
                RightSpeakerImage.color = notSpeaking;
            }
            else if (currentPart.speakerImage && !currentPart.rightSide)
            {
                LeftSpeakerImage.gameObject.SetActive(true);
                RightSpeakerImage.gameObject.SetActive(true);

                RightSpeakerImage.color = Color.white;
                RightSpeakerImage.sprite = currentPart.speakerImage;
                LeftSpeakerImage.color = notSpeaking;
            }
            else
            {
                LeftSpeakerImage.gameObject.SetActive(false);
                RightSpeakerImage.gameObject.SetActive(false);
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
        if (script.Length == 0) return;
        

        isFinished = false;
        isEnabled = true;

        foreach (var item in script)
        {
            this.script.Enqueue(item);
        }
        DialogueAnimator.Play(ENTER);

        GetSpeakerPortraits();

        NextPart();
    }

    private void GetSpeakerPortraits()
    {
        if (script.Count == 0)
        {
            return;
        }

        Sprite speaker1 = null;
        Sprite speaker2 = null;

        foreach (var part in script)
        {
            if (part.speakerImage)
            {
                if (!speaker1)
                {
                    speaker1 = part.speakerImage;
                    if (part.rightSide) LeftSpeakerImage.sprite = speaker1;
                    else RightSpeakerImage.sprite = speaker1;
                }
                else if (!speaker2)
                {
                    speaker2 = part.speakerImage;
                    if (part.rightSide) LeftSpeakerImage.sprite = speaker2;
                    else RightSpeakerImage.sprite = speaker2;
                }
            }
        }
    }
}
