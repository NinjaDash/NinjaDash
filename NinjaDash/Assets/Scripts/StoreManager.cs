using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreManager : MonoBehaviour
{
    #region SingleTon
    public static StoreManager Instance;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
    }
    #endregion
    public List<GameObject> Skins = new List<GameObject>();    
    public GameObject CoinShopPanel;
    public TextMeshProUGUI[] skinStatusText;    
    public List<int> skinPrices = new List<int>();

    private void Start()
    {
        

    }


    public void SelectSkin(int ID)
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        if(MyNFTCollection.insta.AvailableSkins.Contains(ID))
        {
            Debug.Log("Skin Selected");
            data.SelectedSkin = ID;
        }
        else
        {
            int cost = DatabaseManager.Instance.allMetaDataServer[ID-1].cost;            
            if (data.coins >= cost)
            {
                Debug.Log("0 got added" + ID);                
                //data.SelectedSkin = ID;
                CoreWeb3Manager.Instance.purchaseItem(ID-1);

              
            }
            else
            {
                MessageBox.insta.showMsg("Not Enough Money", true);
                Debug.Log("Not Enough Money");
            }
        }
        DatabaseManager.Instance.UpdateData(data);
        RefreshSkinsStatus();
    }
    public void OpenCoinShop()
    {
        Debug.Log("Open Coin Shop here");
        CoinShopPanel.SetActive(true);
    }

  
    public void CoinPack(int ID)
    {
        Debug.Log("Selected Coin pack num" + ID);
    }

    public void RefreshSkinsStatus()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        for (int i = 0; i < skinStatusText.Length; i++)
        {
            if (MyNFTCollection.insta.AvailableSkins.Contains(i))
            {
                if (data.SelectedSkin == i)
                {
                    skinStatusText[i].text = "Selected";
                }
                else
                {
                    skinStatusText[i].text = "Select";
                }
                skinStatusText[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                skinStatusText[i].text = "Buy";
            }
        }
        UIManager.Instance.SetCoinText();
    }
    public void RefreshSkinsStatus(List<int> availableSkins)
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        for (int i = 0; i < skinStatusText.Length; i++)
        {
            if(availableSkins.Contains(i))
            {
                if (data.SelectedSkin == i)
                {
                    skinStatusText[i].text = "Selected";
                }
                else
                {
                    skinStatusText[i].text = "Select";
                }
                skinStatusText[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                skinStatusText[i].text = "Buy";
            }
        }
        UIManager.Instance.SetCoinText();
    }
}
