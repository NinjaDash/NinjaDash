using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyNFTCollection : MonoBehaviour
{
    public static MyNFTCollection insta;
    

        
    

    int currentSelectedItem = 0;    

    [SerializeField] GameObject LoadingMyCollection;
    [SerializeField] GameObject MyCollectionObject;


    [SerializeField] TMP_Text[] priceTexts;
    

    private void Awake()
    {
        insta = this;
       
    }

    async private void OnEnable()
    {
        LoadingMyCollection.SetActive(true);
        MyCollectionObject.SetActive(false);

        for (int i = 0; i < DatabaseManager.Instance.allMetaDataServer.Count; i++)
        {
            priceTexts[i].text = DatabaseManager.Instance.allMetaDataServer[i].cost.ToString();
        }
        await CoreWeb3Manager.Instance.CheckPuzzleList();

        SetNewData();

        LoadingMyCollection.SetActive(false);
        MyCollectionObject.SetActive(true);
       
        UIManager.Instance.SetCoinText();       
    }

    [SerializeField] List<int> available_skins=new List<int>();
    public List<int> AvailableSkins { get { return available_skins; } }
    public void SetNewData()
    {
        List<string> temp_list = new List<string>();


        temp_list = CoreWeb3Manager.Instance.nftList;
        available_skins = new List<int>();
        available_skins.Add(0);

        if (temp_list.Count > 0)
        {
            for (int i = 0; i < temp_list.Count; i++)
            {
                if (temp_list[i].StartsWith("5") && temp_list[i].Length == 3)
                {
                    available_skins.Add(Int32.Parse(temp_list[i]) - 499);
                    //MyNFTCollection.insta.GenerateItem(Int32.Parse(temp_list[i]));
                }
            }
        }
        StoreManager.Instance.RefreshSkinsStatus(available_skins);

      

      
    }

    public int lastSelectedButton = -1;
    public void DisableLastSelectedButton()
    {
        Debug.Log(lastSelectedButton + 1);

       
    }
    private void OnDisable()
    {
      
    }
    

   
   

    public void CloseItemPanel()
    { 
        gameObject.SetActive(false);
    }



    public void DeductCoins(int amount)
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.coins -= amount;
        DatabaseManager.Instance.UpdateData(data);
        UIManager.Instance.SetCoinText();
    }





}
