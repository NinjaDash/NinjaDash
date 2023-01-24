using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class UIManager : MonoBehaviour
{
    #region singleton
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("UImanager Instance was made in Awake");
        }
    }
    #endregion

    #region coroutines
    Coroutine CO_startGame;
    Coroutine typingCoroutine;
    Coroutine CO_missionText;
    #endregion

    public RectTransform CanvasMain;
    public TextMeshProUGUI healthBar;
    public GameObject DoorIsCloseIcon;
    public GameObject DoorIsOpenIcon;
    public TextMeshProUGUI RemainingTargetText;
    public CharacterController2D myCC;
    public GameObject GameplayUI;
    public GameObject MainMenuUI;
    public GameObject GameOverUI;
    public GameObject PauseMenuUI;
    public GameObject SkipLevelBox;
    public GameObject NoInteractionPopUp;
    public TextMeshProUGUI NoInteractionPopUpText;
    public CanvasGroup fadeScreen;
    public LevelManager levelManager;

    [SerializeField] TextMeshProUGUI CurrentMissionText;
    [SerializeField] TextMeshProUGUI[] CurrentCoinCount;


    // public GameObject MainMenuLevel;

    [Header("KnifeButtons")]
    [SerializeField] Button KnifeThrowButton;
    [SerializeField] Button TeleportButton;

    [SerializeField] TextMeshProUGUI Knife_Timer;
    [SerializeField] Image KnifeButtonOverlay;

    [Header("UI Edit")]
    [SerializeField] List<UIPosHolder> uiPosHolderList = new List<UIPosHolder>();

    [Header("Dialogue System")]
    [SerializeField] GameObject DialogueGameObject;
    [SerializeField] Button NextDialogueButton;
    [SerializeField] TextMeshProUGUI DialogueBox;
    [SerializeField] Queue<String> Dialogue_Sentences = new Queue<string>();
    internal static string username;


    #region dialogue System
    internal void StartDialogue(Dialogue myDialogue)
    {
        DialogueGameObject.SetActive(true);
        Dialogue_Sentences.Clear();
        PlayerInputs.canControl = false;
        foreach (string Sentence in myDialogue.dialogue)
        {
            Dialogue_Sentences.Enqueue(Sentence);
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        NextDialogueButton.interactable = false;
        if (Dialogue_Sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = Dialogue_Sentences.Dequeue();
        //DialogueBox.text = sentence;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeSentences(DialogueBox, sentence));
    }

    IEnumerator TypeSentences(TextMeshProUGUI textBox, String sentence)
    {
        bool ignoreText = false;
        textBox.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if (letter == '<') ignoreText = true;
            else if (letter == '>') ignoreText = false;

            if (ignoreText)
            {
                textBox.text += letter;
            }
            else
            {
                textBox.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
        }
        NextDialogueButton.interactable = true;
        /* while (true)
         {
             textBox.text += ".";
             yield return new WaitForSeconds(0.5f);
             textBox.text= DialogueBox.text.Remove(DialogueBox.text.LastIndexOf('.'));
             yield return new WaitForSeconds(0.5f);
         }*/
    }


    private void EndDialogue()
    {
        DialogueGameObject.SetActive(false);
        PlayerInputs.canControl = true;
    }
    #endregion
    #region Knife Area
    public void knifeButtonToggle()
    {
        KnifeThrowButton.gameObject.SetActive(!KnifeThrowButton.gameObject.activeSelf);
        KnifeThrowButton.interactable = false;
        TeleportButton.gameObject.SetActive(!TeleportButton.gameObject.activeSelf);
    }
    public void ResetKnife()
    {
        KnifeThrowButton.gameObject.SetActive(true);
        KnifeThrowButton.interactable = true;
        StartCoroutine(KnifeCDTimer(0));
        TeleportButton.gameObject.SetActive(false);
    }
    public IEnumerator KnifeCDTimer(float value)
    {
        float initValue = value;
        while (value > 0)
        {
            Knife_Timer.text = ((int)value).ToString();
            value -= Time.deltaTime;
            if (KnifeThrowButton.gameObject.activeSelf)
            {
                KnifeButtonOverlay.fillAmount = value / initValue;
            }
            yield return new WaitForEndOfFrame();
        }

        if (KnifeThrowButton.gameObject.activeSelf)
            KnifeThrowButton.interactable = true;

        Knife_Timer.text = " ";
    }
    #endregion
    #region Game Management
    public void StartGame(GameObject Selected_Level, GameObject LevelToDisable, bool enableControllerUI)
    {
        /*if(CO_startGame != null)
        {
            StopCoroutine(CO_startGame);

        }
        if (CO_missionText != null)
        {
            StopCoroutine(CO_missionText);
        }*/

        for (int i = 0; i < StarTexts.Count; i++)
        {
            StarTexts[i].isCompleted = false;
            StarTexts[i].StarTextTransform.gameObject.SetActive(false);
            DisplayStars[i].gameObject.SetActive(false);
           
        }
        RewardAmount = 0;
        RewardText.text = "";
        RewardText.gameObject.SetActive(false);
        StopAllCoroutines();
        LeanTween.cancelAll();
        NoInteractionPopUp.GetComponent<CanvasGroup>().alpha = 0;
        CO_startGame = StartCoroutine(StartGameCO(Selected_Level, LevelToDisable, enableControllerUI));
    }
    internal IEnumerator StartGameCO(GameObject Selected_Level, GameObject LevelToDisable, bool enableControllerUI)
    {
        Debug.Log("Fading out");
        fadeScreen.gameObject.SetActive(true);
        GameplayUI.SetActive(false);
        LeanTween.alphaCanvas(GameplayUI.GetComponent<CanvasGroup>(), 0, 1);
        LeanTween.alphaCanvas(MainMenuUI.GetComponent<CanvasGroup>(), 0, 1);
        LeanTween.alphaCanvas(fadeScreen, 1, 1);
        yield return new WaitForSeconds(1);

        Debug.Log("instantiating");
        Destroy(LevelToDisable);
        ResetKnife();
        LevelManager.CurrentGeneratedLevel = Instantiate(Selected_Level);
        LeanTween.alphaCanvas(fadeScreen, 0, 1);
        CurrentMissionText.text = " ";
        yield return new WaitForSeconds(1);

        Debug.Log("fading out");
        if (enableControllerUI)
        {
            PlayerInputs.canControl = true;

            Debug.Log("player can move : " + PlayerInputs.Instance.myCC.canMove);
            Debug.Log("player can control : " + PlayerInputs.canControl);
            GameplayUI.SetActive(true);
            LeanTween.alphaCanvas(GameplayUI.GetComponent<CanvasGroup>(), 1, 1).setOnComplete(() => ShowGameType());
        }
        else
        {
            MainMenuUI.SetActive(true);
            LeanTween.alphaCanvas(MainMenuUI.GetComponent<CanvasGroup>(), 1, 1);
        }
        yield return new WaitForSeconds(1);


        Debug.Log("Game On");
        fadeScreen.gameObject.SetActive(false);
        CO_startGame = null;
    }


    #region GameOverRegion

    [Header("GameOverSection")]
    [SerializeField] TextMeshProUGUI GameOverText;
    [SerializeField] Button NextLevelButton;
    [SerializeField] RectTransform GameOverObjectsPanel;
    [SerializeField] List<ReSortStarstext> StarTexts = new List<ReSortStarstext>();
    [SerializeField] List<RectTransform> DisplayStars = new List<RectTransform>();
    [SerializeField] Sprite CorrectPic;
    [SerializeField] Sprite WrongPic;
    public int RewardAmount;
    public TextMeshProUGUI RewardText;
    [System.Serializable]
    public class ReSortStarstext:IComparable
    {
        public Transform StarTextTransform;
        public bool isCompleted = false;
        public string StarType;
        public Image Icon;
        

        public int CompareTo(object obj)
        {
            return isCompleted ? 1 : 0;
        }
    }
   

    internal void showGameOverPanel(bool gameWon)
    {
        StartCoroutine(ShowGameOverCO(gameWon));
    }
    internal void taskCompleted(string v,int index)
    {
        for (int i = 0; i < StarTexts.Count; i++)
        {
            if (StarTexts[i].StarType == v)
            {
                StarTexts[i].isCompleted = true;
                break;
            }
        }
    }

    IEnumerator ShowGameOverCO(bool gameWon)
    {
        Debug.Log("Showing game over panel");
        GameOverUI.SetActive(true);
        GameOverUI.GetComponent<ScrollRect>().enabled = false;
        if (gameWon)
        {
            GameOverObjectsPanel.anchoredPosition = new Vector2(-125, GameOverObjectsPanel.anchoredPosition.y);
            GameOverText.text = "Level Completed!";
            LevelManager.CurrentLevelComplete();

            StarTexts.Sort((x, y) => y.CompareTo(x));
            for (int i = 0; i < StarTexts.Count; i++)
            {
                int temp = i;
                StarTexts[temp].StarTextTransform.gameObject.SetActive(true);
                StarTexts[temp].StarTextTransform.SetSiblingIndex(temp);
                StarTexts[temp].StarTextTransform.GetComponent<TMP_Text>().color = StarTexts[temp].isCompleted ? Color.green : Color.yellow;
                StarTexts[temp].Icon.sprite = StarTexts[temp].isCompleted ? CorrectPic : WrongPic;
                yield return new WaitForSeconds(0.5f);

                DisplayStars[temp].GetChild(0).gameObject.SetActive(StarTexts[temp].isCompleted);
                DisplayStars[temp].gameObject.SetActive(true);
                //LeanTween.alpha(StarTexts[temp].StarTextTransform.gameObject, 1, 1).setFrom(0);
                yield return new WaitForSeconds(0.5f);
            }
            if (RewardAmount > 0) RewardText.gameObject.SetActive(true);
            LeanTween.value(0, RewardAmount, 2).setEaseInQuad().setOnUpdate((float value) => RewardText.text = "+" + value.ToString("F0"));
            yield return new WaitForSeconds(2);
            LeanTween.moveLocalX(GameOverObjectsPanel.gameObject, GameOverObjectsPanel.anchoredPosition.x - 225, 1).setOnComplete(() => GameOverUI.GetComponent<ScrollRect>().enabled = true); ;
            Debug.Log("game won");
        }
        else
        {
            Debug.Log("game lost");
            GameOverText.text = "Don't Give Up Yet!";
        }
        NextLevelButton.interactable = gameWon;
        yield return null;
    }


    #endregion

    public void QuitApllication()
    {
        Application.Quit();
    }
    #endregion
    #region UI Management
    internal void DoorOpenIcon(bool state)
    {
        if (DoorIsOpenIcon && DoorIsCloseIcon)
        {
            Debug.Log("Door opening icon" + state);
            DoorIsCloseIcon.SetActive(state);
            DoorIsOpenIcon.SetActive(!state);

        }
    }
    internal void ShowRemainingTargets(int Finished, int Total)
    {
        if (RemainingTargetText)
        {
            Debug.Log("changing remaining targets text");
            string S_finished = Finished.ToString("D2");
            string S_total = Total.ToString("D2");
            if(Finished ==0 && Total == 0)
            {
                RemainingTargetText.text = "";
            }
            else
            {
                RemainingTargetText.text = S_finished + "/" + S_total;
            }
        }
    }
    public void SaveUIPosData()
    {
        for (int i = 0; i < uiPosHolderList.Count; i++)
        {
            uiPosHolderList[i].SaveData();
        }
    }
    public void ResetUIPosData()
    {
        for (int i = 0; i < uiPosHolderList.Count; i++)
        {
            uiPosHolderList[i].ResetPos();
        }
    }
    internal void UpdateHealth(float value)
    {
        value = Mathf.Clamp(value, 0, 1000);
        if (healthBar != null)
        {
            healthBar.text = "X" + value / 2;
        }
    }
    public void ShowGameType()
    {
        string text = null;
        switch (LevelDataHolder.Instance.GameType)
        {
           // < color =#AAAAAA>Play </color> asdfasfa
            case LevelDataHolder.mission.save:
                text = "<b><color=#0F0>Save</color></b> All The Girls Being Held Hostage By Ninjas";
                break;
            case LevelDataHolder.mission.kill:
                text = "<b><color=#F00>Kill</color></b> All The Ninjas";
                break;
            case LevelDataHolder.mission.spy:
                text = "<b><color=#0F0>Save</color></b> All The Girls <b><color=#F00>Without Killing</color></b> Anyone";
                break;
            case LevelDataHolder.mission.save_kill:
                text = "<b><color=#0F0>Save</color></b> All The Girls And <b><color=#F00>Kill</color></b> Everyone Else";
                break;
            case LevelDataHolder.mission.survival:
                text = "Find An Exit <b><color=#0F0>Without Dying</color></b>";
                break;
        }
        
        CO_missionText = StartCoroutine(TypeSentences(CurrentMissionText, text));
       // CurrentMissionText.text = text;
       // RectTransform rect = CurrentMissionText.GetComponent<RectTransform>();
       // LeanTween.moveY(rect,  -CanvasMain.sizeDelta.y /2  + 25f, 1).setFrom(0);
    }

    public void ShowNoInteractionPopUp(string text , float duration)
    {
        NoInteractionPopUpText.text = text;
        CanvasGroup CG = NoInteractionPopUp.GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(CG, 1, 1).setFrom(0).setOnComplete(() => LeanTween.alphaCanvas(CG, 0, 1).setDelay(duration));
    }
    public void SetCoinText()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        if (data != null)
        {
            for (int i = 0; i < CurrentCoinCount.Length; i++)
            {
                CurrentCoinCount[i].text = data.coins.ToString();
            }
        }
    }
    #endregion
}