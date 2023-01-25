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

    //[SerializeField] PlayerCarsInfo playerCarInfo;

    [SerializeField] List<int> available_cars=new List<int>();
    [SerializeField] List<int> all_cars = new List<int>();
    [SerializeField] Button buy_select_btn;
    [SerializeField] TMP_Text car_cost_text;


    int selected_color;
    

    private void Awake()
    {
        insta = this;
        this.gameObject.SetActive(false);
    }

    async private void OnEnable()
    {
        LoadingMyCollection.SetActive(true);
        MyCollectionObject.SetActive(false);

        selectedCarinUI = 0;
        // ClosePurchasePanel();
        all_cars.Clear();
       

        await CoreWeb3Manager.Instance.CheckPuzzleList();

        
        SetNewData();

        
        selected_color= DatabaseManager.Instance.GetLocalData().SelectedSkin;

        

        prev_BTN.SetActive(false);
        next_BTN.SetActive(true);
           
        LoadingMyCollection.SetActive(false);
        MyCollectionObject.SetActive(true);
       
        UIManager.Instance.SetCoinText();       
    }


    public void SetNewData(bool clearOldData=true)
    {
        List<string> temp_list = new List<string>();

        if (clearOldData)
        {
            available_cars.Clear();
            available_cars.Add(0);
        }
        

        temp_list = CoreWeb3Manager.Instance.nftList;

        if (temp_list.Count > 0)
        {
            for (int i = 0; i < temp_list.Count; i++)
            {
                if (temp_list[i].StartsWith("5") && temp_list[i].Length == 3)
                {
                    available_cars.Add(Int32.Parse(temp_list[i]) - 499);
                    //MyNFTCollection.insta.GenerateItem(Int32.Parse(temp_list[i]));
                }
            }
        }

        for (int i = 0; i < available_cars.Count; i++)
        {
           //data.carDetails[available_cars[i]].isBought = true;
           Debug.Log("Available : " + available_cars[i]);
        }

       

        if (clearOldData)
        {
            prev_BTN.SetActive(false);
            next_BTN.SetActive(true);

            SetCarInfo(0);
        }
        else
        {
            SetCarInfo(selectedCarinUI);
        }
    }

    public int lastSelectedButton = -1;
    public void DisableLastSelectedButton()
    {
        Debug.Log(lastSelectedButton + 1);

        if (!available_cars.Contains(lastSelectedButton))
        {
            available_cars.Add(lastSelectedButton);
                    
        }        
    }
    private void OnDisable()
    {
      
    }
    #region Car Info Management
        
    //0-Accel  1-Max Speed 2-Braking 3-NItroTime  4-Drift Control
    
    int selectedCarinUI = 0;
    [SerializeField] GameObject prev_BTN;
    [SerializeField] GameObject next_BTN;
    [SerializeField] Image carImage;
    [SerializeField] TMP_Text carSpeed_txt;
    [SerializeField] TMP_Text carMaxFuel_txt;
    [SerializeField] TMP_Text carAvg_txt;
    [SerializeField] TMP_Text carTurnSpeed_txt;
    [SerializeField] TMP_Text carBagCapacity_txt;




    public void NextCar()
    {

        if (selectedCarinUI >= all_cars.Count - 1) return;

        selectedCarinUI++;
        if (selectedCarinUI == all_cars.Count - 1) next_BTN.SetActive(false);


   
        prev_BTN.SetActive(true);

        SetCarInfo(selectedCarinUI);
        Debug.LogWarning("Change Car Stats UI HERE");

    }
    public void PreviousCar()
    {
        if (selectedCarinUI == 0) return;

        selectedCarinUI--;

        if (selectedCarinUI == 0) prev_BTN.SetActive(false);

        next_BTN.SetActive(true);

        SetCarInfo(selectedCarinUI);
        Debug.LogWarning("Change Car Stats UI HERE");
    }


    
    

    
    //0-FireRate 1-Damage  2=Reload Time 3-Accuracy 

    

    int lastSelectedCar;

 

    void SetCarInfo(int carIndex)
    {
        lastSelectedCar = carIndex;
        
        bool isBought = available_cars.Contains(carIndex);

        buy_select_btn.onClick.RemoveAllListeners();
        if (isBought)
        {
            buy_select_btn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Select";
          
            buy_select_btn.onClick.AddListener(() => {
                SelectCar(carIndex);
            });

            car_cost_text.gameObject.SetActive(false);
        }
        else
        {
            buy_select_btn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Buy";

            buy_select_btn.onClick.AddListener(() => {
                PurchaseCar(carIndex);
            });

            car_cost_text.gameObject.SetActive(true);
            car_cost_text.text = DatabaseManager.Instance.allMetaDataServer[carIndex].cost.ToString();
        }        

       

        /*if (carIndex == 0)
        {
            carInfo_txt.text = "Default Car";
        }
        else
        {
            //carInfo_txt.text = DatabaseManager.Instance.allMetaDataServer[car_details.car_index - 1].description;
            carInfo_txt.text = carIndex.ToString(); ;
        }*/
    }

    #endregion

    #region Car Selection / Buy Area
    private void PurchaseCar(int carIndex)
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        if(data.coins< DatabaseManager.Instance.allMetaDataServer[carIndex].cost)
        {
            MessageBox.insta.showMsg("Not Enough Coins!", true);
            return;
        }

       /* data.coins -= DatabaseManager.Instance.allMetaDataServer[carIndex].cost;
        DatabaseManager.Instance*/
        lastSelectedButton = carIndex;
        //car Index Starts from 0 and 0-is default car. So Buy index-1
        CoreWeb3Manager.Instance.purchaseItem(carIndex-1,false);
    }

    private void SelectCar(int carIndex)
    {
        buy_select_btn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Selected";
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.SelectedSkin = carIndex;
        DatabaseManager.Instance.UpdateData(data);
      
        this.gameObject.SetActive(false);
    }
    #endregion
    public void SelectItem(int _no, Texture _texture)
    {
        Debug.Log("Selected item " + _no);
        currentSelectedItem = _no;
      
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
