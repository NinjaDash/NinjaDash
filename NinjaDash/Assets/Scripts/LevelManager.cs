using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<LevelDataHolder> levels = new List<LevelDataHolder>();
    public static List<LevelDataHolder> levelsData = new List<LevelDataHolder>();

    [SerializeField] Transform levels_button_content;
    [SerializeField] GameObject levelPrefabButton;
    [SerializeField] GameObject MainMenuLevelPrefab;
    [SerializeField] GameObject MainMenuLevel;
    [SerializeField] Sprite EmptyStar;
    [SerializeField] Sprite FilledStar;
    public static int CurrentLevel;
    public static GameObject CurrentGeneratedLevel;    
    bool buttonsGenerated;

    public static bool showAd;


    private void OnEnable()
    {
        if(!buttonsGenerated)
        {
            GenerateButtons();
        }
        if (MainMenuLevel != null)
        {
            CurrentGeneratedLevel = MainMenuLevel;
        }
        SetLevelsButtonInteractable();
        RefreshDataOnEnable();
    }

    /*private void OnValidate()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].LevelID = i;
        }
    }*/
    internal static void CurrentLevelComplete()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        int finishedLevels = data.FinishedLevels;
        if (CurrentLevel >= finishedLevels)
        {
            Debug.Log("new highscore");
            data.FinishedLevels++;
            DatabaseManager.Instance.UpdateData(data);
        }
    }
    private void GenerateButtons()
    {

        levelsData = levels;
        for (int i = 0; i < levels.Count; i++)
        {
            int temp = i;
            GameObject go = Instantiate(levelPrefabButton, levels_button_content);
            go.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = (temp + 1).ToString();
            go.GetComponent<Button>().onClick.AddListener(() => {
                PlayLevel(temp);
            });
            buttonsGenerated = true;
            
        }
    }

    public void RefreshDataOnEnable()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        for (int i = 0; i < levels_button_content.childCount; i++)
        {
            Transform stars = levels_button_content.GetChild(i);
            int StarsToEnable = data.StarsPerLevel[i];
            for (int j = 1; j <= StarsToEnable; j++)
            {
                stars.GetChild(j).GetComponent<Image>().sprite = FilledStar;
            }
        }
    }

    private void PlayLevel(int index)
    {
        if (index < levelsData.Count)
        {
            ResumeGame();
            CurrentLevel = index;
            UIManager.Instance.StartGame(levels[index].gameObject, CurrentGeneratedLevel , true);
            this.gameObject.SetActive(false);
        }
    }
    public void NextLevel()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        int finishedLevels = data.FinishedLevels;
        if (CurrentLevel < finishedLevels)
        {
            //show ads
            if (CurrentLevel > 2)
            {
                if (showAd)
                {
                   // AdController.Instance.ShowInterstitialAd();
                }
                showAd = !showAd;
            }
            PlayLevel(CurrentLevel + 1);
            Debug.Log("Normal next level");
        }
        else
        {
            UIManager.Instance.SkipLevelBox.SetActive(true);
        }
    }
    public void NextLevelWithSkip()
    {
        // dont show ads
        Debug.Log("Doing next level with skip");
        PlayLevel(CurrentLevel + 1);
    }
    public void SkipLevel()
    {
        CurrentLevelComplete();
        NextLevelWithSkip();
    }
    public void RestartLevel()
    {
      
        StopAllCoroutines();
        LeanTween.cancelAll();
        PlayerInputs.Instance.myCC.invincible = true;
        CurrentGeneratedLevel.GetComponent<LevelDataHolder>().gameOverScript.enabled = false;
        PlayLevel(CurrentLevel);
    }
    public void GoToMainMenu()
    {
        ResumeGame();
        PlayerInputs.Instance.myCC.invincible = true;
        CurrentGeneratedLevel.GetComponent<LevelDataHolder>().gameOverScript.enabled = false;
        UIManager.Instance.StartGame(MainMenuLevelPrefab, CurrentGeneratedLevel , false);
    }
    public void SetLevelsButtonInteractable()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        int currenct_level = data.FinishedLevels;
        for (int i = 0; i < levels_button_content.childCount; i++)
        {
            levels_button_content.GetChild(i).GetComponent<Button>().interactable = false;
            if (currenct_level >= i)
            {
                levels_button_content.GetChild(i).GetComponent<Button>().interactable = true;
            }
        }
    }
    

    public static void ResumeGame()
    {
        UIManager.Instance.PauseMenuUI.SetActive(false);
        Time.timeScale = 1;        
    }
    public static void PauseGame()
    {
        Time.timeScale = 0;
        UIManager.Instance.PauseMenuUI.SetActive(true);
    }
}

