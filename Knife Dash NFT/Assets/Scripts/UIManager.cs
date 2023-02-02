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

    public string username;
    public int user_char;


    public RectTransform CanvasMain;
    public TextMeshProUGUI healthBar;
    public GameObject DoorIsCloseIcon;
    public GameObject DoorIsOpenIcon;
    public TextMeshProUGUI RemainingTargetText;
    public CharacterController2D myCC;
    public GameObject GameplayUI;
    public GameObject MainMenuUI;
    public GameObject GameOverUI;
    public GameObject claimTokenBTN;
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
    public class ReSortStarstext : IComparable
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
    internal void taskCompleted(string v, int index)
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
        claimTokenBTN.SetActive(false);
        GameOverUI.GetComponent<ScrollRect>().enabled = false;
        if (gameWon)
        {
            GameOverObjectsPanel.anchoredPosition = new Vector2(-125, GameOverObjectsPanel.anchoredPosition.y);
            GameOverText.text = "Level Completed!";
            LevelManager.CurrentLevelComplete();

            StarTexts.Sort((x, y) => y.CompareTo(x));
            int totalStars = 0;
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
                totalStars += StarTexts[temp].isCompleted ? 1 : 0;
            }

            claimTokenBTN.SetActive(totalStars == 3);

            if (RewardAmount > 0) RewardText.gameObject.SetActive(true);
            LeanTween.value(0, RewardAmount, 2).setEaseInQuad().setOnUpdate((float value) => RewardText.text = "+" + value.ToString("F0"));
            yield return new WaitForSeconds(2);
            LeanTween.move(GameOverObjectsPanel,new Vector2( GameOverObjectsPanel.anchoredPosition.x - 225, GameOverObjectsPanel.anchoredPosition.y), 1)/*.setOnComplete(() => GameOverUI.GetComponent<ScrollRect>().enabled = true)*/;
            Debug.Log("game won");
        }
        else
        {
            GameOverObjectsPanel.anchoredPosition = new Vector2(-125, GameOverObjectsPanel.anchoredPosition.y);
            Debug.Log("game lost");
            GameOverText.text = "Don't Give Up Yet!";
            LeanTween.move(GameOverObjectsPanel, new Vector2(GameOverObjectsPanel.anchoredPosition.x - 225, GameOverObjectsPanel.anchoredPosition.y), 0);/*.setOnComplete(() => GameOverUI.GetComponent<ScrollRect>().enabled = true)*/;
        }
        NextLevelButton.interactable = gameWon;
        yield return null;
    }


    #endregion

    #region Token Claim

    public void ClaimToken()
    {
        claimTokenBTN.SetActive(false);
        CoreWeb3Manager.Instance.getDailyToken();
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
    
    #endregion
    #region Coin Texts
    
    [SerializeField] TMP_Text[] token_texts;
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
    public void SetTokenBalanceText()
    {
        for (int i = 0; i < token_texts.Length; i++)
        {
            token_texts[i].text = CoreWeb3Manager.userTokenBalance;
        }
    }
    #endregion

    #region Edit Profile Section    
    [SerializeField] TMP_InputField name_input;
    [SerializeField] GameObject start_ui_btns;
    [SerializeField] GameObject editprofile_ui;
    public void OpenEditProfile()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();

        name_input.text = data.name;       

        start_ui_btns.SetActive(false);
        editprofile_ui.SetActive(true);
    }
    public void SetProfile()
    {
        if (string.IsNullOrEmpty(name_input.text)) return;

        LocalData data = DatabaseManager.Instance.GetLocalData();

        data.name = name_input.text;
       
        DatabaseManager.Instance.UpdateData(data);


        start_ui_btns.SetActive(true);
        editprofile_ui.SetActive(false);
        UpdateUserName(data.name, SingletonDataManager.userethAdd);
    }

    #endregion
    [Space(20f)]
    [Header("Informaion (Login)")]
    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text statusText;
    public void UpdateUserName(string _name, string _ethad = null)
    {
        if (_ethad != null)
        {
            usernameText.text = "Hi, " + _name + "\n  Your crypto address is : " + _ethad;
            username = _name;
        }
        else usernameText.text = _name;
    }

    [Header("Informaion (InGame)")]
    [SerializeField] GameObject information_box;
    [SerializeField] TMP_Text information_text;
    [SerializeField] Image information_image;
    Coroutine info_coroutine;
    public void ShowInformationMsg(string msg, float time, Sprite image = null)
    {
        if (image != null)
        {
            information_image.sprite = image;
            information_image.gameObject.SetActive(true);
        }
        else
        {
            information_image.gameObject.SetActive(false);
        }

        information_text.text = msg;

        if (info_coroutine != null)
        {
            StopCoroutine(info_coroutine);
        }
        info_coroutine = StartCoroutine(disableInformationMsg(time));
    }
    IEnumerator disableInformationMsg(float time)
    {
        LeanTween.cancel(information_box);

        information_box.SetActive(true);
        LeanTween.scaleY(information_box, 1, 0.15f).setFrom(0);
        // AudioManager.Instance.playSound(0);

        yield return new WaitForSeconds(time);

        LeanTween.scaleY(information_box, 0, 0.15f).setOnComplete(() => {
            information_box.SetActive(false);
        });


    }
}