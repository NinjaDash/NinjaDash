using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    #region Singleton
    public static DatabaseManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    
    #endregion



    [SerializeField] private LocalData data=new LocalData();

    
    public LocalData GetLocalData()
    {
        return data;
    }


    private void Start()
    {
       ///StartCoroutine(getNFTAllData());
       //GetData(true);
    }
    
    IEnumerator updateProfile(int dataType, bool createnew = false)
    {

        JSONObject a = new JSONObject();
        JSONObject b = new JSONObject();
        JSONObject c = new JSONObject();
        //JSONObject d = new JSONObject();
        string url = ConstantManager.getProfile_api + PlayerPrefs.GetString("Account", "test");
        switch (dataType)
        {
            case 0:

                if (!createnew) url += "?updateMask.fieldPaths=userdata";
                else
                {
                  
                    //data.name = LoginManager.Instance._user.DisplayName;                  



                    data.PurchasedSkinsID = new List<int>() { 0 };
                    data.StarsPerLevel = new List<int>();
                    for (int i = 0; i < 50; i++)
                    {
                        data.StarsPerLevel.Add(0);
                    }


                }

                c.AddField("stringValue", Newtonsoft.Json.JsonConvert.SerializeObject(data));
                b.AddField("userdata", c);
                break;
           /* case 3:
                if (!createnew) url += "?updateMask.fieldPaths=gamedata";
                c.AddField("stringValue", PlayerPrefs.GetString("data"));
                b.AddField("gamedata", c);
                break;*/
        }

       

        WWWForm form = new WWWForm();

        Debug.Log("TEST updateProfile");

        // Serialize body as a Json string
        //string requestBodyString = "";



        a.AddField("fields", b);

        Debug.Log(a.Print(true));

        // Convert Json body string into a byte array
        byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(a.Print());

        using (UnityWebRequest www = UnityWebRequest.Put(url, requestBodyData))
        {
            www.method = "PATCH";

            // Set request headers i.e. conent type, authorization etc
            //www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Content-length", (requestBodyData.Length.ToString()));
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //JSONObject obj = new JSONObject(www.downloadHandler.text);
                Debug.Log(www.downloadHandler.text);
                //Debug.Log(obj.GetField("fields").GetField("musedata").GetField("stringValue").stringValue);
                if (UIManager.Instance)
                {
                    UIManager.Instance.SetCoinText();
                    //UIManager.Instance.UpdatePlayerUIData(true, data);
                }

                if (createnew)
                {
                    //CoreManager.Instance.EnablePlayPanels();
                }
            }
            /*if(AdController.Instance && data.AdRemovalPurchased)
            {
                AdController.Instance.DestroyAds();
            }*/
        }
    }

    IEnumerator CheckProfile()
    {
        string url = ConstantManager.getProfile_api + PlayerPrefs.GetString("Account", "test2");

      

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            //www.method = "PATCH";

            // Set request headers i.e. conent type, authorization etc
            //www.SetRequestHeader("Content-Type", "application/json");
            // www.SetRequestHeader("Content-length", (requestBodyData.Length.ToString()));
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Profile not found " + www.downloadHandler.text);
                //Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                StartCoroutine(updateProfile(0, true));
            }
            else
            {
                JSONObject obj = new JSONObject(www.downloadHandler.text);
                Debug.Log(obj);
                //Debug.Log("CheckProfile " + www.downloadHandler.text);
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalData>(obj.GetField("fields").GetField("userdata").GetField("stringValue").stringValue);

                if (UIManager.Instance) {
                    UIManager.username = data.name;
                    UIManager.Instance.SetCoinText();
                    // UIManager.insta.UpdatePlayerUIData(true, data); 
                }
               
                //CoreManager.Instance.EnablePlayPanels();
            }
        }
    }

 

    public void GetData()
    {
        Debug.Log("GET DATA");
        StartCoroutine(CheckProfile());
        //ConvertEpochToDatatime(1659504437);
    }

    public void UpdateData(LocalData localData)
    {
        data = localData;
        StartCoroutine(updateProfile(0));
    }
    async public void UpdateSpinData()
    {
        data = GetLocalData();
       
        StartCoroutine(updateProfile(0));
    }


   
    public void ChangeGenderAndNameData(string username)
    {
      
        data.name = username;
        UpdateData(data);
       // UIManager.username = username;        
    }

    public string GetUserName()
    {
        if (data != null)
        {
            return data.name;
        }
        else
        {
            return "";
        }
    }

    
}
[System.Serializable]
public class LocalData
{

    public string name;
    public int coins = 0;
    public int highscore= 0;

    public int FinishedLevels = 0;
    public int SelectedSkin = 0;

    public List<int> PurchasedSkinsID = new List<int>();
    public List<int> StarsPerLevel = new List<int>();
    public bool AdRemovalPurchased = false;

    public LocalData()
    {
        name = "Player";
        coins = 0;
        highscore = 0;
        FinishedLevels = 0;
        SelectedSkin = 0;
        PurchasedSkinsID = new List<int>();
        StarsPerLevel = new List<int>();
        AdRemovalPurchased = false;
    }

}


