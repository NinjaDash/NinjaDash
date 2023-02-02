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

    public  List<MetaFunNFTLocal> allMetaDataServer = new List<MetaFunNFTLocal>();
    public LocalData GetLocalData()
    {
       
        return data;
    }


    private void Start()
    {
       StartCoroutine(getNFTAllData());
       //GetData(true);
    }
    IEnumerator updateProfile(int dataType, bool createnew = false)
    {

        JSONObject a = new JSONObject();
        JSONObject b = new JSONObject();
        JSONObject c = new JSONObject();
        //JSONObject d = new JSONObject();

        string url = ConstantManager.getProfile_api + PlayerPrefs.GetString("Account", "test").ToLower();
        switch (dataType)
        {
            case 0:

                if (!createnew) url += "?updateMask.fieldPaths=userdata";
                else
                {
                    data = new LocalData();

                    data.StarsPerLevel = new List<int>();
                    for (int i = 0; i < 15; i++)
                    {
                        data.StarsPerLevel.Add(0);
                    }
                    data.name = "Player" + UnityEngine.Random.Range(0,99999).ToString();

                    
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
                

                if (createnew)
                {

                    CoreWeb3Manager.Instance.EnablePlayPanels();
                    UIManager.Instance.OpenEditProfile();
                }
            }
        }
    }

    IEnumerator CheckProfile(bool firstTime = false)
    {
        string url = ConstantManager.getProfile_api + PlayerPrefs.GetString("Account", "test2").ToLower();

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
                //TEST RESET DATA
               //StartCoroutine(updateProfile(0, true));

               JSONObject obj = new JSONObject(www.downloadHandler.text);
                Debug.Log(obj);
                //Debug.Log("CheckProfile " + www.downloadHandler.text);
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalData>(obj.GetField("fields").GetField("userdata").GetField("stringValue").stringValue);

                if (UIManager.Instance) {
                    UIManager.Instance.username = data.name;
                    UIManager.Instance.user_char = data.SelectedSkin;

                    UIManager.Instance.UpdateUserName(data.name, SingletonDataManager.userethAdd);
                }

                CoreWeb3Manager.Instance.EnablePlayPanels();              

                
                //Debug.Log(obj.GetField("fields").GetField("musedata").GetField("stringValue").stringValue);
                if (data.transactionsInformation!=null && data.transactionsInformation.Count > 0)
                {
                    for (int i = 0; i < data.transactionsInformation.Count; i++)
                    {
                        if (data.transactionsInformation[i].transactionStatus.Equals("pending"))
                        {
                            Debug.Log("Pending Test 1");
                            CoreWeb3Manager.Instance.CheckDatabaseTransactionStatus(data.transactionsInformation[i].transactionId);                         
                        }
                    }
                }
            }
        }
    }

    
    IEnumerator getNFTAllData()
    {
        
        using (UnityWebRequest www = UnityWebRequest.Get(ConstantManager.getgameNFTData_api))
        {
              www.timeout = 60;
              yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("getNFTAllData not found " + www.downloadHandler.text);
                //Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);

                //StartCoroutine(updateProfile(0, true));
            }
            else
            {
                Debug.Log("getNFTAllData  found " + www.downloadHandler.text);
                JSONObject obj = new JSONObject(www.downloadHandler.text);


                //data = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalData>(obj.GetField("fields").GetField("data").GetField("stringValue").stringValue);
                Debug.Log("Data >>  " + obj);

                allMetaDataServer = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MetaFunNFTLocal>>(obj.GetField("fields").GetField("data").GetField("stringValue").stringValue);
                
                GetAllNFTImg();
            }
        }
    }
    public void GetAllNFTImg()
    {
        for (int i = 0; i < allMetaDataServer.Count; i++)
        {
            StartCoroutine(GetTexture(allMetaDataServer[i].imageurl, i));
        }

    }
    IEnumerator GetTexture(string _url, int _index)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            allMetaDataServer[_index].imageTexture = (((DownloadHandlerTexture)www.downloadHandler).texture);
        }
    }

    public Texture GetNFTTexture(int tokenId)
    {       
        MetaFunNFTLocal result = allMetaDataServer.Find(x => x.itemid == tokenId);
        return result.imageTexture;
    }

    public string GetNFTName(int tokenId)
    {
        MetaFunNFTLocal result = allMetaDataServer.Find(x => x.itemid ==tokenId);
        return result.name;
    }
    public MetaFunNFTLocal GetNFTMetaData(int tokenId)
    {
        MetaFunNFTLocal result = allMetaDataServer.Find(x => x.itemid == tokenId);
        return result;
    }


    public void GetData(bool firstTime=false)
    {
        StartCoroutine(CheckProfile(firstTime));
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
        data.last_spin_time =(await GetCurrentTime()).ToString();
        StartCoroutine(updateProfile(0));
    }
   
  /*  public DateTime ConvertEpochToDatatime(long epochSeconds) {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(epochSeconds);
        DateTime dateTime = dateTimeOffset.DateTime;
        
        return dateTime;
    }
*/
    async public Task<long> GetCurrentTime()
    {
        
        string result =await CoreWeb3Manager.Instance.CheckTimeStatus();
       
        long currentEpoch;
        if (!string.IsNullOrEmpty(result))
        {
            currentEpoch=long.Parse(result);
        }
        else
        {
            currentEpoch = 1659504437;
        }
        // Get Current EPOCH Time
       // DateTime currentTime= ConvertEpochToDatatime(currentEpoch);
        return currentEpoch;
    }

    public void AddTransaction(string TransId, string status,int _shopId)
    {
        TranscationInfo info = new TranscationInfo(TransId, status);
        switch (_shopId)
        {
            case 0:
                {
                    info.coinAmount = 500;
                    break;
                }
            case 1:
                {
                    info.coinAmount = 1000;
                    break;
                }
            case 2:
                {
                    info.coinAmount = 2000;
                    break;
                }
            case 3:
                {
                    info.coinAmount = 4000;
                    break;
                }
        }

        data.transactionsInformation.Add(info);
        UpdateData(data);
    }
    public void AddCoins(int pack)
    {
        switch (pack)
        {
            case 1: {
                    data.coins += 500;

                    break;
                }
            case 2:
                {
                    data.coins += 1000;
                    break;
                }
            case 3:
                {
                    data.coins += 2000;
                    break;
                }
            case 4:
                {
                    data.coins += 4000;
                    break;
                }

        }
        UpdateData(data);
        UIManager.Instance.SetCoinText();

    }
    public void ChangeTransactionStatus(string transID, string txConfirmed)
    {
        Debug.Log("Changing Database " + transID + " " + txConfirmed);
        TranscationInfo trans_info = data.transactionsInformation.Find(x => x.transactionId == transID);
        if (trans_info != null)
        {
            int index = data.transactionsInformation.IndexOf(trans_info);
            trans_info.transactionStatus = txConfirmed;
            data.transactionsInformation[index] = trans_info;
            if (txConfirmed.Equals("success"))
            {
                data.coins += trans_info.coinAmount;
                data.transactionsInformation.RemoveAt(index);
            }
            

            UpdateData(data);

            if (UIManager.Instance)
            {
                UIManager.Instance.ShowInformationMsg(trans_info.coinAmount.ToString() + " Coins Added", 3);
                UIManager.Instance.SetCoinText();
            }
        }       
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
    public int highscore = 0;

    public int FinishedLevels = 0;
    public int SelectedSkin = 0;

    
    public List<int> StarsPerLevel = new List<int>();
    public bool AdRemovalPurchased = false;
    public List<TranscationInfo> transactionsInformation = new List<TranscationInfo>();
    public string last_spin_time="";

    public LocalData()
    {
        name = "Player";
        coins = 0;
        highscore = 0;
        FinishedLevels = 0;
        SelectedSkin = 0;
        
        StarsPerLevel = new List<int>();
        AdRemovalPurchased = false;
        transactionsInformation = new List<TranscationInfo>();
        last_spin_time = "0";
    }


}




[System.Serializable]
public class TranscationInfo
{
    public string transactionId;
    public string transactionStatus;
    public int coinAmount;
    public TranscationInfo(string Id, string status)
    {
        transactionId = Id;
        transactionStatus = status;        
    }
}