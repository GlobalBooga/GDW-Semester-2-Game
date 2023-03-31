using System.Collections;
using UnityEngine;
using static ContractSelector;

public class HubManager : SceneManager
{
    [Header("Contracts")]
    public GameObject[] contracts;

    [Header("Dialogue")]
    public bool enableDialogue = false;
    public DialogueController.DialoguePart[] introScript;
    public DialogueController.DialoguePart[] andarozContractScript;
    public DialogueController.DialoguePart[] trainContractScript;
    public DialogueController.DialoguePart[] timeLordContractScript;

    [Header("Music")]
    public AudioSource audioSource;
    public float audioInSpeed = 1f;

    [Header("Weapons")]
    public GameObject AdanasDualies;
    public GameObject EnergyDualies;
    public GameObject Bow;
    //public GameObject AndarozGun;

    private GameData gameData;

    private const int MIN_ANDAROZ_LEVEL_INDEX = 4;
    private const int MAX_ANDAROZ_LEVEL_INDEX = 6;
    
    private const int MIN_VALKYRIE_LEVEL_INDEX = 7;
    private const int MAX_VALKYRIE_LEVEL_INDEX = 9;

    private const int MIN_TRAIN_LEVEL_INDEX = 10;
    private const int MAX_TRAIN_LEVEL_INDEX = 12;

    private void Awake()
    {
        gameData = LevelManager.instance.LoadGameData();
    }

    private void Start()
    {
        if (gameData.weaponID == 1) AdanasDualies.SetActive(false);
        else if (gameData.weaponID == 2) EnergyDualies.SetActive(false);
        else if (gameData.weaponID == 3) Bow.SetActive(false);
        //else if (gameData.weaponID == 4) AndarozGun.SetActive(false);

        //if (gameData.weaponID != 4 && !gameData.foundAndarozGun) AndarozGun.SetActive(false);

        LevelManager.instance.screenOverlayAnimator.Play(LevelManager.TELEPORT_END);

        foreach (var item in contracts)
        {
            ContractSelector cs = item.GetComponent<ContractSelector>();
            if (gameData.beatAndaroz && cs.boss == Bosses.Andaroz) item.SetActive(false);
            if (gameData.beatGluttony && cs.boss == Bosses.Gluttony) item.SetActive(false);
            if (gameData.beatAndaroz && gameData.beatGluttony && cs.boss == Bosses.TimeLord) item.SetActive(true);
        }
    }

    public override void ForceSetScene()
    {
        base.ForceSetScene();

        if (!enableDialogue)
        {
            LevelManager.instance.GetPlayer().DisableAttacks();
            return;
        }

        StartCoroutine(MusicStart());

        if (LevelManager.instance.hintController && gameData.firstTimeInHub == true) 
        {
            StartCoroutine(IntroDialogue());
        }
        else
        {
            LevelManager.instance.GetPlayer().DisableAttacks();
            StartCoroutine(MusicContinue());
        }
    }

    private IEnumerator IntroDialogue()
    {
        LevelManager.instance.DisablePlayerInput();

        LevelManager.instance.dialogueController.StartDialogue(introScript);
        while (!LevelManager.instance.dialogueController.isFinished) yield return null;
        
        LevelManager.instance.EnablePlayerInput();
        LevelManager.instance.GetPlayer().DisableAttacks();
        StartCoroutine(MusicContinue());

        yield return new WaitForSeconds(0.5f);
        LevelManager.instance.hintController.ShowHint();
        gameData.firstTimeInHub = false;
        LevelManager.instance.Save(gameData);
    }

    public void PickedAndarozContract()
    {
        LevelManager.instance.hintController.ObjectiveComplete(PICK_A_CONTRACT_OBJECTIVE);
        StartCoroutine(StartLevel(0));
    }

    public void PickedTrainContract()
    {
        LevelManager.instance.hintController.ObjectiveComplete(PICK_A_CONTRACT_OBJECTIVE);
        StartCoroutine(StartLevel(1));
    }

    public void PickedTimeLordContract()
    {
        LevelManager.instance.hintController.ObjectiveComplete(PICK_A_CONTRACT_OBJECTIVE);
        StartCoroutine(LoopGame());
    }

    private IEnumerator StartLevel(int contract)
    {
        //yield return new WaitForSeconds(0.5f);

        LevelManager.instance.DisablePlayerInput();

        //if (contract == 0) LevelManager.instance.dialogueController.StartDialogue(andarozContractScript);
        //else if (contract == 1) LevelManager.instance.dialogueController.StartDialogue(trainContractScript);

        //while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        yield return new WaitForSeconds(2f);

        LevelManager.instance.screenOverlayAnimator.Play(LevelManager.TELEPORT_START);

        yield return new WaitForSeconds(0.25f);

        if (contract == 0)
        {
            if (!gameData.beatValkyrie)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(MIN_VALKYRIE_LEVEL_INDEX, MAX_VALKYRIE_LEVEL_INDEX + 1));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(MIN_ANDAROZ_LEVEL_INDEX, MAX_ANDAROZ_LEVEL_INDEX + 1));
            }
        }
        else if (contract == 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(Random.Range(MIN_TRAIN_LEVEL_INDEX, MAX_TRAIN_LEVEL_INDEX + 1));
        }

    }

    private IEnumerator LoopGame()
    {
        LevelManager.instance.DisablePlayerInput();
        yield return new WaitForSeconds(2f);

        //LevelManager.instance.dialogueController.StartDialogue(timeLordContractScript);
        //while (!LevelManager.instance.dialogueController.isFinished) yield return null;

        LevelManager.instance.screenOverlayAnimator.Play(LevelManager.TELEPORT_START);

        yield return new WaitForSeconds(0.25f);

        LevelManager.instance.ResetLevelProgress();
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    internal IEnumerator MusicStart()
    {
        if (audioSource)
        {
            AudioLowPassFilter lpf = audioSource.GetComponent<AudioLowPassFilter>();
            if (lpf)
            {
                while (lpf.cutoffFrequency < 1000f)
                {
                    lpf.cutoffFrequency += audioInSpeed;
                    yield return null;
                }
            }
        }
    }

    internal IEnumerator MusicContinue()
    {
        if (audioSource)
        {
            AudioLowPassFilter lpf = audioSource.GetComponent<AudioLowPassFilter>();
            if (lpf)
            {
                while (lpf.cutoffFrequency < 10000f)
                {
                    lpf.cutoffFrequency += audioInSpeed * 15f;
                    yield return null;
                }
            }
            lpf.enabled = false;
        }
    }
}
