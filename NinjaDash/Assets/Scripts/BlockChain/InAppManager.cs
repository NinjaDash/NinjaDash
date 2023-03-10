using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
    public static InAppManager Instance;
    private void Awake()
    {
        Instance = this;        
    }

    [SerializeField] TMP_Text[] priceTexts;

    async void OnEnable()
    {
        balanceText.text = "";

        if (CoreWeb3Manager.Instance)
        {
            CoreWeb3Manager.Instance.CheckUserBalance();

            for (int i = 0; i < priceTexts.Length; i++)
            {
                priceTexts[i].text = CoreWeb3Manager.Instance.coinCost[i].ToString();
            }
        }
        
        SetBalanceText();
    }

    [SerializeField] TMP_Text balanceText;
    public void SetBalanceText()
    {
        balanceText.text = "Balance : " + CoreWeb3Manager.userBalance.ToString();
    }
    public void purchaseCoins(int index)
    {
        CoreWeb3Manager.Instance.CoinBuyOnSendContract(index);
    }

    public void ExchangeCoins(int index)
    {
        int tokenBalance = System.Int32.Parse(CoreWeb3Manager.userTokenBalance);
        if (tokenBalance >= index)
        {
            CoreWeb3Manager.Instance.ExchangeToken(index);
        }
        else
        {
            MessageBox.insta.showMsg("Not Enough Tokens to exchange", true);
        }
    }

}
