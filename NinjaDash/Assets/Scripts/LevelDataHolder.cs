using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(1)]
public class LevelDataHolder : MonoBehaviour
{
    public static LevelDataHolder Instance;

    private void Awake()
    {
       Instance = this;
        if (DatabaseManager.Instance)
        {
            LocalData data = DatabaseManager.Instance.GetLocalData();
            Instantiate(StoreManager.Instance.Skins[data.SelectedSkin], MyCC.transform);
        }
    }

    private void Start()
    {
        if (AmountOfEnemies == 0) AllEnemiesKilled = true;
        if (AmountOfHostages == 0) AllGirlsSaved = true;
        CheckForCompletion();
    }

    public static bool RewardHealth { get; set; }
    public static bool RewardSkip { get; set; }
    public bool GoBackToLastPos { get; set; }

    public int LevelID;
    public int StarsEarned = 1;
    public CharacterController2D MyCC;
    public mission GameType;
    public GameOver gameOverScript;
    public int overrideHostageAmount = -1;
    public int overrideEnemiesAmount = -1;
    public int AmountOfHostages;
    public int AmountOfEnemies;

    public int targetAmount = -1;
    public int completedAmount = 0;

    [Space(20)]

    public bool AllGirlsSaved;
    public bool AllEnemiesKilled;


    [SerializeField] private int _girlsSaved;
    [SerializeField] private int _enemiesKilled;

    public int GirlsSaved { get { return _girlsSaved; }
        set
        {
            _girlsSaved = value;

            if (overrideHostageAmount == -1)
            {
                if (AmountOfHostages <= value)
                {
                    AllGirlsSaved = true;
                }
            }else
            {
                if(overrideHostageAmount <= value)
                {
                    AllGirlsSaved = true;
                }
            }
            CheckForCompletion();
        }
    }


    public int EnemiesKilled
    {
        get { return _enemiesKilled; }
        set
        {
            _enemiesKilled = value;
            if (overrideEnemiesAmount == -1)
            {
                if (AmountOfEnemies <= value)
                {
                    AllEnemiesKilled = true;
                }
            }
            else
            {
                if (overrideEnemiesAmount <= value)
                {
                    AllEnemiesKilled = true;
                }
            }
            CheckForCompletion();
        }
    }
    [ContextMenu("Check")]
    private void CheckForCompletion()
    {
        switch (GameType)
        {
            case mission.save:
                {
                    if (targetAmount == -1)
                    {
                        targetAmount = overrideHostageAmount != -1 ? overrideHostageAmount : AmountOfHostages;
                    }

                    completedAmount = GirlsSaved;
                    //gameOverScript.toggleWall(!AllGirlsSaved);
                }
                break;
            case mission.kill:
                {
                    if (targetAmount == -1)
                    {
                        targetAmount = overrideEnemiesAmount != -1 ? overrideEnemiesAmount : AmountOfEnemies;
                    }
                    completedAmount = EnemiesKilled;
                    //gameOverScript.toggleWall(!AllEnemiesKilled);
                }
                break;
            case mission.spy:
                {
                    if (targetAmount == -1)
                    {
                        targetAmount = overrideHostageAmount != -1 ? overrideHostageAmount : AmountOfHostages;
                    }
                    completedAmount = GirlsSaved;
                    if (EnemiesKilled == 0)
                    {
                       // gameOverScript.toggleWall(!AllGirlsSaved);
                    }
                    else
                    {
                        /*Debug.Log("GameOver");
                        UIManager.Instance.TempRestart();*/
                        gameOverScript.GameDone(PlayerInputs.Instance.myCC, false);
                    }
                }
                break;
            case mission.save_kill:
                {
                    if (targetAmount == -1)
                    {
                        int hs = overrideHostageAmount != -1 ? overrideHostageAmount : AmountOfHostages;
                        int es = overrideEnemiesAmount != -1 ? overrideEnemiesAmount : AmountOfEnemies;
                        targetAmount = hs + es;
                    }
                    completedAmount = GirlsSaved + EnemiesKilled;
                    Debug.Log("Checking");
                    //gameOverScript.toggleWall(!AllGirlsSaved || !AllEnemiesKilled);
                }
                break;
            case mission.survival:
                {
                    targetAmount = 0;
                    completedAmount = 0;
                    //gameOverScript.toggleWall(false);
                }
                break;
        }
        gameOverScript.toggleWall(targetAmount == completedAmount);
        UIManager.Instance.ShowRemainingTargets(completedAmount, targetAmount);
    }

    public enum mission
    {
        save,
        kill,
        spy,
        save_kill,
        survival
    }

    private void Update()
    {
        if(RewardHealth)
        {
            RewardHealth = false;
            MyCC.life+=2;
            UIManager.Instance.ShowNoInteractionPopUp("Player Healed" , 3);
        }
        if(RewardSkip)
        {
            RewardSkip = false;
            UIManager.Instance.levelManager.SkipLevel();
        }
    }


    public int HowManyStarts(float health, bool CompletedAll)
    {
        StarsEarned = 1;
        UIManager.Instance.taskCompleted("Finish",StarsEarned);
        if (health >= 8) { StarsEarned++; UIManager.Instance.taskCompleted("Health", StarsEarned); }
        if (CompletedAll) { StarsEarned++; UIManager.Instance.taskCompleted("Target", StarsEarned); }
        return StarsEarned;
    }

    public void SaveStarsAmount()
    {
        Debug.Log(StarsEarned);
        LocalData data = DatabaseManager.Instance.GetLocalData();
        int reward = 0;
        int PreviousRecord = data.StarsPerLevel[LevelID];
        if(StarsEarned > PreviousRecord)
        {
            data.StarsPerLevel[LevelID] = StarsEarned;
            reward = (StarsEarned - PreviousRecord) * 25;
            data.coins += reward;
            UIManager.Instance.RewardAmount = reward;
            DatabaseManager.Instance.UpdateData(data);
        }

    }

    public void PurchaseHealthWithCoin()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.coins -= 3;
        DatabaseManager.Instance.UpdateData(data);
        RewardHealth = true;

    }
}
