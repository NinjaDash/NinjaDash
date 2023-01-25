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
        if(data.PurchasedSkinsID.Contains(ID))
        {
            Debug.Log("Skin Selected");
            data.SelectedSkin = ID;
        }
        else
        {
            int cost = skinPrices[ID];            
            if (data.coins >= cost)
            {
                Debug.Log("0 got added" + ID);
                data.coins -= cost;
                data.PurchasedSkinsID.Add(ID);
                data.SelectedSkin = ID;
                Debug.Log("Purchase Successful");
            }
            else
            {
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
            if(data.PurchasedSkinsID.Contains(i))
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
